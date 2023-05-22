using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Services
{
    public class GoogleMapTestService
    {
        private readonly IConfiguration _configuration;

        public GoogleMapTestService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ActionResult<string>> searchRestaurant(double lat, double lng)
        {

            string api_key = _configuration["MapsApiKey"];
            int radius = 500;

            using (HttpClient client = new HttpClient())
            {
                // 建立請求 URL
                string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lng}&radius={radius}&type=restaurant&language=zh-TW&key={api_key}";

                // 發送 GET 請求
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // 讀取回應內容
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // 在這裡處理回應內容（JSON 資料）
                    Console.WriteLine(responseBody);

                    return responseBody;
                }
                else
                {
                    Console.WriteLine("發生錯誤：" + response.StatusCode);

                    return "發生錯誤：" + response.StatusCode;
                }
            }
        }
    }
}
