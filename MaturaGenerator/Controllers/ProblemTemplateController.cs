using MaturaGenerator.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MaturaGenerator.Evaluation;
using MaturaGenerator.Services;

namespace MaturaGenerator.Controllers;

public class ProblemTemplateController(
    IProblemTemplateService templateService,
    IRuleEvaluator ruleEvaluator) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await templateService.GetAllTemplatesAsync());

    [HttpGet]
    public IActionResult Create() => View(new ProblemTemplateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProblemTemplateViewModel model)
    {
        ValidateRules(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await templateService.CreateTemplateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await templateService.GetTemplateForEditAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProblemTemplateViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        ValidateRules(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await templateService.UpdateTemplateAsync(id, model);
        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await templateService.DeleteTemplateAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private void ValidateRules(ProblemTemplateViewModel model)
    {
        for (int i = 0; i < model.Rules.Count; i++)
        {
            var error = ruleEvaluator.ValidateRule(model.Rules[i]);
            if (error != null)
            {
                ModelState.AddModelError($"Rules[{i}]", $"Невалиден синтаксис на правило: {error}");
            }
        }
    }
}
