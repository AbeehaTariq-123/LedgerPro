using LedgerPro.Models.ViewModels;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerPro.Controllers;

[Authorize(Roles = "Admin")]
public class ReportController(IReportRepository reports) : Controller
{
    // GET /Report?from=2026-11-01&to=2026-11-22   (defaults to the current month)
    public async Task<IActionResult> Index(
        [FromQuery(Name = "from")] DateTime? fromDate,
        [FromQuery(Name = "to")] DateTime? toDate)
    {
        var today = DateTime.Today;
        var from = (fromDate ?? new DateTime(today.Year, today.Month, 1)).Date;
        var to = (toDate ?? today).Date;

        if (to < from) (from, to) = (to, from);

        var model = new ReportViewModel
        {
            From = from,
            To = to,
            Sales = await reports.GetSalesTotalAsync(from, to),
            Purchases = await reports.GetPurchaseTotalAsync(from, to),
            PaymentsReceived = await reports.GetPaymentsReceivedAsync(from, to),
            PaymentsPaid = await reports.GetPaymentsPaidAsync(from, to)
        };

        return View(model);
    }
}
