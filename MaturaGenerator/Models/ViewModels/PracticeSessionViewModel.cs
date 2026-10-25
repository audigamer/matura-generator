namespace MaturaGenerator.Models.ViewModels;

public abstract record PracticeSessionViewModel
{
    private PracticeSessionViewModel() { }

    public record Active(Guid SessionId, GeneratedProblem Problem, int TimeLimitSeconds) :
        PracticeSessionViewModel;

    public record Error(ProblemTemplate Template, ProblemGenerationError Details) :
        PracticeSessionViewModel;
}