
using LedgerPro.Data;
using LedgerPro.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new AdminDashboardViewModel
        {
            UserName = user?.FullName ?? user?.Email ?? "Admin",

            TotalSales = 0,
            TotalPurchases = 0,
            TotalIncome = 0,
            TotalExpenses = 0,

            OutstandingReceivables = 0,
            OutstandingPayables = 0,

            TotalCustomers = 0,
            TotalSuppliers = 0,

            RecentTransactions = new(),
            TopCustomers = new(),
            RecentActivities = new()
        };

        return View(model);
    }

    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> Staff()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new StaffDashboardViewModel
        {
            UserName = user?.FullName ?? user?.Email ?? "Staff",

            TodaySales = 0,
            TodayPurchases = 0,
            TodayPaymentsReceived = 0,

            TransactionsCount = 0,
            PendingPayments = 0,

            TodaysTransactions = new(),
            LowStockProducts = new(),
            RecentActivities = new()
        };

        return View(model);
    }
}
