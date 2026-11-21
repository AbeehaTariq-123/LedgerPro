namespace LedgerPro.Models.ViewModels;

public class ReportViewModel
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal Sales { get; set; }
    public decimal Purchases { get; set; }
    public decimal PaymentsReceived { get; set; }
    public decimal PaymentsPaid { get; set; }

    public decimal SalesMinusPurchases => Sales - Purchases;
    public decimal NetCash => PaymentsReceived - PaymentsPaid;
}
