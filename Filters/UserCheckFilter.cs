using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Filters
{
    public class UserCheckFilter : IActionFilter
    {
        private readonly AppDbContext _context;

        public UserCheckFilter(AppDbContext context)
        {
            _context = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var userId = http.Session.GetInt32("UserId");

            // IMPORTANT: allow only Auth controller without checks
            var controller = context.RouteData.Values["controller"]?.ToString();
            if (controller == "Auth")
                return;

            // IMPORTANT: if no session need to redirect to login
            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // IMPORTANT: check if user exists
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                http.Session.Clear();
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // IMPORTANT: blocked users cannot continue
            if (user.Status == User.UserStatus.Blocked)
            {
                http.Session.Clear();
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
