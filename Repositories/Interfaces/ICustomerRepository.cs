using LedgerPro.Models;
namespace LedgerPro.Repositories.Interfaces;

public interface ICustomerRepository { Task<IEnumerable<Customer>> GetAllAsync(); Task<Customer?> GetByIdAsync(int id); Task<int> AddAsync(Customer model); Task<bool> UpdateAsync(Customer model); Task<bool> DeleteAsync(int id); }
