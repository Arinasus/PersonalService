using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;
using WebApplication2.Services;
using static WebApplication2.Models.User;

namespace WebApplication2.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _email;
        public AuthController(AppDbContext context, EmailService email)
        {
            _context = context;
            _email = email;
        }

        //GET
        public IActionResult Register()
        {
            return View();
        }
        //GET
        public IActionResult Login()
        {
            return View();
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            var token = GetUniqIdValue();
            var user = new User
            {
                Name = name,
                Email = email,
                Password = password,
                Status = UserStatus.Unverified,
                RegisteredAt = DateTime.UtcNow,
                ConfirmToken = token
            };
            if (password.Length < 6 || !password.Any(char.IsDigit) || !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ViewBag.Error = "Пароль должен содержать минимум 6 символов, хотя бы одну цифру и один специальный символ.";
                return View();
            }

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            catch
            {
                ViewBag.Error = "Такой Email уже существует.";
                return View();
            }
            var confirmLink = Url.Action("ConfirmEmail", "Auth", new { token = user.ConfirmToken }, Request.Scheme); 
            var body = $"<h2>Здравствуйте, {user.Name}!</h2><p>Для подтверждения регистрации перейдите по ссылке: <a href='{confirmLink}'>Подтвердить Email</a></p>"; 
            await _email.SendEmailAsync(user.Email, "Подтверждение регистрации", body);
            TempData["Success"] = "Регистрация успешна! Теперь войдите в систему."; 
            return RedirectToAction("Login");
        }

        //POST
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if(user == null)
            {
                ViewBag.Error = "Неверный email или пароль";
                return View();
            }
            if(user.Password != password)
            {
                ViewBag.Error = "Неверный email или пароль.";
                return View();
            }
            if(user.Status == WebApplication2.Models.User.UserStatus.Blocked)
            {
                ViewBag.Error = "Ваш аккаунт заблокирован";
                return View();
            }
            user.LastLoginAt = DateTime.UtcNow;
            user.LastActivityAt = DateTime.UtcNow;
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Index", "Users");
        }

        //GET
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        private string GetUniqIdValue()
        {
            var random = Random.Shared.NextInt64().ToString("x");
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x");
            return $"{time}-{random}";
        }
        public IActionResult ConfirmEmail(string token) { if (string.IsNullOrEmpty(token)) { 
                TempData["Error"] = "Некорректная ссылка подтверждения."; 
                return RedirectToAction("Login"); } 
            var user = _context.Users.FirstOrDefault(u => u.ConfirmToken == token); 
            if (user == null) { TempData["Error"] = "Токен подтверждения недействителен."; 
                return RedirectToAction("Login"); } 
            user.Status = UserStatus.Active; 
            user.ConfirmToken = null; 
            _context.SaveChanges(); 
            TempData["Success"] = "Email успешно подтверждён! Теперь вы можете войти."; 
            return RedirectToAction("Login"); 
        }

    }
}
