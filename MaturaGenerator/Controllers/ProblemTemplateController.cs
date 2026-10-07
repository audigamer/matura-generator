using MaturaGenerator.Data;
using MaturaGenerator.Models;
using MaturaGenerator.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Controllers;

public class ProblemTemplateController(MaturaDbContext context) : Controller
{
    private readonly MaturaDbContext _context = context;

    public IActionResult Index() =>
        View(_context.ProblemTemplates.AsNoTracking().OrderByDescending(t => t.CreatedAt).ToList());

    [HttpGet]
    public IActionResult Create() => View(new ProblemTemplateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProblemTemplateViewModel model)
    {
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
            Domains = model.Domains.Select(d => d.ToDomainEntity()).ToList()
        };

        _context.ProblemTemplates.Add(template);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var template = _context.ProblemTemplates
            .Include(t => t.Domains)
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
            Domains = template.Domains.Select(DomainInputModel.FromDomainEntity).ToList()
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

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = _context.ProblemTemplates
            .Include(t => t.Domains)
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

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var template = _context.ProblemTemplates.Find(id);
        if (template != null)
        {
            _context.ProblemTemplates.Remove(template);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }
}