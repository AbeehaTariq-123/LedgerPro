namespace LedgerPro.Models.ViewModels;

public class LedgerViewModel
{
    public int PartyId { get; set; }
    public string PartyName { get; set; } = "";
    public bool IsCustomer { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal Balance { get; set; }
    public List<AccountLedgerRow> Rows { get; set; } = new();
}
