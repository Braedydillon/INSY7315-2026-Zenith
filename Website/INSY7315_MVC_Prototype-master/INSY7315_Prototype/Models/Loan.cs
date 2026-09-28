using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INSY7315_Prototype.Models
{
    public class Loan
    {
        [Key]
        public int LoanID { get; set; }

        public int ApplicationID { get; set; }

        [ForeignKey(nameof(ApplicationID))]
        public LoanApplication? LoanApplication { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        public int TermMonths { get; set; }

        public LoanStatus Status { get; set; }

        public DateTime? DisbursementDate { get; set; }

    }

    public enum LoanStatus
    {
        Active,
        Disbursed,
        Closed,
        Defaulted
    }
}
