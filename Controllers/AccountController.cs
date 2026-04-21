using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Data;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Введите логин и пароль";
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.IsActive);

            if (user == null)
            {
                ViewBag.Error = "Неверный логин или пароль";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            try
            {
                _context.LogEntries.Add(new LogEntry
                {
                    UserId = user.Id,
                    Action = "Вход в систему",
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Timestamp = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
            catch { /* игнорируем ошибку логирования */ }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                try
                {
                    _context.LogEntries.Add(new LogEntry
                    {
                        UserId = userId.Value,
                        Action = "Выход из системы",
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Timestamp = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                }
                catch { /* игнорируем ошибку логирования */ }
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}