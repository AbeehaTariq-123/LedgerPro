using LedgerPro.Models;
namespace LedgerPro.Repositories.Interfaces;

public interface ISupplierRepository { Task<IEnumerable<Supplier>> GetAllAsync(); Task<Supplier?> GetByIdAsync(int id); Task<int> AddAsync(Supplier model); Task<bool> UpdateAsync(Supplier model); Task<bool> DeleteAsync(int id); }
