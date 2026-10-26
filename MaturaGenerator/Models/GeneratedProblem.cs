namespace MaturaGenerator.Models;

public class GeneratedProblem
{
    public ProblemTemplate Template { get; set; } = null!;
    public string RenderedText { get; set; } = string.Empty;
    public decimal ExpectedAnswer { get; set; }
    public GeneratedAnswerInput AnswerInput { get; set; } = null!;
}
