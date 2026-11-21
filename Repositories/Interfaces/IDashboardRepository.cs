namespace LedgerPro.Repositories.Interfaces;

public interface IDashboardRepository { Task<DashboardSummary> GetSummaryAsync(DateTime from, DateTime to); }
public sealed record DashboardSummary(decimal Sales, decimal Purchases, decimal PaymentsReceived, decimal PaymentsPaid, decimal CustomerOutstanding, decimal SupplierOutstanding);
