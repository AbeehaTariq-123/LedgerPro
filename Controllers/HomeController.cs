using System.Diagnostics;
using LedgerPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    // GET /  -> signed-in users go to their dashboard, everyone else to the Identity login page.
    public IActionResult Index()
    {
        if (User.IsInRole("Admin")) return RedirectToAction("Admin", "Dashboard");
        if (User.IsInRole("Staff")) return RedirectToAction("Staff", "Dashboard");

        return RedirectToPage("/Account/Login", new { area = "Identity" });
    }

    // GET /Home/Error  (used by UseExceptionHandler outside Development)
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
