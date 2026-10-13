namespace MaturaGenerator.Models;

public class GeneratedProblem
{
    public ProblemTemplate Template { get; set; } = null!;
    public string RenderedText { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, object> ConstantAssignments { get; set; } = new Dictionary<string, object>();
}
