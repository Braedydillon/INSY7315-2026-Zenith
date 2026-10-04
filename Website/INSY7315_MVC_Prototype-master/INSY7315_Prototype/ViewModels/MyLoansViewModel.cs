using INSY7315_Prototype.Models;

namespace INSY7315_Prototype.ViewModels
{
    public class MyLoansViewModel
    {
        public List<Loan> Loans { get; set; } = new();
        public int Total => Loans.Count;
        public int Pending => Loans.Count(l => string.Equals(l.Status,"Submitted",StringComparison.OrdinalIgnoreCase) || string.Equals(l.Status,"UnderReview",StringComparison.OrdinalIgnoreCase));
        public int Approved => Loans.Count(l => string.Equals(l.Status, "Approved", StringComparison.OrdinalIgnoreCase));
        public int Rejected => Loans.Count(l => string.Equals(l.Status, "Rejected", StringComparison.OrdinalIgnoreCase));
        public decimal TotalRequested => Loans.Sum(l => l.RequestedAmount);
     
    }
}
