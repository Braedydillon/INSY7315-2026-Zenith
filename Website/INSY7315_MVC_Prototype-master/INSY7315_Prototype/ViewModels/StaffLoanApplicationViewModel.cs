using System.ComponentModel.DataAnnotations;

namespace INSY7315_Prototype.ViewModels
{
    // One row in a queue (staff and manager both use it)
    public class StaffLoanApplicationViewModel
    {
        public string ApplicationId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal RequestedAmount { get; set; }
        public DateTime? DateApplied { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool RequiresManager { get; set; }
        public bool IsPending { get; set; }
    }

    public class StaffVerifyLoanViewModel
    {
        public string ApplicationId { get; set; } = string.Empty;
        public decimal RequestedAmount { get; set; }
        public string ReasonForLoan { get; set; } = string.Empty;
        public DateTime? DateApplied { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool RequiresManager { get; set; }

        public StaffDetailsViewModel Client { get; set; } = new();

        [Display(Name = "ID number verified")]
        public bool IdVerified { get; set; }
        [Display(Name = "Proof of address checked")]
        public bool AddressVerified { get; set; }
        [Display(Name = "Employment confirmed")]
        public bool EmploymentVerified { get; set; }
        [Display(Name = "Bank details confirmed")]
        public bool BankVerified { get; set; }

        [StringLength(500), Display(Name = "Staff notes")]
        public string? StaffNotes { get; set; }

        // "Approve" / "Decline" (under R7,000)  or  "Verify" / "Reject" (R7,000+)
        public string Decision { get; set; } = string.Empty;
    }

    public class StaffDetailsViewModel
    {
        public string FullNameAndSurname { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string CellNo { get; set; } = string.Empty;
        public string HomeTelNo { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string CurrentPhysicalAddress { get; set; } = string.Empty;
        public string PostalAddress { get; set; } = string.Empty;
        public string ResidenceDuration { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string WorkTelephone { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string SpouseNameAndSurname { get; set; } = string.Empty;
        public string Relative1Name { get; set; } = string.Empty;
        public string Relative1TelNumber { get; set; } = string.Empty;
    }
}