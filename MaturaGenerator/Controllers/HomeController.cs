using MaturaGenerator.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Controllers;

public class HomeController(MaturaDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        const int showcasedProblemId = 1;
        
        var sampleTemplateId = await context.ProblemTemplates
            .Where(t => t.Id == showcasedProblemId)
            .Select(t => t.Id)
            .SingleAsync();

        return View(sampleTemplateId);
    }
}
