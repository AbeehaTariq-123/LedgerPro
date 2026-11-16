
using LedgerPro.Data;
using LedgerPro.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleDashboard();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            var signInResult = await _signInManager
                .CheckPasswordSignInAsync(
                    user,
                    model.Password,
                    lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is temporarily locked. Please try again later.");

                return View(model);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            // Verify that the user belongs to the selected account type.
            bool hasSelectedRole = await _userManager
                .IsInRoleAsync(user, model.AccountType);

            if (!hasSelectedRole)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This account does not have the selected account type.");

                return View(model);
            }

            await _signInManager.SignInAsync(
                user,
                model.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRoleDashboard();
        }

        // GET: /Account/Signup
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Signup()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleDashboard();
            }

            return View();
        }

        // POST: /Account/Signup
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Only people with the correct secret code can register as Admin.
            if (model.AccountType == "Admin")
            {
                string? correctAdminCode =
                    _configuration["AdminRegistrationCode"];

                if (string.IsNullOrWhiteSpace(correctAdminCode) ||
                    string.IsNullOrWhiteSpace(model.AdminRegistrationCode) ||
                    model.AdminRegistrationCode != correctAdminCode)
                {
                    ModelState.AddModelError(
                        nameof(model.AdminRegistrationCode),
                        "The Admin registration code is incorrect.");

                    return View(model);
                }
            }

            // Check whether this email is already registered.
            var existingUser = await _userManager
                .FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            // Create the user account.
            var createResult = await _userManager
                .CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            // Assign the selected Identity role: Admin or Staff.
            var roleResult = await _userManager
                .AddToRoleAsync(user, model.AccountType);

            if (!roleResult.Succeeded)
            {
                // Delete the account if role assignment fails.
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            // Sign in the newly registered user.
            await _signInManager.SignInAsync(
                user,
                isPersistent: false);

            return RedirectToRoleDashboard();
        }

        // POST: /Account/Logout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/AccessDenied
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Redirect users to the dashboard matching their Identity role.
        private IActionResult RedirectToRoleDashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(
                    "Admin",
                    "Dashboard");
            }

            if (User.IsInRole("Staff"))
            {
                return RedirectToAction(
                    "Staff",
                    "Dashboard");
            }

            return RedirectToAction(nameof(AccessDenied));
        }
    }
}
