using Dapper;
using LedgerPro.Data;
using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
namespace LedgerPro.Repositories;

public sealed class CustomerRepository(DbConnectionFactory factory) : ICustomerRepository
{
    public async Task<IEnumerable<Customer>> GetAllAsync() { using var db = factory.CreateConnection(); return await db.QueryAsync<Customer>("SELECT CustomerId,Name,Phone,Email,Address,OpeningBalance FROM Customers ORDER BY CustomerId DESC"); }
    public async Task<Customer?> GetByIdAsync(int id) { using var db = factory.CreateConnection(); return await db.QuerySingleOrDefaultAsync<Customer>("SELECT * FROM Customers WHERE CustomerId=@id", new { id }); }
    public async Task<int> AddAsync(Customer m) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<int>("INSERT INTO Customers(Name,Phone,Email,Address,OpeningBalance) VALUES(@Name,@Phone,@Email,@Address,@OpeningBalance); SELECT CAST(SCOPE_IDENTITY() AS int);", m); }
    public async Task<bool> UpdateAsync(Customer m) { using var db = factory.CreateConnection(); return await db.ExecuteAsync("UPDATE Customers SET Name=@Name,Phone=@Phone,Email=@Email,Address=@Address,OpeningBalance=@OpeningBalance WHERE CustomerId=@CustomerId", m) > 0; }
    public async Task<bool> DeleteAsync(int id) { using var db = factory.CreateConnection(); return await db.ExecuteAsync("DELETE FROM Customers WHERE CustomerId=@id", new { id }) > 0; }
}
