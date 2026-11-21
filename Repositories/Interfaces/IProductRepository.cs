using LedgerPro.Models;
using Microsoft.CodeAnalysis;
namespace LedgerPro.Repositories.Interfaces;

public interface IProductRepository { Task<IEnumerable<Product>> GetAllAsync(); Task<Product?> GetByIdAsync(int id); Task<int> AddAsync(Product model); Task<bool> UpdateAsync(Product model); Task<bool> DeleteAsync(int id); }
