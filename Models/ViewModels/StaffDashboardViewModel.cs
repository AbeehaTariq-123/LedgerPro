namespace LedgerPro.Models.ViewModels
{
    public class StaffDashboardViewModel
    {
        public string UserName { get; set; } = "";

        public decimal TodaySales { get; set; }
        public decimal TodayPurchases { get; set; }
        public decimal TodayPaymentsReceived { get; set; }
        public int TransactionsCount { get; set; }
        public int PendingPayments { get; set; }

        
        public List<StaffTransaction> TodaysTransactions { get; set; } = new();
        public List<LowStockProduct> LowStockProducts { get; set; } = new();
        public List<string> RecentActivities { get; set; } = new();
    }

    public class StaffTransaction
    {
        public int Id { get; set; }
        public string Time { get; set; } = "";     
        public string Party { get; set; } = "";
        public string Type { get; set; } = "";      
        public decimal Amount { get; set; }
        public string Status { get; set; } = "";    
    }

    public class LowStockProduct
    {
        public string Name { get; set; } = "";
        public int Stock { get; set; }
        public int MinStock { get; set; }
    }
}