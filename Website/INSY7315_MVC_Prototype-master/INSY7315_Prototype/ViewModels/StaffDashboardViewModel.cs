namespace INSY7315_Prototype.ViewModels
{
    public class StaffDashboardViewModel
    {
        public int PendingCount { get; set; }
        public int SentToManagerCount { get; set; }
        public int ApprovedCount { get; set; }
        public int DeclinedCount { get; set; }
        public List<StaffLoanApplicationViewModel> Applications { get; set; } = new();
    }
}