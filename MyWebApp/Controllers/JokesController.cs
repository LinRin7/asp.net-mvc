using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class JokesController : Controller
    {
        private readonly MyWebAppDbContext _context;

        public JokesController(MyWebAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Joke> resultList = new List<Joke>();
            
            if (User.Identity.IsAuthenticated)
            {
                resultList = await _context.Joke
                    .Where(i => i.Owner == User.FindFirst(ClaimTypes.Email).Value.ToString())
                    .OrderBy(j =>j.Id)
                    .ToListAsync();//先撈取自己所創建的
                var tempList = await _context.Joke
                    .Where(i => i.Owner != Convert.ToString(User.FindFirst(ClaimTypes.Email).Value))
                    .OrderBy(j => j.Id)
                    .ToListAsync();
                resultList = resultList.Concat(tempList).ToList();
            }
            else
            {
                resultList = await _context.Joke.ToListAsync();
            }

            if(TempData["Message"] is not null and not (object?)"")
            {
                var message = TempData["Message"] as string;
                ViewBag.Message = message;
            }
            
            return _context.Joke != null ? 
                View(resultList) :
                Problem("Entity set 'MyWebAppDbContext.Joke'  is null.");
        }

        public IActionResult ShowSearchForm()
        {
            return View();
                        
        }

        [HttpPost]
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)//搜尋笑話
        {
            Console.WriteLine("搜尋字串為：" + SearchPhrase);
            ViewBag.SearchPhrase = SearchPhrase;
            return View("Index", await _context.Joke.Where(j => j.JokeQuestion.Contains(SearchPhrase)).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.Id == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        [Authorize]
        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                Console.WriteLine("前往創建JOKE畫面");
                return View();
            }
            else
            {
                Console.WriteLine("需先會員登入才能創建JOKE");
                return RedirectToAction(nameof(MembersController.Login), "Members");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Id,JokeQuestion,JokeAnswer")]*/ Joke joke)
        {
            //joke.Owner = User.Identity.Name.ToString();
            joke.Owner = User.FindFirst(ClaimTypes.Email).Value.ToString();
            var isSuccess = false;
            //if (ModelState.IsValid)
            //{
            //    _context.Add(joke);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            try
            {
                _context.Add(joke);
                await _context.SaveChangesAsync();
                isSuccess = true;
                //ViewBag.Message = "創建成功！";
                TempData["Message"] = "創建成功！";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {

            }
            return isSuccess == true ? RedirectToAction(nameof(Index)) : View(joke);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke.FindAsync(id);
            if (joke == null)
            {
                return NotFound();
            }
            return View(joke);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JokeQuestion,JokeAnswer")] Joke joke)
        {
            if (id != joke.Id)
            {
                return NotFound();
            }

            bool isSuccess = false;

            try
            {
                var entity = _context.Joke.Find(joke.Id);
                var entry = _context.Entry(entity);
                entry.CurrentValues.SetValues(new { JokeQuestion = joke.JokeQuestion, JokeAnswer = joke.JokeAnswer });
                await _context.SaveChangesAsync();
                isSuccess = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                //isSuccess = false;
            }
            finally
            {

            }
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(joke);
            //        await _context.SaveChangesAsync();
            //        //ViewBag.Message = "修改成功！";
            //        TempData["Message"] = "修改成功！";
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!JokeExists(joke.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.Id == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Joke == null)
            {
                return Problem("Entity set 'MyWebAppDbContext.Joke'  is null.");
            }
            var joke = await _context.Joke.FindAsync(id);
            if (joke != null)
            {
                _context.Joke.Remove(joke);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JokeExists(int id)
        {
          return (_context.Joke?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
