using MaturaGenerator.Models;
using MaturaGenerator.Models.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace MaturaGenerator.Services;

public class PracticeService(
    IProblemGeneratorService problemGenerator,
    IMemoryCache cache) : IPracticeService
{
    private const int DefaultProblemTimeSeconds = 120;
    private const decimal PrecisionTolerance = 0.01m;
    
    public PracticeSessionViewModel StartPracticeSession(ProblemTemplate template)
    {
        try
        {
            var problem = problemGenerator.Generate(template);

            var timeLimitSeconds = DefaultProblemTimeSeconds;
            var sessionId = Guid.NewGuid();

            var state = new PracticeProblemState
            {
                SessionId = sessionId,
                TemplateId = template.Id,
                CorrectAnswer = problem.ExpectedAnswer,
                StartedAt = DateTimeOffset.UtcNow,
                TimeLimit = TimeSpan.FromSeconds(timeLimitSeconds)
            };

            cache.Set(sessionId, state, TimeSpan.FromSeconds(timeLimitSeconds + 60));

            return new PracticeSessionViewModel.Active(sessionId, problem, timeLimitSeconds);
        }
        catch (ProblemGenerationException e)
        {
            return new PracticeSessionViewModel.Error(template, e.Error);
        }
    }

    public AnswerCheckResult CheckAnswer(Guid sessionId, decimal userAnswer)
    {
        if (!cache.TryGetValue(sessionId, out PracticeProblemState? state) || state == null)
        {
            return new AnswerCheckResult.SessionNotFound();
        }
        
        cache.Remove(sessionId);

        var elapsed = DateTimeOffset.UtcNow - state.StartedAt;
        var totalElapsedSeconds = (int)elapsed.TotalSeconds;

        if (elapsed > state.TimeLimit)
        {
            return new AnswerCheckResult.TimeExpired(totalElapsedSeconds);
        }
        
        bool isCorrect = Math.Abs(state.CorrectAnswer - userAnswer) < PrecisionTolerance;

        if (isCorrect)
            return new AnswerCheckResult.Correct(totalElapsedSeconds);
        
        return new AnswerCheckResult.Incorrect(state.CorrectAnswer, totalElapsedSeconds);
    }
}
