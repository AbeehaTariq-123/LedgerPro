using Dapper;
using LedgerPro.Data;
using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
namespace LedgerPro.Repositories;

public sealed class LedgerRepository(DbConnectionFactory factory) : ILedgerRepository
{
    public async Task<IEnumerable<AccountLedgerRow>> GetCustomerLedgerAsync(int id) { using var db = factory.CreateConnection(); return await db.QueryAsync<AccountLedgerRow>(@"WITH E AS (SELECT TransactionDate EntryDate,TransactionType EntryType,Description,CASE WHEN TransactionType IN ('Sale','Credit','Debit') THEN Amount ELSE 0 END Debit,CASE WHEN TransactionType IN ('Received','Payment','Receipt') THEN Amount ELSE 0 END Credit FROM Transactions WHERE CustomerId=@id) SELECT *, SUM(Debit-Credit) OVER(ORDER BY EntryDate ROWS UNBOUNDED PRECEDING) RunningBalance FROM E ORDER BY EntryDate", new { id }); }
    public async Task<IEnumerable<AccountLedgerRow>> GetSupplierLedgerAsync(int id) { using var db = factory.CreateConnection(); return await db.QueryAsync<AccountLedgerRow>(@"WITH E AS (SELECT TransactionDate EntryDate,TransactionType EntryType,Description,CASE WHEN TransactionType IN ('Purchase','Credit') THEN Amount ELSE 0 END Credit,CASE WHEN TransactionType IN ('Paid','Payment','Debit') THEN Amount ELSE 0 END Debit FROM Transactions WHERE SupplierId=@id) SELECT *, SUM(Credit-Debit) OVER(ORDER BY EntryDate ROWS UNBOUNDED PRECEDING) RunningBalance FROM E ORDER BY EntryDate", new { id }); }
    public async Task<decimal> GetCustomerBalanceAsync(int id) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<decimal>(@"SELECT COALESCE((SELECT OpeningBalance FROM Customers WHERE CustomerId=@id),0)+COALESCE(SUM(CASE WHEN TransactionType IN ('Sale','Credit','Debit') THEN Amount WHEN TransactionType IN ('Received','Payment','Receipt') THEN -Amount ELSE 0 END),0) FROM Transactions WHERE CustomerId=@id", new { id }); }
    public async Task<decimal> GetSupplierBalanceAsync(int id) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<decimal>(@"SELECT COALESCE((SELECT OpeningBalance FROM Suppliers WHERE SupplierId=@id),0)+COALESCE(SUM(CASE WHEN TransactionType IN ('Purchase','Credit') THEN Amount WHEN TransactionType IN ('Paid','Payment','Debit') THEN -Amount ELSE 0 END),0) FROM Transactions WHERE SupplierId=@id", new { id }); }
}
