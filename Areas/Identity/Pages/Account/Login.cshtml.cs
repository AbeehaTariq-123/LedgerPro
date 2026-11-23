using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using LedgerPro.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LedgerPro.Areas.Identity.Pages.Account;

public class LoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
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

        [DataType(DataType.Password)]
        [Display(Name = "Admin Registration Code")]
        public string? AdminRegistrationCode { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(DashboardUrl(User.IsInRole("Admin") ? "Admin" : "Staff"));

        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        // Admin sign-in also needs the Admin registration code.
        if (Input.AccountType == "Admin" && string.IsNullOrWhiteSpace(Input.AdminRegistrationCode))
            ModelState.AddModelError("Input.AdminRegistrationCode", "Admin registration code is required.");

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

        if (!await userManager.IsInRoleAsync(user, Input.AccountType))
            return Fail("This account does not have the selected account type.");

        if (Input.AccountType == "Admin" && !AdminCodeIsCorrect(Input.AdminRegistrationCode))
        {
            // A wrong code counts as a failed attempt so it can't be guessed without limit.
            await userManager.AccessFailedAsync(user);
            if (await userManager.IsLockedOutAsync(user)) return RedirectToPage("./Lockout");

            ModelState.AddModelError("Input.AdminRegistrationCode", "The Admin registration code is incorrect.");
            return Page();
        }

        await signInManager.SignInAsync(user, Input.RememberMe);
        logger.LogInformation("User logged in as {Role}.", Input.AccountType);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return LocalRedirect(DashboardUrl(Input.AccountType));
    }

    private bool AdminCodeIsCorrect(string? supplied)
    {
        var expected = configuration["AdminRegistrationCode"];
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(supplied)) return false;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(supplied),
            Encoding.UTF8.GetBytes(expected));
    }

    private IActionResult Fail(string message)
    {
        ModelState.AddModelError(string.Empty, message);
        return Page();
    }

    private static string DashboardUrl(string role) =>
        role == "Admin" ? "/Dashboard/Admin" : "/Dashboard/Staff";
}
