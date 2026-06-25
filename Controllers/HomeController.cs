using Microsoft.AspNetCore.Mvc;

namespace ForasKhadra.API.Controllers;

/// <summary>Serves the HTML pages (Razor views).</summary>
public class HomeController : Controller
{
    // GET /  -> dashboard
    public IActionResult Index() => View("Dashboard");

    // GET /Home/Chat -> chat page
    public IActionResult Chat() => View();
}
