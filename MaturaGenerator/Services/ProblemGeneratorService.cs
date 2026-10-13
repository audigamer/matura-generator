using System.Text.RegularExpressions;
using MaturaGenerator.Models;

namespace MaturaGenerator.Services;

public class ProblemGeneratorService : IProblemGeneratorService
{
    private static readonly Regex ConstantPlaceholderRegex = new(@"@([a-zA-Z0-9_]+)", RegexOptions.Compiled);

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

        // Render structure by substituting placeholder constants
        var rendered = ConstantPlaceholderRegex.Replace(template.Structure, match =>
        {
            var varName = match.Groups[1].Value;
            if (assignments.TryGetValue(varName, out var value))
            {
                return value?.ToString() ?? string.Empty;
            }

            // Fallback: leave match as-is if no domain was specified
            return match.Value;
        });

        return new GeneratedProblem
        {
            Template = template,
            RenderedText = rendered,
            ConstantAssignments = assignments
        };
    }
}
