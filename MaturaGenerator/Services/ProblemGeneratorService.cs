using System.Text.RegularExpressions;
using MaturaGenerator.Evaluation;
using MaturaGenerator.MathEngine.Ast;
using MaturaGenerator.MathEngine.Parsing;
using MaturaGenerator.MathEngine.Rendering;
using MaturaGenerator.MathEngine.Transforms;
using MaturaGenerator.Models;

namespace MaturaGenerator.Services;

public class ProblemGeneratorService(IRuleEvaluator ruleEvaluator) : IProblemGeneratorService
{
    private static readonly Regex MathBracketRegex = new(@"\[(.*?)\]", RegexOptions.Compiled);
    private const int MaxAttempts = 150;

    public GeneratedProblem Generate(ProblemTemplate template)
    {
        ValidateTemplate(template);

        var assignments = ResolveValidAssignments(template);
        var (renderedStatement, ast) = RenderStatement(template.Statement, assignments);

        decimal expectedAnswer = GetExpectedAnswer(template.AnswerRule?.CorrectAnswerRule, assignments);
        

        return new GeneratedProblem
        {
            Template = template,
            RenderedText = renderedStatement,
            Latex = renderedStatement,
            Ast = ast,
            ConstantAssignments = assignments,
            ExpectedAnswer = expectedAnswer
        };
    }

    private decimal GetExpectedAnswer(
        string? correctAnswerRule,
        IReadOnlyDictionary<string,object> constantAssignments)
    {
        if (string.IsNullOrWhiteSpace(correctAnswerRule))
        {
            throw new InvalidOperationException(
                "Cannot practice or evaluate this problem because it does not have a configured AnswerRule.");
        }
        
        try
        {
            var rawResult = ruleEvaluator.Evaluate(correctAnswerRule, constantAssignments);
            return Convert.ToDecimal(rawResult);
        }
        catch (Exception e)
        {

            throw new InvalidOperationException(
                $"Couldn't evaluate the correct answer for ProblemTemplate with answer rule {correctAnswerRule}. Check for syntax errors.");
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

        throw new InvalidOperationException(
            $"Unable to satisfy template rules for ProblemTemplate ID {template.Id} ('{template.Title}') after {MaxAttempts} attempts. " +
            $"Please verify that the domains and rules are mathematically satisfiable.");
    }

    private static (string RenderedText, MathNode? LastAst) RenderStatement(
        string statement, 
        Dictionary<string, object> assignments)
    {
        MathNode? lastAst = null;

        var renderedText = MathBracketRegex.Replace(statement, match =>
        {
            var mathExpr = match.Groups[1].Value.Trim();
            var (latex, ast) = RenderMathExpression(mathExpr, assignments);
            lastAst = ast;
            return $"${latex}$";
        });

        return (renderedText, lastAst);
    }

    private static (string Latex, MathNode Ast) RenderMathExpression(
        string mathExpr, 
        Dictionary<string, object> assignments)
    {
        try
        {
            var rawAst = MathParser.Parse(mathExpr);
            var substitutedAst = ConstantSubstitutor.Substitute(rawAst, assignments);
            var simplifiedAst = MathSimplifier.Simplify(substitutedAst);
            var latex = LatexRenderer.Render(simplifiedAst);

            return (latex, simplifiedAst);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Failed to parse math expression '[{mathExpr}]' in statement: {ex.Message}", ex);
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
