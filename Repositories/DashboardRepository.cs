using Dapper;
using LedgerPro.Data;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
namespace LedgerPro.Repositories;

public sealed class DashboardRepository(DbConnectionFactory factory) : IDashboardRepository
{
    public async Task<DashboardSummary> GetSummaryAsync(DateTime from, DateTime to) { using var db = factory.CreateConnection(); var p = new { from, to = to.Date.AddDays(1) }; return await db.QuerySingleAsync<DashboardSummary>(@"SELECT (SELECT COALESCE(SUM(TotalAmount),0) FROM Sales WHERE SaleDate>=@from AND SaleDate<@to) Sales,(SELECT COALESCE(SUM(TotalAmount),0) FROM Purchases WHERE PurchaseDate>=@from AND PurchaseDate<@to) Purchases,(SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE PaymentType='Received' AND PaymentDate>=@from AND PaymentDate<@to) PaymentsReceived,(SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE PaymentType='Paid' AND PaymentDate>=@from AND PaymentDate<@to) PaymentsPaid,(SELECT COALESCE(SUM(CASE WHEN TransactionType IN ('Sale','Credit') THEN Amount WHEN TransactionType IN ('Received','Payment') THEN -Amount ELSE 0 END),0) FROM Transactions WHERE CustomerId IS NOT NULL) CustomerOutstanding,(SELECT COALESCE(SUM(CASE WHEN TransactionType IN ('Purchase','Credit') THEN Amount WHEN TransactionType IN ('Paid','Payment') THEN -Amount ELSE 0 END),0) FROM Transactions WHERE SupplierId IS NOT NULL) SupplierOutstanding", p); }
}
