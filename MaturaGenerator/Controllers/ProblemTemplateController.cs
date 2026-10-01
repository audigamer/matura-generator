using MaturaGenerator.Data;
using MaturaGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Controllers;

public class ProblemTemplateController(MaturaDbContext context) : Controller
{
    private readonly MaturaDbContext _context = context;

    public IActionResult Index()
    {
        List<ProblemTemplate> templates = _context.ProblemTemplates
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        return View(templates);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost] 
    public IActionResult Create(ProblemTemplate template)
    {
        if (!ModelState.IsValid)
        {
            return View(template);
        }

        _context.ProblemTemplates.Add(template);
        _context.SaveChanges();
        
        return RedirectToAction(nameof(Index));
    }
}