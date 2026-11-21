using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LedgerPro.Controllers;

[Authorize]
public class LedgerController(ILedgerRepository ledger, ICustomerRepository customers, ISupplierRepository suppliers) : Controller
{
    public async Task<IActionResult> Index() { ViewBag.Customers = await customers.GetAllAsync(); ViewBag.Suppliers = await suppliers.GetAllAsync(); return View(); }
    public async Task<IActionResult> Customer(int id) { var c = await customers.GetByIdAsync(id); if (c is null) return NotFound(); ViewBag.PartyName = c.Name; ViewBag.PartyType = "Customer"; ViewBag.Balance = await ledger.GetCustomerBalanceAsync(id); return View("Statement", await ledger.GetCustomerLedgerAsync(id)); }
    public async Task<IActionResult> Supplier(int id) { var s = await suppliers.GetByIdAsync(id); if (s is null) return NotFound(); ViewBag.PartyName = s.Name; ViewBag.PartyType = "Supplier"; ViewBag.Balance = await ledger.GetSupplierBalanceAsync(id); return View("Statement", await ledger.GetSupplierLedgerAsync(id)); }
}
