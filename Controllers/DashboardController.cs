using LedgerPro.Controllers.Helpers;
using LedgerPro.Data;
using LedgerPro.Models;
using LedgerPro.Models.ViewModels;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers;

[Authorize]
public class DashboardController(
    UserManager<ApplicationUser> userManager,
    IDashboardRepository dashboard,
    ICustomerRepository customers,
    ISupplierRepository suppliers,
    IProductRepository products,
    ITransactionRepository transactions) : Controller
{
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin()
    {
        var user = await userManager.GetUserAsync(User);

        var summary = await dashboard.GetSummaryAsync(new DateTime(2000, 1, 1), DateTime.Today);
        var customerList = (await customers.GetAllAsync()).ToList();
        var supplierList = (await suppliers.GetAllAsync()).ToList();
        var txs = (await transactions.GetAllAsync()).ToList();
        var names = await PartyLookup.LoadAsync(customers, suppliers);

        var recent = txs.Take(8).ToList();

        var model = new AdminDashboardViewModel
        {
            UserName = user?.FullName ?? user?.Email ?? "Admin",

            TotalSales = summary.Sales,
            TotalPurchases = summary.Purchases,
            TotalIncome = summary.PaymentsReceived,
            TotalExpenses = summary.PaymentsPaid,

            OutstandingReceivables = summary.CustomerOutstanding,
            OutstandingPayables = summary.SupplierOutstanding,

            TotalCustomers = customerList.Count,
            TotalSuppliers = supplierList.Count,

            RecentTransactions = recent.Select(t => new RecentTransaction
            {
                Id = t.TransactionId,
                Date = t.TransactionDate,
                Party = names.Name(t.CustomerId, t.SupplierId),
                Type = t.TransactionType,
                Amount = t.Amount,
                Status = StatusFor(t.TransactionType)
            }).ToList(),

            TopCustomers = CustomerBalances(customerList, txs)
                .OrderByDescending(c => c.Balance)
                .Take(5)
                .ToList(),

            RecentActivities = recent.Take(5)
                .Select(t => $"{t.TransactionType} of PKR {t.Amount:N0} — {names.Name(t.CustomerId, t.SupplierId)}")
                .ToList()
        };

        return View(model);
    }

    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> Staff()
    {
        var user = await userManager.GetUserAsync(User);

        var today = DateTime.Today;
        var summary = await dashboard.GetSummaryAsync(today, today);
        var customerList = (await customers.GetAllAsync()).ToList();
        var txs = (await transactions.GetAllAsync()).ToList();
        var names = await PartyLookup.LoadAsync(customers, suppliers);

        var todays = txs.Where(t => t.TransactionDate.Date == today).ToList();

        var model = new StaffDashboardViewModel
        {
            UserName = user?.FullName ?? user?.Email ?? "Staff",

            TodaySales = summary.Sales,
            TodayPurchases = summary.Purchases,
            TodayPaymentsReceived = summary.PaymentsReceived,

            TransactionsCount = todays.Count,
            // customers who currently owe money
            PendingPayments = CustomerBalances(customerList, txs).Count(c => c.Balance > 0),

            TodaysTransactions = todays.Select(t => new StaffTransaction
            {
                Id = t.TransactionId,
                Time = t.TransactionDate.ToString("dd MMM"),
                Party = names.Name(t.CustomerId, t.SupplierId),
                Type = t.TransactionType,
                Amount = t.Amount,
                Status = StatusFor(t.TransactionType)
            }).ToList(),

            LowStockProducts = (await products.GetAllAsync())
                .Where(p => p.StockQuantity <= p.ReorderLevel)
                .OrderBy(p => p.StockQuantity)
                .Take(8)
                .Select(p => new LowStockProduct
                {
                    Name = p.ProductName,
                    Stock = (int)p.StockQuantity,
                    MinStock = (int)p.ReorderLevel
                })
                .ToList(),

            RecentActivities = txs.Take(5)
                .Select(t => $"{t.TransactionType} of PKR {t.Amount:N0} — {names.Name(t.CustomerId, t.SupplierId)}")
                .ToList()
        };

        return View(model);
    }

    // Payments settle an account; sales/purchases are on account ("Credit").
    private static string StatusFor(string type) => type switch
    {
        "Received" or "Paid" or "Payment" or "Receipt" => "Paid",
        "Sale" or "Purchase" or "Credit" => "Credit",
        _ => "Pending"
    };

    // Same arithmetic as LedgerRepository.GetCustomerBalanceAsync, done in memory for the dashboard.
    private static List<TopCustomer> CustomerBalances(List<Customer> customerList, List<Transaction> txs)
    {
        var byCustomer = txs
            .Where(t => t.CustomerId.HasValue)
            .GroupBy(t => t.CustomerId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        return customerList.Select(c =>
        {
            byCustomer.TryGetValue(c.CustomerId, out var list);
            list ??= new List<Transaction>();

            decimal balance = c.OpeningBalance + list.Sum(t => t.TransactionType switch
            {
                "Sale" or "Credit" or "Debit" => t.Amount,
                "Received" or "Payment" or "Receipt" => -t.Amount,
                _ => 0m
            });

            return new TopCustomer
            {
                Name = c.Name,
                Balance = balance,
                Orders = list.Count(t => t.TransactionType == "Sale")
            };
        }).ToList();
    }
}
