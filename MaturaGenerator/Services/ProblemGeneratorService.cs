using MaturaGenerator.MathEngine.Ast;
using MaturaGenerator.MathEngine.Parsing;
using MaturaGenerator.MathEngine.Rendering;
using MaturaGenerator.MathEngine.Transforms;
using MaturaGenerator.Models;

namespace MaturaGenerator.Services;

public class ProblemGeneratorService : IProblemGeneratorService
{
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
        var assignments = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // Map domains to their designated constant names
        foreach (var domain in template.Domains)
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
}
