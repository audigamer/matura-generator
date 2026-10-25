using MaturaGenerator.Data;
using MaturaGenerator.Models;
using MaturaGenerator.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaturaGenerator.Evaluation;

namespace MaturaGenerator.Controllers;

public class ProblemTemplateController(
    MaturaDbContext context,
    IRuleEvaluator ruleEvaluator) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await context.ProblemTemplates.AsNoTracking().OrderByDescending(t => t.CreatedAt).ToListAsync());
    

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

        var template = new ProblemTemplate
        {
            Title = model.Title,
            Statement = model.Statement,
            TargetGrade = model.TargetGrade,
            CreatedAt = DateTime.UtcNow,
            Domains = model.Domains.Select(d => d.ToDomainEntity()).ToList(),
            Rules = model.Rules
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => new TemplateRule { Expression = r.Trim() })
                .ToList(),
            AnswerRule = model.AnswerRule.ToAnswerEntity()
        };

        context.ProblemTemplates.Add(template);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var template = await context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .Include(t => t.AnswerRule)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (template == null)
        {
            return NotFound();
        }

        var model = new ProblemTemplateViewModel
        {
            Id = template.Id,
            Title = template.Title,
            Statement = template.Statement,
            TargetGrade = template.TargetGrade,
            Domains = template.Domains.Select(DomainInputModel.FromDomainEntity).ToList(),
            Rules = template.Rules.Select(r => r.Expression).ToList(),
            AnswerRule = AnswersInputModel.FromAnswerEntity(template.AnswerRule)
        };

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

        var existing = await context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .Include(t => t.AnswerRule)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.Title = model.Title;
        existing.Statement = model.Statement;
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

        if (existing.AnswerRule != null)
        {
            context.AnswerRules.Remove(existing.AnswerRule);
        } 

        existing.AnswerRule = model.AnswerRule.ToAnswerEntity();

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var template = context.ProblemTemplates.Find(id);
        if (template != null)
        {
            context.ProblemTemplates.Remove(template);
            await context.SaveChangesAsync();
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