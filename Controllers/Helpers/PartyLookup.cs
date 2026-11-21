using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;

namespace LedgerPro.Controllers.Helpers;

/// <summary>Resolves a customer/supplier id pair to a display name (used by list screens and the dashboard).</summary>
public sealed class PartyLookup
{
    private readonly Dictionary<int, string> _customers;
    private readonly Dictionary<int, string> _suppliers;

    private PartyLookup(IEnumerable<Customer> customers, IEnumerable<Supplier> suppliers)
    {
        _customers = customers.ToDictionary(c => c.CustomerId, c => c.Name);
        _suppliers = suppliers.ToDictionary(s => s.SupplierId, s => s.Name);
    }

    public static async Task<PartyLookup> LoadAsync(ICustomerRepository customers, ISupplierRepository suppliers)
        => new(await customers.GetAllAsync(), await suppliers.GetAllAsync());

    public string Name(int? customerId, int? supplierId)
    {
        if (customerId is int c && _customers.TryGetValue(c, out var customerName)) return customerName;
        if (supplierId is int s && _suppliers.TryGetValue(s, out var supplierName)) return supplierName;
        return "—";
    }
}
