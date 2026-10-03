using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INSY7315_Prototype.Models
{
    public class Loan
    {
        public string? Id { get; set; }
        public decimal RequestedAmount { get; set; }
        public string? ReasonForLoan { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime ApplicantFormDate { get; set; }
    }

}
