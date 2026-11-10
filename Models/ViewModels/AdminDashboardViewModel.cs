namespace LedgerPro.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        
        public string UserName { get; set; } = "";

        
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal OutstandingReceivables { get; set; }
        public decimal OutstandingPayables { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSuppliers { get; set; }

        
        public List<RecentTransaction> RecentTransactions { get; set; } = new();
        public List<TopCustomer> TopCustomers { get; set; } = new();
        public List<string> RecentActivities { get; set; } = new();
    }

    public class RecentTransaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Party { get; set; } = "";
        public string Type { get; set; } = "";    
        public decimal Amount { get; set; }
        public string Status { get; set; } = "";   
    }

    public class TopCustomer
    {
        public string Name { get; set; } = "";
        public decimal Balance { get; set; }
        public int Orders { get; set; }
    }
}