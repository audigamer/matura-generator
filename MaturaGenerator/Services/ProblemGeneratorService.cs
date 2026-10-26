using System.Text.RegularExpressions;
using MaturaGenerator.Evaluation;
using MaturaGenerator.MathEngine.Ast;
using MaturaGenerator.MathEngine.Parsing;
using MaturaGenerator.MathEngine.Rendering;
using MaturaGenerator.MathEngine.Transforms;
using MaturaGenerator.Models;
using MaturaGenerator.Models.AnswerRules;
using MaturaGenerator.Models.DomainConstraints;

namespace MaturaGenerator.Services;

public class ProblemGeneratorService(IRuleEvaluator ruleEvaluator) : IProblemGeneratorService
{
    private static readonly Regex MathBracketRegex = new(@"\[(.*?)\]", RegexOptions.Compiled);
    private const int MaxAttempts = 150;

    public GeneratedProblem Generate(ProblemTemplate template)
    {
        ValidateTemplate(template);
        
        // Time limit for the problem generation process.
        // Avoids expressions that are very performance intensive or made to waste server resources
        // E.g. 9999999!
        try
        {
            return Task.Run(() => GenerateInternal(template))
                .WaitAsync(TimeSpan.FromMilliseconds(500))
                .GetAwaiter()
                .GetResult();
        }
        catch (TimeoutException e)
        {
            throw new ProblemGenerationException(ProblemGenerationError.RuleEvaluationTimeout, e);
        }
    }

    private GeneratedProblem GenerateInternal(ProblemTemplate template)
    {
        var assignments = ResolveValidAssignments(template);
        var renderedStatement = RenderStatement(template.Statement, assignments);

        decimal expectedAnswer = GetExpectedAnswer(template.AnswerRule?.CorrectAnswerRule, assignments);

        GeneratedAnswerInput answerInput = template.AnswerRule switch
        {
            CustomAnswerRule custom => new GeneratedAnswerInput.Slider(
                custom.SliderBottomBound,
                custom.SliderUpperBound),

            MultiChoiceAnswerRule multi => new GeneratedAnswerInput.MultipleChoice(
                EvaluateChoices(multi, assignments, expectedAnswer)),

            _ => throw new ProblemGenerationException(ProblemGenerationError.MissingAnswerRule)
        };

        return new GeneratedProblem
        {
            Template = template,
            RenderedText = renderedStatement,
            ExpectedAnswer = expectedAnswer,
            AnswerInput = answerInput
        };
    }

    private IReadOnlyList<decimal> EvaluateChoices(
        MultiChoiceAnswerRule multiChoice,
        IReadOnlyDictionary<string, object> constantAssignments,
        decimal expectedAnswer)
    {
        var options = new List<decimal> { expectedAnswer };

        foreach (var choiceRule in multiChoice.ChoiceRules)
        {
            if (string.IsNullOrWhiteSpace(choiceRule)) continue;

            try
            {
                var rawResult = ruleEvaluator.Evaluate(choiceRule, constantAssignments);
                options.Add(Convert.ToDecimal(rawResult));
            }
            catch (Exception e)
            {
                throw new ProblemGenerationException(ProblemGenerationError.InvalidAnswerRuleSyntax, e);
            }
        }

        return options.OrderBy(_ => Random.Shared.Next()).ToList();
    }

    private decimal GetExpectedAnswer(
        string? correctAnswerRule,
        IReadOnlyDictionary<string, object> constantAssignments)
    {
        if (string.IsNullOrWhiteSpace(correctAnswerRule))
            throw new ProblemGenerationException(ProblemGenerationError.MissingAnswerRule);

        try
        {
            var rawResult = ruleEvaluator.Evaluate(correctAnswerRule, constantAssignments);
            return Convert.ToDecimal(rawResult);
        }
        catch (Exception e)
        {
            throw new ProblemGenerationException(ProblemGenerationError.InvalidAnswerRuleSyntax, e);
        }
    }

    private static void ValidateTemplate(ProblemTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);

        if (template.Domains is null)
        {
            throw new InvalidOperationException(
                $"Cannot generate a problem from ProblemTemplate ID {template.Id} ('{template.Title}') because its '{nameof(template.Domains)}' collection was not loaded. " +
                $"Ensure you queried the template using '.Include(t => t.Domains)'.");
        }

        if (template.Rules is null)
        {
            throw new InvalidOperationException(
                $"Cannot generate a problem from ProblemTemplate ID {template.Id} ('{template.Title}') because its '{nameof(template.Rules)}' collection was not loaded. " +
                $"Ensure you queried the template using '.Include(t => t.Rules)'.");
        }
    }

    private Dictionary<string, object> ResolveValidAssignments(ProblemTemplate template)
    {
        var rng = Random.Shared;
        var ruleExpressions = template.Rules
            .Select(r => r.Expression)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToList();

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var candidate = RollConstants(template.Domains, rng);

            if (ruleExpressions.Count == 0 || ruleEvaluator.SatisfiesAll(ruleExpressions, candidate))
            {
                return candidate;
            }
        }

        throw new ProblemGenerationException(ProblemGenerationError.UnsatisfiableRules);
    }

    private static string RenderStatement(
        string statement, 
        Dictionary<string, object> assignments)
    {
        return MathBracketRegex.Replace(statement, match =>
        {
            var mathExpr = match.Groups[1].Value.Trim();
            var latex = RenderMathExpression(mathExpr, assignments);
            return $"${latex}$";
        });
    }

    private static string RenderMathExpression(
        string mathExpr, 
        Dictionary<string, object> assignments)
    {
        try
        {
            var rawAst = MathParser.Parse(mathExpr);
            var substitutedAst = ConstantSubstitutor.Substitute(rawAst, assignments);
            var simplifiedAst = MathSimplifier.Simplify(substitutedAst);
            return LatexRenderer.Render(simplifiedAst);
        }
        catch (Exception e)
        {
            throw new ProblemGenerationException(ProblemGenerationError.StatementParsingError, e);
        }
    }

    private static Dictionary<string, object> RollConstants(IEnumerable<DomainConstraint> domains, Random rng)
    {
        var assignments = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var domain in domains)
        {
            if (string.IsNullOrWhiteSpace(domain.ConstantNames))
                continue;

            var names = domain.ConstantNames
                .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var name in names)
            {
                var cleanName = name.TrimStart('@');
                var value = domain.GenerateValue(rng);
                assignments[cleanName] = value;
            }
        }

        return assignments;
    }
}
