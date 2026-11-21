namespace LedgerPro.Models.ViewModels;

public class InvoiceRowViewModel
{
    public int TransactionId { get; set; }
    public string InvoiceNumber => $"INV-{TransactionId:D6}";
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal Amount { get; set; }
}

public class InvoiceViewModel
{
    public int TransactionId { get; set; }
    public string InvoiceNumber => $"INV-{TransactionId:D6}";
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public decimal CustomerBalance { get; set; }
}
