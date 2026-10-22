using MaturaGenerator.Data;
using MaturaGenerator.Evaluation;
using MaturaGenerator.Models;
using MaturaGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Controllers;

public class PracticeController(
    MaturaDbContext context,
    IProblemGeneratorService problemGenerator): Controller
{
    [HttpGet]
    public IActionResult Generate(int id)
    {
        var template = context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .Include(t => t.AnswerRule)
            .FirstOrDefault(t => t.Id == id);

        if (template == null)
        {
            return NotFound();
        }

        try
        {
            var generated = problemGenerator.Generate(template);
            return View(generated);
        }
        catch (InvalidOperationException ex)
        {
            var failedProblem = new GeneratedProblem
            {
                Template = template,
                ErrorMessage = ex.Message
            };
            return View(failedProblem);
        }
    }
}