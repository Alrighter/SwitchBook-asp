using Microsoft.AspNetCore.Mvc;

namespace SwitchBook.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Contacts()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
}