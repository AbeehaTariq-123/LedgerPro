namespace LedgerPro.Models;

public sealed class Transaction
{
    public int TransactionId { get; set; }
    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    public string TransactionType { get; set; } = ""; // Credit, Debit, Sale, Purchase, Payment
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
}
