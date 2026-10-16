using MaturaGenerator.Data;
using MaturaGenerator.Models;
using MaturaGenerator.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MaturaGenerator.Evaluation;
using MaturaGenerator.Services;

namespace MaturaGenerator.Controllers;

public class ProblemTemplateController(
    MaturaDbContext context,
    IProblemGeneratorService problemGenerator,
    IRuleEvaluator ruleEvaluator) : Controller
{
    public IActionResult Index() =>
        View(context.ProblemTemplates.AsNoTracking().OrderByDescending(t => t.CreatedAt).ToList());

    [HttpGet]
    public IActionResult Generate(int id)
    {
        var template = context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
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

    [HttpGet]
    public IActionResult Create() => View(new ProblemTemplateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProblemTemplateViewModel model)
    {
        ValidateRules(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var template = new ProblemTemplate
        {
            Title = model.Title,
            Structure = model.Structure,
            TargetGrade = model.TargetGrade,
            CreatedAt = DateTime.UtcNow,
            Domains = model.Domains.Select(d => d.ToDomainEntity()).ToList(),
            Rules = model.Rules
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => new TemplateRule { Expression = r.Trim() })
                .ToList()
        };

        context.ProblemTemplates.Add(template);
        context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var template = context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .FirstOrDefault(t => t.Id == id);

        if (template == null)
        {
            return NotFound();
        }

        var model = new ProblemTemplateViewModel
        {
            Id = template.Id,
            Title = template.Title,
            Structure = template.Structure,
            TargetGrade = template.TargetGrade,
            Domains = template.Domains.Select(DomainInputModel.FromDomainEntity).ToList(),
            Rules = template.Rules.Select(r => r.Expression).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ProblemTemplateViewModel model)
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

        var existing = context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .FirstOrDefault(t => t.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.Title = model.Title;
        existing.Structure = model.Structure;
        existing.TargetGrade = model.TargetGrade;

        existing.Domains.Clear();
        foreach (var d in model.Domains)
        {
            existing.Domains.Add(d.ToDomainEntity());
        }

        existing.Rules.Clear();
        foreach (var r in model.Rules.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            existing.Rules.Add(new TemplateRule { Expression = r.Trim() });
        }

        context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var template = context.ProblemTemplates.Find(id);
        if (template != null)
        {
            context.ProblemTemplates.Remove(template);
            context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    private void ValidateRules(ProblemTemplateViewModel model)
    {
        for (int i = 0; i < model.Rules.Count; i++)
        {
            var error = ruleEvaluator.ValidateRule(model.Rules[i]);
            if (error != null)
            {
                ModelState.AddModelError($"Rules[{i}]", $"Invalid rule syntax: {error}");
            }
        }
    }
}