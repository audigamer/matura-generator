using MaturaGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaturaGenerator.Controllers;

public class HomeController(IProblemTemplateService templateService) : Controller
{
    public async Task<IActionResult> Index()
    {
        const int showcasedProblemId = 1;
        
        var sampleTemplateId = await templateService.GetSampleTemplateIdAsync(showcasedProblemId);

        return View(sampleTemplateId);
    }

    public IActionResult Help()
    {
        return View();
    }
}
