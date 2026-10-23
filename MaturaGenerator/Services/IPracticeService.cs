using MaturaGenerator.Models;
using MaturaGenerator.Models.ViewModels;

namespace MaturaGenerator.Services;

public interface IPracticeService
{
    public PracticeSessionViewModel StartPracticeSession(ProblemTemplate template);
    
    AnswerCheckResult CheckAnswer(Guid sessionId, decimal userAnswer);
}