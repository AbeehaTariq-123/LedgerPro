namespace LedgerPro.Models;

public sealed class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string? SKU { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal ReorderLevel { get; set; }
}
