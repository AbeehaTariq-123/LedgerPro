using LedgerPro.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers
{
    public class DashboardController : Controller
    {
        private bool IsLoggedIn() => HttpContext.Session.GetString("UserEmail") != null;

        public IActionResult Admin()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Staff");

            var vm = new AdminDashboardViewModel
            {
                UserName = HttpContext.Session.GetString("UserName") ?? "Admin",
                TotalSales = 485320m,
                TotalPurchases = 312750m,
                TotalIncome = 172570m,
                TotalExpenses = 98240m,
                OutstandingReceivables = 63400m,
                OutstandingPayables = 41200m,
                TotalCustomers = 128,
                TotalSuppliers = 24,
                RecentTransactions = new()
                {
                    new() { Id = 1, Date = DateTime.Today, Party = "Ali Traders", Type = "Sale", Amount = 12500m, Status = "Paid" },
                    new() { Id = 2, Date = DateTime.Today, Party = "Karachi Supplies", Type = "Purchase", Amount = 8400m, Status = "Pending" },
                    new() { Id = 3, Date = DateTime.Today.AddDays(-1), Party = "Hassan Store", Type = "Sale", Amount = 3200m, Status = "Credit" },
                    new() { Id = 4, Date = DateTime.Today.AddDays(-1), Party = "Bilal Wholesale", Type = "Payment", Amount = 5000m, Status = "Paid" },
                    new() { Id = 5, Date = DateTime.Today.AddDays(-2), Party = "Metro Mart", Type = "Sale", Amount = 9700m, Status = "Overdue" },
                },
                TopCustomers = new()
                {
                    new() { Name = "Ali Traders", Balance = 18400m, Orders = 42 },
                    new() { Name = "Hassan Store", Balance = 12800m, Orders = 35 },
                    new() { Name = "Metro Mart", Balance = 9600m, Orders = 28 },
                },
                RecentActivities = new()
                {
                    "Invoice #INV-1042 generated for Ali Traders",
                    "Payment of PKR 5,000 received from Bilal Wholesale",
                    "New supplier 'Karachi Supplies' added",
                    "Staff account created for Sara Khan",
                    "Monthly report for October exported",
                }
            };
            return View(vm);
        }

        public IActionResult Staff()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("UserRole") != "Staff") return RedirectToAction("Admin");

            var vm = new StaffDashboardViewModel
            {
                UserName = HttpContext.Session.GetString("UserName") ?? "Staff",
                TodaySales = 18400m,
                TodayPurchases = 6200m,
                TodayPaymentsReceived = 9500m,
                TransactionsCount = 14,
                PendingPayments = 3,
                TodaysTransactions = new()
                {
                    new() { Id = 1, Time = "09:15", Party = "Ali Traders", Type = "Sale", Amount = 4200m, Status = "Paid" },
                    new() { Id = 2, Time = "10:40", Party = "Hassan Store", Type = "Sale", Amount = 3100m, Status = "Credit" },
                    new() { Id = 3, Time = "11:20", Party = "Bilal Wholesale", Type = "Payment", Amount = 5000m, Status = "Paid" },
                    new() { Id = 4, Time = "13:05", Party = "Metro Mart", Type = "Sale", Amount = 2600m, Status = "Pending" },
                },
                LowStockProducts = new()
                {
                    new() { Name = "Cotton Fabric Roll", Stock = 3, MinStock = 10 },
                    new() { Name = "Ceramic Mug Set", Stock = 1, MinStock = 5 },
                    new() { Name = "Handmade Notebook", Stock = 4, MinStock = 8 },
                },
                RecentActivities = new()
                {
                    "Sale recorded for Ali Traders (PKR 4,200)",
                    "Customer payment of PKR 5,000 added",
                    "New customer 'Zara Boutique' registered",
                    "Stock check completed - 3 items low",
                }
            };
            return View(vm);
        }
    }
}