using MaturaGenerator.Data;
using MaturaGenerator.Models;
using MaturaGenerator.Models.DomainConstraints;
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