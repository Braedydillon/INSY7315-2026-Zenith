using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INSY7315_Prototype.Models
{
    public class Document
    {
        [Key]
        public int DocumentID { get; set; }

        public int ApplicationID { get; set; }

        [ForeignKey(nameof(ApplicationID))]
        public LoanApplication? LoanApplication { get; set; }

        [Required]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string? FilePath { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}