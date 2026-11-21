using LedgerPro.Controllers.Helpers;
using LedgerPro.Models;
using LedgerPro.Models.ViewModels;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LedgerPro.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class TransactionController(
    ITransactionRepository transactions,
    ICustomerRepository customers,
    ISupplierRepository suppliers) : Controller
{
    // GET /Transaction
    public IActionResult Index() => View();

    // GET /Transaction/GetAll  (called by wwwroot/js/transactions.js)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var names = await PartyLookup.LoadAsync(customers, suppliers);

        var rows = (await transactions.GetAllAsync()).Select(t => new
        {
            t.TransactionId,
            t.TransactionType,
            PartyName = names.Name(t.CustomerId, t.SupplierId),
            t.Amount,
            t.TransactionDate,
            t.Description
        });

        return Json(rows);
    }

    // GET /Transaction/Create?type=Sale|Purchase&partyId=5
    [HttpGet]
    public async Task<IActionResult> Create(string? type, int? partyId)
    {
        var model = new TransactionFormViewModel
        {
            TransactionType = NormalizeType(type),
            PartyId = partyId
        };

        await FillPartiesAsync(model);
        return View(model);
    }

    // POST /Transaction/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransactionFormViewModel model)
    {
        model.TransactionType = NormalizeType(model.TransactionType);

        if (!ModelState.IsValid)
        {
            await FillPartiesAsync(model);
            return View(model);
        }

        bool isSale = model.TransactionType == "Sale";

        await transactions.AddAsync(new Transaction
        {
            CustomerId = isSale ? model.PartyId : null,
            SupplierId = isSale ? null : model.PartyId,
            TransactionType = model.TransactionType,
            Amount = model.Amount!.Value,
            TransactionDate = model.TransactionDate!.Value,
            Description = model.Description
        });

        TempData["Success"] = $"{model.TransactionType} of PKR {model.Amount:N0} recorded.";
        return RedirectToAction(nameof(Index));
    }

    private static string NormalizeType(string? type) =>
        string.Equals(type, "Purchase", StringComparison.OrdinalIgnoreCase) ? "Purchase" : "Sale";

    private async Task FillPartiesAsync(TransactionFormViewModel model)
    {
        model.Parties = model.TransactionType == "Sale"
            ? (await customers.GetAllAsync())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.CustomerId.ToString()))
                .ToList()
            : (await suppliers.GetAllAsync())
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem(s.Name, s.SupplierId.ToString()))
                .ToList();
    }
}
