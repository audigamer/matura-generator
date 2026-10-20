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
        ArgumentNullException.ThrowIfNull(template);

        if (template.Domains is null)
        {
            throw new InvalidOperationException(
                $"Cannot generate a problem from ProblemTemplate ID {template.Id} ('{template.Title}') because its '{nameof(template.Domains)}' collection was not loaded. " +
                $"Ensure you queried the template using '.Include(t => t.Domains)'.");
        }

        var rng = Random.Shared;
        var ruleExpressions = template.Rules
            .Select(r => r.Expression)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToList();

        Dictionary<string, object>? validAssignments = null;

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var candidate = RollConstants(template.Domains, rng);

            if (ruleExpressions.Count == 0 || ruleEvaluator.SatisfiesAll(ruleExpressions, candidate))
            {
                validAssignments = candidate;
                break;
            }
        }

        if (validAssignments == null)
        {
            throw new InvalidOperationException(
                $"Unable to satisfy template rules for ProblemTemplate ID {template.Id} ('{template.Title}') after {MaxAttempts} attempts. " +
                $"Please verify that the domains and rules are mathematically satisfiable.");
        }

        var assignments = validAssignments;
        MathNode? lastAst = null;

        var statement = template.Statement;

        var renderedStatement = MathBracketRegex.Replace(statement, match =>
        {
            var mathExpr = match.Groups[1].Value.Trim();
            try
            {
                var rawAst = MathParser.Parse(mathExpr);
                var substitutedAst = ConstantSubstitutor.Substitute(rawAst, assignments);
                var simplifiedAst = MathSimplifier.Simplify(substitutedAst);
                lastAst = simplifiedAst;
                var latex = LatexRenderer.Render(simplifiedAst);
                return $"${latex}$";
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse math expression '[{mathExpr}]' in statement: {ex.Message}", ex);
            }
        });

        return new GeneratedProblem
        {
            Template = template,
            RenderedText = renderedStatement,
            Latex = renderedStatement,
            Ast = lastAst,
            ConstantAssignments = assignments
        };
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
