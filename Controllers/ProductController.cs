using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LedgerPro.Controllers;

[Authorize(Roles = "Admin")]
public class ProductController(IProductRepository products) : Controller
{
    // GET /Product
    public IActionResult Index() => View();

    // GET /Product/GetAll  (called by wwwroot/js/products.js)
    [HttpGet]
    public async Task<IActionResult> GetAll() => Json(await products.GetAllAsync());

    // GET /Product/Create
    [HttpGet]
    public IActionResult Create() => View("Form", new Product());

    // GET /Product/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : View("Form", product);
    }

    // POST /Product/Save  (add when ProductId == 0, otherwise update)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Product model)
    {
        // Products.SKU is UNIQUE in the schema, and SQL Server allows only ONE NULL in a unique column,
        // so a blank SKU would break the second product. Require it.
        if (string.IsNullOrWhiteSpace(model.SKU))
            ModelState.AddModelError(nameof(Product.SKU), "SKU is required.");

        if (model.PurchasePrice < 0) ModelState.AddModelError(nameof(Product.PurchasePrice), "Price can't be negative.");
        if (model.SalePrice < 0) ModelState.AddModelError(nameof(Product.SalePrice), "Price can't be negative.");
        if (model.StockQuantity < 0) ModelState.AddModelError(nameof(Product.StockQuantity), "Stock can't be negative.");
        if (model.ReorderLevel < 0) ModelState.AddModelError(nameof(Product.ReorderLevel), "Reorder level can't be negative.");

        if (!ModelState.IsValid) return View("Form", model);

        try
        {
            if (model.ProductId == 0)
            {
                await products.AddAsync(model);
                TempData["Success"] = $"Product “{model.ProductName}” added.";
            }
            else
            {
                if (!await products.UpdateAsync(model)) return NotFound();
                TempData["Success"] = $"Product “{model.ProductName}” updated.";
            }
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627) // duplicate SKU
        {
            ModelState.AddModelError(nameof(Product.SKU), "Another product already uses this SKU.");
            return View("Form", model);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /Product/Delete  (called by products.js)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            return await products.DeleteAsync(id) ? Ok() : NotFound("Product not found.");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return Conflict("This product is referenced elsewhere, so it can't be deleted.");
        }
    }
}
