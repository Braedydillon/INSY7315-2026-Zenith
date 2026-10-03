using INSY7315_Prototype.Models;

namespace INSY7315_Prototype.ViewModels
{
    public class MyLoansViewModel
    {
       
            public List<Loan> Loans { get; set; } = new();
            public int Total => Loans.Count;
            public int Pending => Loans.Count(l => l.Status == "Pending");
            public int Approved => Loans.Count(l => l.Status == "Approved");
            public decimal TotalRequested => Loans.Sum(l => l.RequestedAmount);
     
    }
}
