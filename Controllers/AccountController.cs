using LedgerPro.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers
{
    public class AccountController : Controller
    {
        // ===== MOCK USERS (Static) =====
        private static readonly Dictionary<string, (string Password, string Name, string Role)> MockUsers = new()
        {
            { "admin@ledgerpro.com", ("admin123", "Abecha Tariq", "Admin") },
            { "staff@ledgerpro.com", ("staff123", "Sara Khan", "Staff") }
        };

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (MockUsers.TryGetValue(model.Email.ToLower(), out var user) && user.Password == model.Password)
            {
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role);

                return user.Role == "Admin"
                    ? RedirectToAction("Admin", "Dashboard")
                    : RedirectToAction("Staff", "Dashboard");
            }

            ViewBag.Error = "Invalid email or password. Try admin@ledgerpro.com / admin123";
            return View(model);
        }

        [HttpGet]
        public IActionResult Signup() => View(new SignupViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (MockUsers.ContainsKey(model.Email.ToLower()))
            {
                ViewBag.Error = "This email is already registered.";
                return View(model);
            }

            // Mock registration - just simulate success
            MockUsers[model.Email.ToLower()] = (model.Password, model.FullName, model.Role);
            TempData["Success"] = "Account created! Please sign in.";
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            ViewBag.Info = "Password reset link sent to your email (demo only).";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}