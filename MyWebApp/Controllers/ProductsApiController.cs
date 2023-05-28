using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Services;
using Newtonsoft.Json;
using System;
using System.Composition;

namespace MyWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly MyWebAppDbContext _context;

        private readonly ProductService _productService;

        public ProductsApiController(MyWebAppDbContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()//取得全部商品
        {
            //https://localhost:7218/api/ProductsApi
            return await _productService.GetProducts();
        }

        [HttpGet("GetProductsJson")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsJson()//取得全部商品(Json)
        {
            //https://localhost:7218/api/ProductsApi/GetProductsJson
            var json = JsonConvert.SerializeObject(await _productService.GetProductsJson());
            return Content(json, "application/json");
        }

        [HttpGet("GetPageProducts/{page}")]
        public async Task<IEnumerable<Product>> GetPageProducts(int page)//取得指定頁數的商品
        {
            /*
             * https://localhost:7218/api/ProductsApi/GetPageProducts/XX
             * */
            Console.WriteLine("頁數：" + page);
            int pageSize = 6;//參考ProductController
            var json = await _productService.PageGetProducts(page, pageSize, "all");
            var productsToShow = json.Skip((Convert.ToInt32(page) - 1) * pageSize).Take(pageSize);
            return productsToShow;
        }

        [HttpGet("GetTheProduct")]
        public async Task<ActionResult<Product>> GetTheProduct(int productId)//取得指定id商品
        {
            //https://localhost:7218/api/ProductsApi/GetTheProduct?productId=40
            var product = await _productService.GetTheProduct(productId);
            if(product == null)
            {
                return NotFound();
            }
            else
            {
                //var json = JsonConvert.SerializeObject(product);
                //return Content(json, "application/json");
                return product;
            }
        }

        [HttpPost("NewProduct")]
        [Produces("application/json")] // 指定傳入請求的格式為 JSON
        [Authorize(Policy = "AdminOnly")]
        //[ValidateAntiForgeryToken]
        //防偽標記值通常是在後端生成並傳遞到前端視圖中
        //請求的Header需新增 key = __RequestVerificationToken
        //value在開發人員工具中，切換到網路（Network）選項找
        public async Task<ActionResult> NewProduct(Product product)//新增商品
        {
            var msg = new { message = await _productService.NewProduct(product) };
            var resultJson = JsonConvert.SerializeObject(msg);
            return Content(resultJson, "application/json");
        }


        [HttpPut("EditProductPut")]
        [Produces("application/json")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditProductPut(int productId, [FromBody] Product updateProduct)
        {   
            var msg = await _productService.EditProductPut(productId, updateProduct);
            var resultJson = JsonConvert.SerializeObject(new { message = msg});
            return Content(resultJson, "application/json");
        }


        [HttpPatch("EditProductPatch")]
        [Produces("application/json")]
        [Authorize(Policy = "AdminOnly")]
        //Patch參考資料
        //https://learn.microsoft.com/zh-tw/aspnet/core/web-api/jsonpatch?view=aspnetcore-7.0
        public async Task<IActionResult> EditProductPatch(int productId, JsonPatchDocument<Product> patchDoc)
        {
            var msg = await _productService.EditProductPatch(productId, patchDoc);
            var resultJson = JsonConvert.SerializeObject(new { message = msg});
            return Content(resultJson, "application/json");
        }

        [HttpDelete("DeleteProduct")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var msg = await _productService.DeleteProduct(productId);
            var resultJson = JsonConvert.SerializeObject(new { message = msg});
            return Content(resultJson, "application/json");
        }
    }
}
