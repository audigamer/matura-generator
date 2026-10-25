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

            var timeLimitSeconds = DefaultProblemTimeSeconds; // TODO: Make dependent on the problem
            var sessionId = Guid.NewGuid();

            var state = new PracticeProblemState
            {
                SessionId = sessionId,
                TemplateId = template.Id,
                CorrectAnswer = problem.ExpectedAnswer,
                StartedAt = DateTimeOffset.UtcNow,
                TimeLimit = TimeSpan.FromSeconds(timeLimitSeconds)
            };

            // Cache will be used temporarily as a simpler way to store practice sessions.
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
        
        Console.WriteLine($"userAnswer: {userAnswer}, correctAnswer: {state.CorrectAnswer}");
        
        if (elapsed > state.TimeLimit)
        {
            return new AnswerCheckResult.Correct(elapsed.Seconds);
        }
        
        bool isCorrect = Math.Abs(state.CorrectAnswer - userAnswer) < PrecisionTolerance;

        if (isCorrect)
            return new AnswerCheckResult.Correct(elapsed.Seconds);
        
        return new AnswerCheckResult.Incorrect(state.CorrectAnswer, elapsed.Seconds);
    }
}