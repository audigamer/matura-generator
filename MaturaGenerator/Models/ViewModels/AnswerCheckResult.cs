namespace MaturaGenerator.Models.ViewModels;

public abstract record AnswerCheckResult
{
    private AnswerCheckResult() { }

    public record Correct(int TimeTakenSeconds) : AnswerCheckResult;
    public record Incorrect(decimal CorrectAnswer, int TimeTakenSeconds) : AnswerCheckResult;
    public record TimeExpired(int TimeTakenSeconds) : AnswerCheckResult;
    public record SessionNotFound : AnswerCheckResult;
}