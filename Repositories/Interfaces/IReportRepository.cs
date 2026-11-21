namespace LedgerPro.Repositories.Interfaces;

public interface IReportRepository { Task<decimal> GetSalesTotalAsync(DateTime from, DateTime to); Task<decimal> GetPurchaseTotalAsync(DateTime from, DateTime to); Task<decimal> GetPaymentsReceivedAsync(DateTime from, DateTime to); Task<decimal> GetPaymentsPaidAsync(DateTime from, DateTime to); }
