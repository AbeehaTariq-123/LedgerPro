namespace LedgerPro.Models;

public sealed class AccountLedgerRow
{
    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = "";
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal RunningBalance { get; set; }
}
