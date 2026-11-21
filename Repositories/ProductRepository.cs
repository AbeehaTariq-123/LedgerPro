using Dapper;
using LedgerPro.Data;
using LedgerPro.Models;
using LedgerPro.Repositories.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.CodeAnalysis;
namespace LedgerPro.Repositories;

public sealed class ProductRepository(DbConnectionFactory factory) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllAsync() { using var db = factory.CreateConnection(); return await db.QueryAsync<Product>("SELECT * FROM Products ORDER BY ProductId DESC"); }
    public async Task<Product?> GetByIdAsync(int id) { using var db = factory.CreateConnection(); return await db.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE ProductId=@id", new { id }); }
    public async Task<int> AddAsync(Product m) { using var db = factory.CreateConnection(); return await db.ExecuteScalarAsync<int>("INSERT INTO Products(ProductName,SKU,PurchasePrice,SalePrice,StockQuantity,ReorderLevel) VALUES(@ProductName,@SKU,@PurchasePrice,@SalePrice,@StockQuantity,@ReorderLevel); SELECT CAST(SCOPE_IDENTITY() AS int);", m); }
    public async Task<bool> UpdateAsync(Product m) { using var db = factory.CreateConnection(); return await db.ExecuteAsync("UPDATE Products SET ProductName=@ProductName,SKU=@SKU,PurchasePrice=@PurchasePrice,SalePrice=@SalePrice,StockQuantity=@StockQuantity,ReorderLevel=@ReorderLevel WHERE ProductId=@ProductId", m) > 0; }
    public async Task<bool> DeleteAsync(int id) { using var db = factory.CreateConnection(); return await db.ExecuteAsync("DELETE FROM Products WHERE ProductId=@id", new { id }) > 0; }
}
