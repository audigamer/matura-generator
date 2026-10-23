namespace MaturaGenerator.Models;

public class GeneratedProblem
{
    public ProblemTemplate Template { get; set; } = null!;
    public string RenderedText { get; set; } = string.Empty;
    public string Latex { get; set; } = string.Empty;
    public MathEngine.Ast.MathNode? Ast { get; set; }
    public IReadOnlyDictionary<string, object> ConstantAssignments { get; set; } = new Dictionary<string, object>();
    public decimal ExpectedAnswer { get; set; }
    public string? ErrorMessage { get; set; }
    
}
