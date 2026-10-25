namespace MaturaGenerator.Models;

public class ProblemGenerationException(ProblemGenerationError error, Exception? innerException = null)
    : Exception(error.Description, innerException)
{
    public ProblemGenerationError Error { get; } = error;
}