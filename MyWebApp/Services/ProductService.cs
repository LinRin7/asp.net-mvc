using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MyWebApp.Services
{
    public class ProductService
    {
        private readonly MyWebAppDbContext _context;

        public ProductService(MyWebAppDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Product.ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Product>>> GetProductsJson()
        {
            return await _context.Product.ToListAsync();
        }

        public async Task<ActionResult<Product>?> GetTheProduct(int productId)
        {
            var product = await _context.Product.FirstOrDefaultAsync(e => e.Id == productId);
            if (product == null)
            {
                return null;
            }
            var json = JsonConvert.SerializeObject(product);
            return product;
        }

        public async Task<string> NewProduct(Product product)
        {
            string msg = "";
            try
            {
                if (product.Picture.IndexOf("file/d/") > 0)
                {
                    product.Picture = product.Picture.Replace("file/d/", "uc?export=view&id=");
                }
                if (product.Picture.LastIndexOf("/view?usp=share_link") > 0)
                {
                    product.Picture = product.Picture.Replace("/view?usp=share_link", "");
                }
                if (product.Picture.LastIndexOf("/view") > 0)
                {
                    product.Picture = product.Picture.Replace("/view", "");
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                msg = "新增商品成功。";
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return msg;
        }


        public async Task<string> EditProductPut(int productId, Product updateProduct)
        {
            string msg = "";

            try
            {
                var product = await _context.Product.Where(i => i.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    msg = "無此商品Id";
                    return msg;
                }
                product.setValue(updateProduct);
                await _context.SaveChangesAsync();
                msg = "編輯成功";
                var resultJson = JsonConvert.SerializeObject(msg);
                return msg;
            }
            catch (Exception e)
            {
                msg = e.Message;
                return msg;
            }
        }


        public async Task<string> EditProductPatch(int productId, JsonPatchDocument<Product> patchDoc)
        {
            string msg = "";

            try
            {
                var product = await _context.Product.Where(i => i.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    msg = "無此商品Id";
                    var fail404 = JsonConvert.SerializeObject(msg);
                    return msg;
                }
                patchDoc.ApplyTo(product);
                await _context.SaveChangesAsync();
                msg = "編輯成功";
                var resultJson = JsonConvert.SerializeObject(msg);
                return msg;
            }
            catch (Exception e)
            {
                msg = e.Message;
                return msg;
            }
        }

        public async Task<string> DeleteProduct(int productId)
        {
            string msg = "";

            try
            {
                var product = await _context.Product.Where(i => i.Id == productId).FirstOrDefaultAsync();
                if (product == null)
                {
                    msg = "無此商品Id";
                    var fail404 = JsonConvert.SerializeObject(msg);
                    return msg;
                }
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                msg = "移除商品成功";
                var resultJson = JsonConvert.SerializeObject(msg);
                return msg;
            }
            catch (Exception e)
            {
                msg = e.Message;
                return msg;
            }
        }

        public async Task<List<Product>> PageGetProducts(int page, int pageSize, string productCategory, string? search = null)
        {
            if (page == 0)
            {
                page = 1;
            }
            List<Product>? products = null;
            if (search == null)
            {
                if (productCategory == "all")
                {
                    var Iproducts = _context.Product.OrderBy(p => p.Id);//總商品
                    products = await Iproducts.ToListAsync();
                }
                else
                {
                    var Iproducts = _context.Product.Where(i => i.Category == productCategory).OrderBy(p => p.Id);//依總類搜尋商品
                    products = await Iproducts.ToListAsync();
                }
            }
            else
            {
                products = await _context.Product.Where(j => j.Name.Contains(search)).ToListAsync();
            }

            return products;
        }

        public async Task<IEnumerable<Product>> MemberToWishlistPage(int memberId)
        {
            var WishlistToShow = await (from Product in _context.Product
                                 join Wishlist in _context.Wishlist on Product.Id equals Wishlist.ProductId
                                 where Wishlist.MemberId == memberId
                                 select Product).ToListAsync();

            return WishlistToShow;
        }

        public async Task<Product> ProductPage(int productId)
        {
            var product = await _context.Product.FirstOrDefaultAsync(e => e.Id == productId);
            return product;
        }

        public async Task<string> AddWishlist(int productId, int memberId)//加入願望清單
        {
            string msg = "";
            if (!ProductExistsInWishlist(productId, memberId))
            {
                Wishlist wishlist = new Wishlist();
                wishlist.MemberId = memberId;
                wishlist.ProductId = productId;
                _context.Wishlist.Add(wishlist);
                await _context.SaveChangesAsync();
                msg = "加入至願望清單";
            }
            else
            {
                msg = "已在願望清單";
            }
            return msg;
        }


        public async Task<string> DeleteFromWishlist(int productId, int memberId)
        {
            string msg = "";

            var Wishlist = await _context.Wishlist.Where(e => e.ProductId == productId && e.MemberId == memberId).FirstAsync();
            if (Wishlist != null)
            {
                _context.Wishlist.Remove(Wishlist);
                await _context.SaveChangesAsync();
                msg = "移除成功";
            }

            return msg;
        }


        private bool ProductExists(int id)//檢查商品是否存在
        {
            return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool ProductExistsInWishlist(int productId, int memberId)//檢查商品是否存在該會員的願望清單
        {
            return (_context.Wishlist?.Any(e => e.MemberId == memberId && e.ProductId == productId)).GetValueOrDefault();
        }
    }
}
