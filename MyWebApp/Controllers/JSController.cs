using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers
{
    public class JSController : Controller
    {
        public IActionResult Meal()
        {
            return View();
        }
    }
}
