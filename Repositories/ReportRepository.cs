using Dapper;
using LedgerPro.Data;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
namespace LedgerPro.Repositories;

public sealed class ReportRepository(DbConnectionFactory factory) : IReportRepository
{
    public async Task<decimal> GetSalesTotalAsync(DateTime from, DateTime to) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(TotalAmount),0) FROM Sales WHERE SaleDate>=@from AND SaleDate<@to", new { from, to = to.Date.AddDays(1) }); }
    public async Task<decimal> GetPurchaseTotalAsync(DateTime from, DateTime to) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(TotalAmount),0) FROM Purchases WHERE PurchaseDate>=@from AND PurchaseDate<@to", new { from, to = to.Date.AddDays(1) }); }
    public async Task<decimal> GetPaymentsReceivedAsync(DateTime from, DateTime to) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE PaymentType='Received' AND PaymentDate>=@from AND PaymentDate<@to", new { from, to = to.Date.AddDays(1) }); }
    public async Task<decimal> GetPaymentsPaidAsync(DateTime from, DateTime to) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE PaymentType='Paid' AND PaymentDate>=@from AND PaymentDate<@to", new { from, to = to.Date.AddDays(1) }); }
}
