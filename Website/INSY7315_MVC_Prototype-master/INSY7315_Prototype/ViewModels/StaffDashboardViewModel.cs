namespace INSY7315_Prototype.ViewModels
{
    public class StaffDashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int AwaitingVerification { get; set; }
        public int AwaitingApproval { get; set; }
        public int SentToManager { get; set; }
        public List<ManagerLoanApplicationViewModel> RecentApplications { get; set; } = new();
    }
}