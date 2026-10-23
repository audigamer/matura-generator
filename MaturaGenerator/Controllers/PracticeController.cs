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
    public IActionResult Generate(int id)
    {
        var template = context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .Include(t => t.AnswerRule)
            .FirstOrDefault(t => t.Id == id);

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