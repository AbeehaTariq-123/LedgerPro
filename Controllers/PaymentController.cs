using LedgerPro.Controllers.Helpers;
using LedgerPro.Models;
using LedgerPro.Models.ViewModels;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LedgerPro.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class PaymentController(
    IPaymentRepository payments,
    ICustomerRepository customers,
    ISupplierRepository suppliers) : Controller
{
    // GET /Payment
    public IActionResult Index() => View();

    // GET /Payment/GetAll  (called by wwwroot/js/payments.js)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var names = await PartyLookup.LoadAsync(customers, suppliers);

        var rows = (await payments.GetAllAsync()).Select(p => new
        {
            p.PaymentId,
            p.PaymentType,
            PartyName = names.Name(p.CustomerId, p.SupplierId),
            p.Amount,
            p.PaymentDate,
            p.Description
        });

        return Json(rows);
    }

    // GET /Payment/Create?type=Received|Paid&partyId=5
    [HttpGet]
    public async Task<IActionResult> Create(string? type, int? partyId)
    {
        var model = new PaymentFormViewModel
        {
            PaymentType = NormalizeType(type),
            PartyId = partyId
        };

        await FillPartiesAsync(model);
        return View(model);
    }

    // POST /Payment/Create
    // PaymentRepository.AddAsync writes the Payments row AND the matching Transactions row in one SQL transaction,
    // so ledgers and the dashboard stay in sync.
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentFormViewModel model)
    {
        model.PaymentType = NormalizeType(model.PaymentType);

        if (!ModelState.IsValid)
        {
            await FillPartiesAsync(model);
            return View(model);
        }

        bool received = model.PaymentType == "Received";

        await payments.AddAsync(new Payment
        {
            CustomerId = received ? model.PartyId : null,
            SupplierId = received ? null : model.PartyId,
            PaymentType = model.PaymentType,
            Amount = model.Amount!.Value,
            PaymentDate = model.PaymentDate!.Value,
            Description = model.Description
        });

        TempData["Success"] = received
            ? $"Payment of PKR {model.Amount:N0} received."
            : $"Payment of PKR {model.Amount:N0} recorded as paid.";

        return RedirectToAction(nameof(Index));
    }

    private static string NormalizeType(string? type) =>
        string.Equals(type, "Paid", StringComparison.OrdinalIgnoreCase) ? "Paid" : "Received";

    private async Task FillPartiesAsync(PaymentFormViewModel model)
    {
        model.Parties = model.PaymentType == "Received"
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
