using MaturaGenerator.Models;

namespace MaturaGenerator.Services;

public interface IProblemGeneratorService
{
    /// <summary>
    /// Generates a concrete problem instance given a problem template and its variable domains.
    /// </summary>
    /// <param name="template">The template containing the statement and domain constraints.</param>
    /// <returns>A generated problem with concrete values assigned to constants.</returns>
    GeneratedProblem Generate(ProblemTemplate template);
}
