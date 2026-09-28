using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace INSY7315_Prototype.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<LoanApplication> LoanApplications { get; set; }
            = new List<LoanApplication>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }

    public enum UserRole
    {
        Client,
        Staff,
        Manager
    }
}
