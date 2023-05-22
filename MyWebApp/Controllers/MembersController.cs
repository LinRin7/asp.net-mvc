using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace MyWebApp.Controllers
{
    public class MembersController : Controller
    {
        private readonly MyWebAppDbContext _context;

        public MembersController(MyWebAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()//會員中心首頁
        {
            if (TempData["Message"] is not null and not (object?)"")
            {
                var message = TempData["Message"] as string;
                ViewBag.Message = message;
            }

            if (User.Identity.IsAuthenticated)
            {
                // 使用者已通過身份驗證，執行已認證的邏輯
                if(User.FindFirst(ClaimTypes.Role).Value.ToString() == "admin")//當Role為admin時，在console顯示Cookie的值，提供ProductApi使用
                {
                    //ViewBag.CookieValue = HttpContext.Request.Cookies["MyWebAppCookie"].ToString();
                    Console.WriteLine("admin：");
                    Console.WriteLine("MyWebAppCookie=" +  HttpContext.Request.Cookies["MyWebAppCookie"]);
                }
                else
                {
                    Console.WriteLine("member：");
                    Console.WriteLine("MyWebAppCookie=" + HttpContext.Request.Cookies["MyWebAppCookie"]);
                }
                return View();
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Member model)
        {
            Console.WriteLine("登入動作開始");
            if (/*ModelState.IsValid*/true)
            {
                var member = await _context.Member.FirstOrDefaultAsync(m => m.Email == model.Email && m.Password == model.Password);
                if (member != null)
                {
                    // 登入，設置使用者為已驗證狀態
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, member.Id.ToString()),
                        new Claim(ClaimTypes.Name, member.Name),
                        new Claim(ClaimTypes.Email, member.Email),
                        new Claim(ClaimTypes.Role, member.Role)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(principal);

                    Console.WriteLine("登入成功");
                    ViewBag.Message = "登入成功。";
                    return RedirectToAction("Index", "Members");
                }
                else
                {
                    ModelState.AddModelError("", "無效的Email或密碼。");
                    ViewBag.Message = "登入失敗";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("會員登出動作");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (TempData["Message"] is not null and not (object?)"")
            {
                var message = TempData["Message"] as string;
                ViewBag.Message = message;
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize]
        public async Task<IActionResult> Details()//會員個人資料
        {
            var member = await _context.Member.FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(User.FindFirst(ClaimTypes.Sid).Value));
            if (member == null)
            {
                Console.WriteLine("無此會員");
                return NotFound();
            }

            return View(member);
        }

        public IActionResult Register()//會員註冊頁面
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Name,Email,Password,Role")] Member member)//會員註冊
        {
            if (ModelState.IsValid)
            {
                //檢查email是否重複
                var checkEmail = MemberExists(member.Email);
                if (checkEmail)
                {
                    //Email重複
                    Console.WriteLine("創建失敗，此Email重複。");
                    ViewBag.Message = "此Email已有人使用，請使用其他Email";
                    //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    //ViewBag.Message = Encoding.GetEncoding("Big5").GetString(Encoding.UTF8.GetBytes(ViewBag.Message));
                    return View(member);
                }
                else
                {
                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("會員創建成功。");
                    //ViewBag.Message = "會員創建成功，回首頁";
                    TempData["Message"] = "會員創建成功，回首頁";
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                
            }
            Console.WriteLine("創建失敗，可能是錯誤格式輸入。請再試一次");
            return View(member);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)//會員更改資料頁面
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] Member member)//會員更改資料
        {
            if (id != member.Id)
            {
                return NotFound();
            }
            bool isSuccess = false;
            //if (ModelState.IsValid)
            //{
            try
            {
                var entity = _context.Member.Find(member.Id);
                var entry = _context.Entry(entity);
                entry.CurrentValues.SetValues(new { Name = member.Name, Password = member.Password });
                await _context.SaveChangesAsync();

                TempData["Message"] = "修改成功。請重新登入";
                isSuccess = true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(member.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (isSuccess)
            {
                return RedirectToAction(nameof(Logout));
            }
            
            //}
            //else
            //{
            //    ViewBag.Message = "格式有誤，請重新輸入。";
            //}
            return View(member);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Member == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Member == null)
            {
                return Problem("Entity set 'MyWebAppDbContext.Member'  is null.");
            }
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
          return (_context.Member?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool MemberExists(string email)
        {
            return (_context.Member?.Any(e => e.Email == email)).GetValueOrDefault();
        }
    }
}
