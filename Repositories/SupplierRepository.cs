using Dapper;
using LedgerPro.Data;
using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
namespace LedgerPro.Repositories;

public sealed class SupplierRepository(DbConnectionFactory factory) : ISupplierRepository
{
    public async Task<IEnumerable<Supplier>> GetAllAsync() { using var db = factory.CreateConnection(); return await db.QueryAsync<Supplier>("SELECT SupplierId,Name,Phone,Email,Address,OpeningBalance FROM Suppliers ORDER BY SupplierId DESC"); }
    public async Task<Supplier?> GetByIdAsync(int id) { using var db = factory.CreateConnection(); return await db.QuerySingleOrDefaultAsync<Supplier>("SELECT * FROM Suppliers WHERE SupplierId=@id", new { id }); }
    public async Task<int> AddAsync(Supplier m) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<int>("INSERT INTO Suppliers(Name,Phone,Email,Address,OpeningBalance) VALUES(@Name,@Phone,@Email,@Address,@OpeningBalance); SELECT CAST(SCOPE_IDENTITY() AS int);", m); }
    public async Task<bool> UpdateAsync(Supplier m) { using var db = factory.CreateConnection(); return await db.ExecuteAsync("UPDATE Suppliers SET Name=@Name,Phone=@Phone,Email=@Email,Address=@Address,OpeningBalance=@OpeningBalance WHERE SupplierId=@SupplierId", m) > 0; }
    public async Task<bool> DeleteAsync(int id) { using var db = factory.CreateConnection(); return await db.ExecuteAsync("DELETE FROM Suppliers WHERE SupplierId=@id", new { id }) > 0; }
}
