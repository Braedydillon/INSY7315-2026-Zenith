
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INSY7315_Prototype.Models
{
    public class ClientDetailsDto
    {
        [Required(ErrorMessage = "Full name and surname are required.")]
        [StringLength(120, MinimumLength = 3)]
        [Display(Name = "Full name and surname")]
        public string FullNameAndSurname { get; set; } = "";

        [Required(ErrorMessage = "ID number is required.")]
        [RegularExpression(@"^\d{13}$",
            ErrorMessage = "Enter a 13-digit South African ID number.")]
        [Display(Name = "ID number")]
        public string IdNumber { get; set; } = "";

        [Required(ErrorMessage = "Cell number is required.")]
        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Enter a 10-digit number starting with 0.")]
        [Display(Name = "Cell number")]
        public string CellNo { get; set; } = "";
    }

    public class LoanApplyRequest
    {

        [Range(typeof(decimal), "300", "500000", ErrorMessage = "Loan amount must be between R300 and R500,000.")]
        [Display(Name = "Requested amount (R)")]
        public decimal RequestedAmount { get; set; }

        [Required(ErrorMessage = "Please provide a reason for your loan.")]
        [StringLength(500)]
        [Display(Name = "Main reason for loan")]
        public string ReasonForLoan { get; set; } = "";

        [Required]
        public ClientDetailsDto ClientDetails { get; set; } = new();

        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Enter a valid 10-digit telephone number.")]
        public string? HomeTelNo { get; set; }

        public string? MarriedOrUnmarried { get; set; }
        public string? MarriageCommunity { get; set; }
        public string? PreviouslyDivorced { get; set; }
        public string? DivorceYear { get; set; }
        public string? DivorceCommunity { get; set; }

        [Required(ErrorMessage = "Physical address is required.")]
        public string CurrentPhysicalAddress { get; set; } = "";

        public string? PostalAddress { get; set; }
        public string? ParentsAddress { get; set; }

        [Range(0, 100, ErrorMessage = "Enter a valid number of years.")]
        public int? ResidenceYears { get; set; }

        [Range(0, 11, ErrorMessage = "Months must be between 0 and 11.")]
        public int? ResidenceMonths { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyName { get; set; } = "";

        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Enter a valid 10-digit telephone number.")]
        public string? WorkTelephone { get; set; }

        [Required(ErrorMessage = "Occupation is required.")]
        public string Occupation { get; set; } = "";

        public string? WorkAddress { get; set; }

        [Required(ErrorMessage = "Bank name is required.")]
        public string BankName { get; set; } = "";

        [Required(ErrorMessage = "Account type is required.")]
        public string AccountType { get; set; } = "";

        [Required(ErrorMessage = "Account number is required.")]
        [RegularExpression(@"^\d{6,17}$",
            ErrorMessage = "Account number must contain 6 to 17 digits.")]
        public string AccountNumber { get; set; } = "";

        public string? BranchName { get; set; }

        [Required(ErrorMessage = "Account holder name is required.")]
        public string AccountName { get; set; } = "";

        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "Branch code must contain 6 digits.")]
        public string? BranchCode { get; set; }

        // Spouse fields are validated conditionally
        // in LoanController when the applicant is married.
        public string? SpouseNameAndSurname { get; set; }

        [RegularExpression(@"^\d{13}$",
            ErrorMessage = "Spouse ID number must contain 13 digits.")]
        public string? SpouseIdNumber { get; set; }

        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Enter a valid spouse telephone number.")]
        public string? SpouseTelNumber { get; set; }

        public string? SpouseEmployerName { get; set; }
        public string? SpouseEmployerAddress { get; set; }

        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Enter a valid employer telephone number.")]
        public string? SpouseEmployerTelNumber { get; set; }

        [Required(ErrorMessage = "Relative name is required.")]
        public string? Relative1Name { get; set; }

        [Required(ErrorMessage = "Relative relationship is required.")]
        public string? Relative1Relationship { get; set; }

        [Required(ErrorMessage = "Relative telephone number is required.")]
        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Enter a valid 10-digit relative telephone number.")]
        public string? Relative1TelNumber { get; set; }

        [Required(ErrorMessage = "Relative address is required.")]
        public string? Relative1Address { get; set; }

        public string? Relative2Name { get; set; }
        public string? Relative2Relationship { get; set; }

        [RegularExpression(@"^0\d{9}$",
            ErrorMessage = "Enter a valid 10-digit relative telephone number.")]
        public string? Relative2TelNumber { get; set; }

        public string? Relative2Address { get; set; }

        public List<string> ReasonsForLoan { get; set; } = new();

        public string? OtherReason { get; set; }

        [Required(ErrorMessage = "Please type your full name as your signature.")]
        [Display(Name = "Type your full name as signature")]
        public string ApplicantSignature { get; set; } = "";

        public DateTime? ApplicantFormDate { get; set; } = DateTime.UtcNow;
    }
}
