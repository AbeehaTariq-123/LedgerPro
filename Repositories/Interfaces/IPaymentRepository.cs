using LedgerPro.Models;
namespace LedgerPro.Repositories.Interfaces;

public interface IPaymentRepository { Task<IEnumerable<Payment>> GetAllAsync(); Task<int> AddAsync(Payment model); Task<IEnumerable<Payment>> GetForCustomerAsync(int customerId); Task<IEnumerable<Payment>> GetForSupplierAsync(int supplierId); }
