using MaturaGenerator.Evaluation;
using MaturaGenerator.MathEngine.Ast;
using MaturaGenerator.MathEngine.Parsing;
using MaturaGenerator.MathEngine.Rendering;
using MaturaGenerator.MathEngine.Transforms;
using MaturaGenerator.Models;

namespace MaturaGenerator.Services;

public class ProblemGeneratorService(IRuleEvaluator ruleEvaluator) : IProblemGeneratorService
{
    private readonly IRuleEvaluator _ruleEvaluator = ruleEvaluator;
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
        var ruleExpressions = (template.Rules ?? Enumerable.Empty<TemplateRule>())
            .Select(r => r.Expression)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToList();

        Dictionary<string, object>? validAssignments = null;

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var candidate = RollConstants(template.Domains, rng);

            if (ruleExpressions.Count == 0 || _ruleEvaluator.SatisfiesAll(ruleExpressions, candidate))
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

        // 1. Parse template structure into AST
        MathNode rawAst;
        try
        {
            rawAst = MathParser.Parse(template.Structure);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Failed to parse problem structure '{template.Structure}': {ex.Message}", ex);
        }

        // 2. Substitute constants with generated values
        var substitutedAst = ConstantSubstitutor.Substitute(rawAst, assignments);

        // 3. Canonically simplify AST (eliminates "+ -", redundant 1 multipliers, etc.)
        var simplifiedAst = MathSimplifier.Simplify(substitutedAst);

        // 4. Render to LaTeX
        var latex = LatexRenderer.Render(simplifiedAst);

        return new GeneratedProblem
        {
            Template = template,
            RenderedText = latex,
            Latex = latex,
            Ast = simplifiedAst,
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
