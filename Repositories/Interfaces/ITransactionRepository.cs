using LedgerPro.Models;
namespace LedgerPro.Repositories.Interfaces;

public interface ITransactionRepository { Task<IEnumerable<Transaction>> GetAllAsync(); Task<Transaction?> GetByIdAsync(int id); Task<int> AddAsync(Transaction model); Task<IEnumerable<Transaction>> GetForCustomerAsync(int customerId); Task<IEnumerable<Transaction>> GetForSupplierAsync(int supplierId); }
