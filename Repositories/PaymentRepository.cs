using Dapper;
using LedgerPro.Data;
using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
namespace LedgerPro.Repositories;

public sealed class PaymentRepository(DbConnectionFactory factory) : IPaymentRepository
{
    public async Task<IEnumerable<Payment>> GetAllAsync() { using var db = factory.CreateConnection(); return await db.QueryAsync<Payment>("SELECT * FROM Payments ORDER BY PaymentDate DESC,PaymentId DESC"); }
    public async Task<int> AddAsync(Payment m) { using var db = factory.CreateConnection(); db.Open(); using var tx = db.BeginTransaction(); try { var id = await db.ExecuteScalarAsync<int>("INSERT INTO Payments(CustomerId,SupplierId,Amount,PaymentType,PaymentDate,Description) VALUES(@CustomerId,@SupplierId,@Amount,@PaymentType,@PaymentDate,@Description); SELECT CAST(SCOPE_IDENTITY() AS int);", m, tx); await db.ExecuteAsync("INSERT INTO Transactions(CustomerId,SupplierId,TransactionType,Amount,TransactionDate,Description) VALUES(@CustomerId,@SupplierId,@PaymentType,@Amount,@PaymentDate,@Description)", m, tx); tx.Commit(); return id; } catch { tx.Rollback(); throw; } }
    public async Task<IEnumerable<Payment>> GetForCustomerAsync(int id) { using var db = factory.CreateConnection(); return await db.QueryAsync<Payment>("SELECT * FROM Payments WHERE CustomerId=@id ORDER BY PaymentDate DESC", new { id }); }
    public async Task<IEnumerable<Payment>> GetForSupplierAsync(int id) { using var db = factory.CreateConnection(); return await db.QueryAsync<Payment>("SELECT * FROM Payments WHERE SupplierId=@id ORDER BY PaymentDate DESC", new { id }); }
}
