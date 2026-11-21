using LedgerPro.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LedgerPro.Areas.Identity.Pages.Account;

public class LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger) : PageModel
{
    // Visiting /Identity/Account/Logout directly shows a confirmation (or bounces to Login if signed out).
    public IActionResult OnGet() =>
        User.Identity?.IsAuthenticated == true ? Page() : RedirectToPage("./Login");

    // The sidebar "Logout" button posts here.
    public async Task<IActionResult> OnPostAsync()
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("User logged out.");
        return RedirectToPage("./Login");
    }
}
