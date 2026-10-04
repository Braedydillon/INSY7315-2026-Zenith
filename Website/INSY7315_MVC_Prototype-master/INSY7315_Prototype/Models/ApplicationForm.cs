using System.ComponentModel.DataAnnotations;

namespace INSY7315_Prototype.Models
{
    public class ApplicationForm
    {
        // ---- Client details ----
        [Required, StringLength(150)]
        [Display(Name = "Full name and surname")]
        public string FullNameAndSurname { get; set; } = "";

        [Required, RegularExpression(@"^\d{13}$", ErrorMessage = "ID number must be 13 digits.")]
        [Display(Name = "ID number")]
        public string IdNumber { get; set; } = "";

        [StringLength(30)]
        [Display(Name = "Home telephone")]
        public string HomeTelNo { get; set; } = "";

        [Required, RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "Enter a valid cell number.")]
        [Display(Name = "Cell number")]
        public string CellNo { get; set; } = "";

        [Display(Name = "Marital status")]
        public string MarriedOrUnmarried { get; set; } = "";

        [Display(Name = "If married: in or out of community")]
        public string MarriageCommunity { get; set; } = "";

        [Display(Name = "Previously divorced")]
        public string PreviouslyDivorced { get; set; } = "";

        [Display(Name = "Divorce year")]
        public string DivorceYear { get; set; } = "";

        [Display(Name = "If divorced: in or out of community")]
        public string DivorceCommunity { get; set; } = "";

        // ---- Address details ----
        [Required(ErrorMessage = "Please enter your current physical address.")]
        [Display(Name = "Current physical address")]
        public string CurrentPhysicalAddress { get; set; } = "";

        [Display(Name = "Postal address")]
        public string PostalAddress { get; set; } = "";

        [Display(Name = "Parents address")]
        public string ParentsAddress { get; set; } = "";

        [Display(Name = "Years at residence")]
        public int ResidenceYears { get; set; }

        [Display(Name = "Months at residence")]
        public int ResidenceMonths { get; set; }

        // ---- Employment ----
        [Required(ErrorMessage = "Please enter your company name.")]
        [Display(Name = "Company name")]
        public string CompanyName { get; set; } = "";

        [Display(Name = "Work telephone")]
        public string WorkTelephone { get; set; } = "";

        [Required(ErrorMessage = "Please enter your occupation.")]
        public string Occupation { get; set; } = "";

        [Display(Name = "Work address")]
        public string WorkAddress { get; set; } = "";

        // ---- Banking ----
        [Display(Name = "Bank name")]
        public string BankName { get; set; } = "";

        [Display(Name = "Account type")]
        public string AccountType { get; set; } = "";

        [Display(Name = "Account number")]
        public string AccountNumber { get; set; } = "";

        [Display(Name = "Branch name")]
        public string BranchName { get; set; } = "";

        [Display(Name = "Account name")]
        public string AccountName { get; set; } = "";

        [Display(Name = "Branch code")]
        public string BranchCode { get; set; } = "";

        // ---- Spouse / partner ----
        [Display(Name = "Spouse name and surname")]
        public string SpouseNameAndSurname { get; set; } = "";

        [Display(Name = "Spouse ID number")]
        public string SpouseIdNumber { get; set; } = "";

        [Display(Name = "Spouse telephone")]
        public string SpouseTelNumber { get; set; } = "";

        [Display(Name = "Spouse employer name")]
        public string SpouseEmployerName { get; set; } = "";

        [Display(Name = "Spouse employer address")]
        public string SpouseEmployerAddress { get; set; } = "";

        [Display(Name = "Spouse employer telephone")]
        public string SpouseEmployerTelNumber { get; set; } = "";

        // ---- Relatives ----
        [Display(Name = "Name")]
        public string Relative1Name { get; set; } = "";
        [Display(Name = "Relationship")]
        public string Relative1Relationship { get; set; } = "";
        [Display(Name = "Telephone")]
        public string Relative1TelNumber { get; set; } = "";
        [Display(Name = "Address")]
        public string Relative1Address { get; set; } = "";

        [Display(Name = "Name")]
        public string Relative2Name { get; set; } = "";
        [Display(Name = "Relationship")]
        public string Relative2Relationship { get; set; } = "";
        [Display(Name = "Telephone")]
        public string Relative2TelNumber { get; set; } = "";
        [Display(Name = "Address")]
        public string Relative2Address { get; set; } = "";

        // ---- Loan details ----
        [Required, Range(1, 10000000)]
        [Display(Name = "Loan amount required (R)")]
        public double RequestedAmount { get; set; }

        public List<string> ReasonsForLoan { get; set; } = new();

        [Display(Name = "Other reason")]
        public string OtherReason { get; set; } = "";

        // ---- Declaration ----
        [Display(Name = "Applicant signature / typed name")]
        public string ApplicantSignature { get; set; } = "";

        [Display(Name = "Application date")]
        public DateTime ApplicationDate { get; set; } = DateTime.Today;
    }
}