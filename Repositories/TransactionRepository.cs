using Dapper;
using LedgerPro.Data;
using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
namespace LedgerPro.Repositories;

public sealed class TransactionRepository(DbConnectionFactory factory) : ITransactionRepository
{
    public async Task<IEnumerable<Transaction>> GetAllAsync() { using var db = factory.CreateConnection(); return await db.QueryAsync<Transaction>("SELECT * FROM Transactions ORDER BY TransactionDate DESC,TransactionId DESC"); }
    public async Task<Transaction?> GetByIdAsync(int id) { using var db = factory.CreateConnection(); return await db.QuerySingleOrDefaultAsync<Transaction>("SELECT * FROM Transactions WHERE TransactionId=@id", new { id }); }
    public async Task<int> AddAsync(Transaction m)
    {
        using var db = factory.CreateConnection();
        db.Open();
        using var tx = db.BeginTransaction();
        try
        {
            var id = await db.ExecuteScalarAsync<int>(
                "INSERT INTO Transactions(CustomerId,SupplierId,TransactionType,Amount,TransactionDate,Description) VALUES(@CustomerId,@SupplierId,@TransactionType,@Amount,@TransactionDate,@Description); SELECT CAST(SCOPE_IDENTITY() AS int);",
                m, tx);

            // The dashboard and reports read totals from Sales / Purchases, so keep them in sync.
            if (m.TransactionType == "Sale" && m.CustomerId.HasValue)
                await db.ExecuteAsync("INSERT INTO Sales(CustomerId,SaleDate,TotalAmount) VALUES(@CustomerId,@TransactionDate,@Amount)", m, tx);
            else if (m.TransactionType == "Purchase" && m.SupplierId.HasValue)
                await db.ExecuteAsync("INSERT INTO Purchases(SupplierId,PurchaseDate,TotalAmount) VALUES(@SupplierId,@TransactionDate,@Amount)", m, tx);

            tx.Commit();
            return id;
        }
        catch { tx.Rollback(); throw; }
    }
    public async Task<IEnumerable<Transaction>> GetForCustomerAsync(int id) { using var db = factory.CreateConnection(); return await db.QueryAsync<Transaction>("SELECT * FROM Transactions WHERE CustomerId=@id ORDER BY TransactionDate DESC", new { id }); }
    public async Task<IEnumerable<Transaction>> GetForSupplierAsync(int id) { using var db = factory.CreateConnection(); return await db.QueryAsync<Transaction>("SELECT * FROM Transactions WHERE SupplierId=@id ORDER BY TransactionDate DESC", new { id }); }
}
