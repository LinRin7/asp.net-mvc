using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using System.Diagnostics;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // 使用者已通過身份驗證，執行已認證的邏輯
                Console.WriteLine("此使用者已通過身份驗證");
            }
            else
            {
                // 使用者未通過身份驗證，執行未認證的邏輯
                Console.WriteLine("此使用者未通過身份驗證");
            }

            if (TempData["Message"] is not null and not (object?)"")
            {
                var message = TempData["Message"] as string;
                ViewBag.Message = message;
            }
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}