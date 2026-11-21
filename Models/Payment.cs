namespace LedgerPro.Models;

public sealed class Payment
{
    public int PaymentId { get; set; }
    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentType { get; set; } = "Received"; // Received or Paid
    public DateTime PaymentDate { get; set; }
    public string? Description { get; set; }
}
