namespace MaturaGenerator.Models;

public class PracticeProblemState
{
    public Guid SessionId { get; init; } = Guid.NewGuid();
    public int TemplateId { get; init; }
    public decimal CorrectAnswer { get; init; }
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
    public TimeSpan TimeLimit { get; init; }
}