using Microsoft.AspNetCore.Mvc;
using MyWebApp.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MyWebApp.Controllers
{
    public class GoogleMapTestController : Controller
    {
        private readonly IConfiguration _configuration;//DI，取得appsetting.json的資料
        private readonly GoogleMapTestService _googleMapTestService;

        public GoogleMapTestController(IConfiguration configuration, GoogleMapTestService googleMapTestService)
        {
            _configuration = configuration;
            _googleMapTestService = googleMapTestService;
        }

        public IActionResult Index2()
        {
            var srcUrl = _configuration["MapsApiKey"];
            ViewBag.MapsApiKey = $"https://maps.googleapis.com/maps/api/js?key={srcUrl}&callback=initMap";
            return View();
        }

        public async Task<ActionResult<string>> searchRestaurant(double lat, double lng)//搜尋餐廳
        {
            var result = await _googleMapTestService.searchRestaurant(lat, lng);
            return result;
        }


    }
}
