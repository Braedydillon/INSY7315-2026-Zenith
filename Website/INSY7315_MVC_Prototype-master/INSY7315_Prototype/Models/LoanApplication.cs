using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace INSY7315_Prototype.Models
{
    public class LoanApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required]
        public string ClientID { get; set; } = string.Empty;

        [ForeignKey(nameof(ClientID))]
        public ApplicationUser? Client { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RequestedAmount { get; set; }

        [Required]
        public ApplicationStatus Status { get; set; }

        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        // Navigation properties
        public IdentityVerification? IdentityVerification { get; set; }

        public ICollection<Document> Documents { get; set; }
            = new List<Document>();

        public Loan? Loan { get; set; }
    }

    public enum ApplicationStatus
    {
        Pending,
        UnderReview,
        Approved,
        Rejected,
        Cancelled
    }
}
