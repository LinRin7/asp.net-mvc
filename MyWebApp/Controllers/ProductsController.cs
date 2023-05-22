using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyWebAppDbContext _context;
        private readonly ProductService _productService;
        private int pageSize;//1頁顯示的商品數量

        public ProductsController(MyWebAppDbContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
            pageSize = 6;
        }

        public async Task<IActionResult> Index(string? page, string? productCategory, string? searchPhrase)//商品頁面
        {
            int totalProducts = 0;
            int totalPages = 0;
            IEnumerable<Product> productsToShow = new List<Product>();
            if (page == null)
            {
                page = "1";
            }
            if(productCategory == null || productCategory == "")
            {
                productCategory = "all";
            }
            if(searchPhrase == null)
            {
                List<Product> products = await _productService.PageGetProducts(Convert.ToInt32(page), pageSize, productCategory);
                totalProducts = products.Count();//商品總數
                totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);//商品總頁數
                productsToShow = products.Skip((Convert.ToInt32(page) - 1) * pageSize).Take(pageSize);//分頁查詢，取出指定頁數的商品
            }
            else
            {
                return RedirectToAction("Search", new {page = Convert.ToInt32(page), search = searchPhrase });
            }
            

            ViewBag.pageSize = pageSize;//一頁顯示的商品數
            ViewBag.currentPage = Convert.ToInt32(page);//傳遞當前頁數
            ViewBag.totalPages = totalPages;//傳遞總頁數
            ViewBag.productCategory = productCategory;//傳遞搜尋商品類別
            if(searchPhrase != null)
            {
                ViewBag.searchPhrase = searchPhrase;
            }
            
            if (TempData["Message"] is not null and not (object?)"")
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(productsToShow);

        }

        public IActionResult IndexApi()
        {
            return View();
        }

        

        [HttpGet]
        public async Task<IActionResult> Search(int page, string search)
        {
            Console.WriteLine("搜尋字串為：" + search);
            //int page = 1;
            List<Product> products = await _productService.PageGetProducts(page, pageSize, "all", search);
            int totalProducts = products.Count();//商品總數
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);//商品總頁數
            var productsToShow = products.Skip((Convert.ToInt32(page) - 1) * pageSize).Take(pageSize);//分頁查詢，取出指定頁數的商品

            ViewBag.pageSize = pageSize;//一頁顯示的商品數
            ViewBag.currentPage = Convert.ToInt32(page);//傳遞當前頁數
            ViewBag.totalPages = totalPages;//傳遞總頁數
            ViewBag.productCategory = "all";//傳遞搜尋商品類別
            ViewBag.searchPhrase = search;
            return View("Index", productsToShow);
        }


        [Authorize]
        public async Task<IActionResult> WishlistPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                int id = Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value);

                return View(await _productService.MemberToWishlistPage(id));
            }
            TempData["Message"] = "需先登入會員";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ProductPage(string id)
        {
            return View(await _productService.ProductPage(Convert.ToInt32(id)));
        }

        [Authorize]
        public async Task<string> AddWishlist(string id)//加入願望清單
        {
            return await _productService.AddWishlist(Convert.ToInt32(id), Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value));
        }

        public async Task<string> DeleteFromWishlist(int id)
        {
            return await _productService.DeleteFromWishlist(id, Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value));
        }
    }
}
