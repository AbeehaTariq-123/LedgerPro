using LedgerPro.Models;
using LedgerPro.Models.ViewModels;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LedgerPro.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class SupplierController(ISupplierRepository suppliers, ILedgerRepository ledger) : Controller
{
    // GET /Supplier
    public IActionResult Index() => View();

    // GET /Supplier/GetAll  (called by wwwroot/js/suppliers.js)
    [HttpGet]
    public async Task<IActionResult> GetAll() => Json(await suppliers.GetAllAsync());

    // GET /Supplier/Create
    [HttpGet]
    public IActionResult Create() => View("Form", new Supplier());

    // GET /Supplier/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await suppliers.GetByIdAsync(id);
        return supplier is null ? NotFound() : View("Form", supplier);
    }

    // POST /Supplier/Save  (add when SupplierId == 0, otherwise update)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Supplier model)
    {
        if (!ModelState.IsValid) return View("Form", model);

        if (model.SupplierId == 0)
        {
            await suppliers.AddAsync(model);
            TempData["Success"] = $"Supplier “{model.Name}” added.";
        }
        else
        {
            if (!await suppliers.UpdateAsync(model)) return NotFound();
            TempData["Success"] = $"Supplier “{model.Name}” updated.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /Supplier/Delete  (called by suppliers.js)
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            return await suppliers.DeleteAsync(id) ? Ok() : NotFound("Supplier not found.");
        }
        catch (SqlException ex) when (ex.Number == 547) // foreign key violation
        {
            return Conflict("This supplier has transactions or payments, so it can't be deleted.");
        }
    }

    // GET /Supplier/Ledger/5
    [HttpGet]
    public async Task<IActionResult> Ledger(int id)
    {
        var supplier = await suppliers.GetByIdAsync(id);
        if (supplier is null) return NotFound();

        var model = new LedgerViewModel
        {
            PartyId = id,
            PartyName = supplier.Name,
            IsCustomer = false,
            OpeningBalance = supplier.OpeningBalance,
            Balance = await ledger.GetSupplierBalanceAsync(id),
            Rows = (await ledger.GetSupplierLedgerAsync(id)).ToList()
        };

        return View("~/Views/Shared/Ledger.cshtml", model);
    }
}
