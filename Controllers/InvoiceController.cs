using LedgerPro.Models.ViewModels;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers;

/// <summary>
/// Invoices are built from "Sale" rows in the Transactions table (the schema has no line-item tables yet).
/// </summary>
[Authorize(Roles = "Admin,Staff")]
public class InvoiceController(
    ITransactionRepository transactions,
    ICustomerRepository customers,
    ILedgerRepository ledger) : Controller
{
    // GET /Invoice
    public async Task<IActionResult> Index()
    {
        var names = (await customers.GetAllAsync()).ToDictionary(c => c.CustomerId, c => c.Name);

        var rows = (await transactions.GetAllAsync())
            .Where(t => t.TransactionType == "Sale" && t.CustomerId.HasValue)
            .Select(t => new InvoiceRowViewModel
            {
                TransactionId = t.TransactionId,
                Date = t.TransactionDate,
                CustomerName = names.TryGetValue(t.CustomerId!.Value, out var n) ? n : "—",
                Amount = t.Amount
            })
            .ToList();

        return View(rows);
    }

    // GET /Invoice/Print/12
    public async Task<IActionResult> Print(int id)
    {
        var sale = await transactions.GetByIdAsync(id);
        if (sale is null || sale.TransactionType != "Sale" || sale.CustomerId is null) return NotFound();

        var customer = await customers.GetByIdAsync(sale.CustomerId.Value);
        if (customer is null) return NotFound();

        var model = new InvoiceViewModel
        {
            TransactionId = sale.TransactionId,
            Date = sale.TransactionDate,
            CustomerName = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            Description = sale.Description,
            Amount = sale.Amount,
            CustomerBalance = await ledger.GetCustomerBalanceAsync(customer.CustomerId)
        };

        return View(model);
    }
}
