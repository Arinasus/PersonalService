using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        // GET
        public IActionResult Index(string sort = "lastLogin", string dir = "desc", string search = "", string status = "")
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
                return RedirectToAction("Login", "Auth");

            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                users = users.Where(u =>
                    u.Name.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s));
            }

            if (!string.IsNullOrEmpty(status) &&
                Enum.TryParse<User.UserStatus>(status, out var st))
            {
                users = users.Where(u => u.Status == st);
            }

            bool asc = dir == "asc";

            users = sort switch
            {
                "name" => asc ? users.OrderBy(u => u.Name) : users.OrderByDescending(u => u.Name),
                "email" => asc ? users.OrderBy(u => u.Email) : users.OrderByDescending(u => u.Email),
                "status" => asc ? users.OrderBy(u => u.Status) : users.OrderByDescending(u => u.Status),
                _ => asc ? users.OrderBy(u => u.LastLoginAt) : users.OrderByDescending(u => u.LastLoginAt)
            };

            ViewBag.Sort = sort;
            ViewBag.Dir = dir;
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(users.ToList());
        }


        //POST
        [HttpPost]
        public IActionResult Block(int[] ids)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var users = _context.Users.Where(u => ids.Contains(u.Id)).ToList();
            foreach (var u in users)
                u.Status = WebApplication2.Models.User.UserStatus.Blocked;
            _context.SaveChanges();
            if (ids.Contains(currentUserId.Value)) 
            { 
                HttpContext.Session.Clear(); 
                return RedirectToAction("Login", "Auth"); 
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Unblock(int[] ids)
        {
            var users = _context.Users.Where(u => ids.Contains(u.Id)).ToList();
            foreach (var u in users)
                u.Status = WebApplication2.Models.User.UserStatus.Active;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int[] ids)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var users = _context.Users.Where(u => ids.Contains(u.Id)).ToList();
            _context.Users.RemoveRange(users);
            _context.SaveChanges();
            if (ids.Contains(currentUserId.Value)) 
            { 
                HttpContext.Session.Clear(); 
                return RedirectToAction("Login", "Auth"); 
            }
            return RedirectToAction("Index");
        }
        // POST
        [HttpPost] public IActionResult DeleteUnverified() 
        { 
            var users = _context.Users 
                .Where(u => u.Status == WebApplication2.Models.User.UserStatus.Unverified) .ToList(); 
            _context.Users.RemoveRange(users); 
            _context.SaveChanges(); 
            return RedirectToAction("Index"); 
        }
    }
}
