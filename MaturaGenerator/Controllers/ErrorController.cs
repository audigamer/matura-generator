using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MaturaGenerator.Models;

namespace MaturaGenerator.Controllers;

public class ErrorController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}