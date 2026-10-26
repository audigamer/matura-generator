using MaturaGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaturaGenerator.Controllers;

public class PracticeController(
    IProblemTemplateService templateService,
    IPracticeService practiceService): Controller
{
    [HttpGet]
    public async Task<IActionResult> Generate(int id)
    {
        var template = await templateService.GetTemplateByIdAsync(id);

        if (template == null) return NotFound();

        var practiceSession = practiceService.StartPracticeSession(template);
        return View(practiceSession);
    }

    public IActionResult Check(Guid sessionId, decimal answer)
    {
        var answerCheckResult = practiceService.CheckAnswer(sessionId, answer);
        
        return PartialView("_AnswerResultPartial", answerCheckResult);
    }
}
