using LedgerPro.Models;
using LedgerPro.Models.ViewModels;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LedgerPro.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class CustomerController(ICustomerRepository customers, ILedgerRepository ledger) : Controller
{
    // GET /Customer
    public IActionResult Index() => View();

    // GET /Customer/GetAll  (called by wwwroot/js/customers.js)
    [HttpGet]
    public async Task<IActionResult> GetAll() => Json(await customers.GetAllAsync());

    // GET /Customer/Create
    [HttpGet]
    public IActionResult Create() => View("Form", new Customer());

    // GET /Customer/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await customers.GetByIdAsync(id);
        return customer is null ? NotFound() : View("Form", customer);
    }

    // POST /Customer/Save  (add when CustomerId == 0, otherwise update)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Customer model)
    {
        if (!ModelState.IsValid) return View("Form", model);

        if (model.CustomerId == 0)
        {
            await customers.AddAsync(model);
            TempData["Success"] = $"Customer “{model.Name}” added.";
        }
        else
        {
            if (!await customers.UpdateAsync(model)) return NotFound();
            TempData["Success"] = $"Customer “{model.Name}” updated.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /Customer/Delete  (called by customers.js)
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            return await customers.DeleteAsync(id) ? Ok() : NotFound("Customer not found.");
        }
        catch (SqlException ex) when (ex.Number == 547) // foreign key violation
        {
            return Conflict("This customer has transactions or payments, so it can't be deleted.");
        }
    }

    // GET /Customer/Ledger/5
    [HttpGet]
    public async Task<IActionResult> Ledger(int id)
    {
        var customer = await customers.GetByIdAsync(id);
        if (customer is null) return NotFound();

        var model = new LedgerViewModel
        {
            PartyId = id,
            PartyName = customer.Name,
            IsCustomer = true,
            OpeningBalance = customer.OpeningBalance,
            Balance = await ledger.GetCustomerBalanceAsync(id),
            Rows = (await ledger.GetCustomerLedgerAsync(id)).ToList()
        };

        return View("~/Views/Shared/Ledger.cshtml", model);
    }
}
