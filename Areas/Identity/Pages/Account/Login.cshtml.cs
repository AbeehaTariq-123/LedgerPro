using System.ComponentModel.DataAnnotations;
using LedgerPro.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LedgerPro.Areas.Identity.Pages.Account;

public class LoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<LoginModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please select an account type")]
        [RegularExpression("^(Admin|Staff)$", ErrorMessage = "Select Admin or Staff")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; } = "Staff";

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        // Already signed in? Go straight to the matching dashboard.
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(DashboardUrl(User.IsInRole("Admin") ? "Admin" : "Staff"));

        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);

        // Clear any leftover external cookie for a clean login.
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid) return Page();

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user is null) return Fail("Invalid email or password.");

        var result = await signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }

        if (!result.Succeeded) return Fail("Invalid email or password.");

        // The selected account type must match one of the user's Identity roles.
        if (!await userManager.IsInRoleAsync(user, Input.AccountType))
            return Fail("This account does not have the selected account type.");

        await signInManager.SignInAsync(user, Input.RememberMe);
        logger.LogInformation("User logged in as {Role}.", Input.AccountType);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return LocalRedirect(DashboardUrl(Input.AccountType));
    }

    private IActionResult Fail(string message)
    {
        ModelState.AddModelError(string.Empty, message);
        return Page();
    }

    private static string DashboardUrl(string role) =>
        role == "Admin" ? "/Dashboard/Admin" : "/Dashboard/Staff";
}
