namespace INSY7315_Prototype.ViewModels
{
    public class StaffDashboardViewModel
    {
        public int PendingCount { get; set; }
        public int VerifiedCount { get; set; }
        public int RejectedCount { get; set; }
        public List<StaffLoanApplicationViewModel> Applications { get; set; } = new();
    }
}
