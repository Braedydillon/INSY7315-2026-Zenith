using System.ComponentModel.DataAnnotations;

namespace INSY7315_Prototype.Models
{
    public class ClientDetailsDto
    {
        [Required, Display(Name = "Full name and surname")]
        public string FullNameAndSurname { get; set; } = "";

        [Required, RegularExpression(@"^\d{13}$", ErrorMessage = "ID number must be 13 digits.")]
        [Display(Name = "ID number")]
        public string IdNumber { get; set; } = "";

        [Required, Display(Name = "Cell number")]
        public string CellNo { get; set; } = "";
    }

    public class LoanApplyRequest
    {
        [Required, Range(500, 10000000), Display(Name = "Requested amount (R)")]
        public decimal RequestedAmount { get; set; }

        [Required, Display(Name = "Main reason for loan")]
        public string ReasonForLoan { get; set; } = "";

        public ClientDetailsDto ClientDetails { get; set; } = new();

        public string? HomeTelNo { get; set; }
        public string? MarriedOrUnmarried { get; set; }
        public string? MarriageCommunity { get; set; }
        public string? PreviouslyDivorced { get; set; }
        public string? DivorceYear { get; set; }
        public string? DivorceCommunity { get; set; }

        [Required] public string CurrentPhysicalAddress { get; set; } = "";
        public string? PostalAddress { get; set; }
        public string? ParentsAddress { get; set; }
        public int ResidenceYears { get; set; }
        public int ResidenceMonths { get; set; }

        [Required] public string CompanyName { get; set; } = "";
        public string? WorkTelephone { get; set; }
        [Required] public string Occupation { get; set; } = "";
        public string? WorkAddress { get; set; }

        [Required] public string BankName { get; set; } = "";
        [Required] public string AccountType { get; set; } = "";
        [Required] public string AccountNumber { get; set; } = "";
        public string? BranchName { get; set; }
        [Required] public string AccountName { get; set; } = "";
        public string? BranchCode { get; set; }

        public string? SpouseNameAndSurname { get; set; }
        public string? SpouseIdNumber { get; set; }
        public string? SpouseTelNumber { get; set; }
        public string? SpouseEmployerName { get; set; }
        public string? SpouseEmployerAddress { get; set; }
        public string? SpouseEmployerTelNumber { get; set; }

        public string? Relative1Name { get; set; }
        public string? Relative1Relationship { get; set; }
        public string? Relative1TelNumber { get; set; }
        public string? Relative1Address { get; set; }
        public string? Relative2Name { get; set; }
        public string? Relative2Relationship { get; set; }
        public string? Relative2TelNumber { get; set; }
        public string? Relative2Address { get; set; }

        public List<string> ReasonsForLoan { get; set; } = new();
        public string? OtherReason { get; set; }

        [Required, Display(Name = "Type your full name as signature")]
        public string ApplicantSignature { get; set; } = "";

        public DateTime ApplicantFormDate { get; set; } = DateTime.UtcNow;
    }
}