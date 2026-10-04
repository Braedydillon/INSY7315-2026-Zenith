namespace INSY7315_Prototype.ViewModels
{
    public class ManagerLoanApplicationViewModel
    {
        public string ApplicationId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string  UserEmail {  get; set; } = string.Empty;
        public decimal RequestedAmount { get; set; }
        public string ReasonForLoan { get; set; } = string.Empty;
        public DateTime DateApplied { get; set; }
        public string Status {  get; set; } = string.Empty;
        public ClientDetailsViewModel ClientDetails { get; set; } = new();

    }
}
