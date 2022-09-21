using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SwitchBook.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            
            return View();
        }
    }
}
