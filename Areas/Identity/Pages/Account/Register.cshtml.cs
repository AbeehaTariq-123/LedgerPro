using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using LedgerPro.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LedgerPro.Areas.Identity.Pages.Account;

public class RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration,
    ILogger<RegisterModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Please select an account type")]
        [RegularExpression("^(Admin|Staff)$", ErrorMessage = "Select Admin or Staff")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; } = "Staff";

        [DataType(DataType.Password)]
        [Display(Name = "Admin Registration Code")]
        public string? AdminRegistrationCode { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = "";
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(User.IsInRole("Admin") ? "/Dashboard/Admin" : "/Dashboard/Staff");

        ReturnUrl = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid) return Page();

        // Only people who know the secret code can register as Admin.
        if (Input.AccountType == "Admin" && !AdminCodeIsCorrect(Input.AdminRegistrationCode))
        {
            ModelState.AddModelError("Input.AdminRegistrationCode", "The Admin registration code is incorrect.");
            return Page();
        }

        if (await userManager.FindByEmailAsync(Input.Email) is not null)
        {
            ModelState.AddModelError("Input.Email", "An account with this email already exists.");
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            EmailConfirmed = true
        };

        var create = await userManager.CreateAsync(user, Input.Password);
        if (!create.Succeeded)
        {
            foreach (var e in create.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        var addRole = await userManager.AddToRoleAsync(user, Input.AccountType);
        if (!addRole.Succeeded)
        {
            await userManager.DeleteAsync(user); // don't leave a role-less account behind
            foreach (var e in addRole.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        logger.LogInformation("New {Role} account created.", Input.AccountType);

        await signInManager.SignInAsync(user, isPersistent: false);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return LocalRedirect(Input.AccountType == "Admin" ? "/Dashboard/Admin" : "/Dashboard/Staff");
    }

    private bool AdminCodeIsCorrect(string? supplied)
    {
        var expected = configuration["AdminRegistrationCode"];
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(supplied)) return false;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(supplied),
            Encoding.UTF8.GetBytes(expected));
    }
}
