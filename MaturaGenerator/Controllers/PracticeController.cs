using MaturaGenerator.Data;
using MaturaGenerator.Models;
using MaturaGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Controllers;

public class PracticeController(
    MaturaDbContext context,
    IPracticeService practiceService): Controller
{
    [HttpGet]
    public async Task<IActionResult> Generate(int id)
    {
        var template = await context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .Include(t => t.AnswerRule)
            .FirstOrDefaultAsync(t => t.Id == id);

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