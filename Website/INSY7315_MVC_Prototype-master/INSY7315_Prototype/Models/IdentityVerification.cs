using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INSY7315_Prototype.Models
{
    public class IdentityVerification
    {
        [Key]
        public int VerificationID { get; set; }

        public int ApplicationID { get; set; }

        [ForeignKey(nameof(ApplicationID))]
        public LoanApplication? LoanApplication { get; set; }

        public VerificationStatus Status { get; set; }

        public string Method { get; set; } = string.Empty;

        public DateTime VerificationDate { get; set; } = DateTime.UtcNow;
    }

    public enum VerificationStatus
    {
        Pending,
        Verified,
        Failed
    }
}
