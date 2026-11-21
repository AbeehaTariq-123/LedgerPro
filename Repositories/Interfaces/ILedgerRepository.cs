using LedgerPro.Models;
namespace LedgerPro.Repositories.Interfaces;

public interface ILedgerRepository { Task<IEnumerable<AccountLedgerRow>> GetCustomerLedgerAsync(int customerId); Task<IEnumerable<AccountLedgerRow>> GetSupplierLedgerAsync(int supplierId); Task<decimal> GetCustomerBalanceAsync(int customerId); Task<decimal> GetSupplierBalanceAsync(int supplierId); }
