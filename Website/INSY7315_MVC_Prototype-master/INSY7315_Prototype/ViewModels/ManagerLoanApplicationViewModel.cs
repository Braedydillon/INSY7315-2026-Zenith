namespace INSY7315_Prototype.ViewModels
{
    public class ManagerLoanApplicationViewModel
    {
        public string ApplicationId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public decimal RequestedAmount { get; set; }
        public string ReasonForLoan { get; set; } = string.Empty;

        public DateTime DateApplied { get; set; }
        public string Status { get; set; } = string.Empty;

        public ClientDetailsViewModel ClientDetails { get; set; } = new();


        // Personal details
        public string HomeTelNo { get; set; } = string.Empty;
        public string MarriedOrUnmarried { get; set; } = string.Empty;
        public string MarriageCommunity { get; set; } = string.Empty;
        public string PreviouslyDivorced { get; set; } = string.Empty;
        public string DivorceYear { get; set; } = string.Empty;
        public string DivorceCommunity { get; set; } = string.Empty;

        // Address
        public string CurrentPhysicalAddress { get; set; } = string.Empty;
        public string PostalAddress { get; set; } = string.Empty;
        public string ParentsAddress { get; set; } = string.Empty;

        public int ResidenceYears { get; set; }
        public int ResidenceMonths { get; set; }


        // Employment
        public string CompanyName { get; set; } = string.Empty;
        public string WorkTelephone { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string WorkAddress { get; set; } = string.Empty;


        // Banking
        public string BankName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;


        // Spouse
        public string SpouseNameAndSurname { get; set; } = string.Empty;
        public string SpouseIdNumber { get; set; } = string.Empty;
        public string SpouseTelNumber { get; set; } = string.Empty;
        public string SpouseEmployerName { get; set; } = string.Empty;
        public string SpouseEmployerAddress { get; set; } = string.Empty;
        public string SpouseEmployerTelNumber { get; set; } = string.Empty;


        // Relative 1
        public string Relative1Name { get; set; } = string.Empty;
        public string Relative1Relationship { get; set; } = string.Empty;
        public string Relative1TelNumber { get; set; } = string.Empty;
        public string Relative1Address { get; set; } = string.Empty;


        // Relative 2
        public string Relative2Name { get; set; } = string.Empty;
        public string Relative2Relationship { get; set; } = string.Empty;
        public string Relative2TelNumber { get; set; } = string.Empty;
        public string Relative2Address { get; set; } = string.Empty;


        // Loan reason
        public List<string> ReasonsForLoan { get; set; } = new();
        public string OtherReason { get; set; } = string.Empty;


        // Form
        public string ApplicantSignature { get; set; } = string.Empty;
        public DateTime? ApplicantFormDate { get; set; }
    }

    public class ClientDetailsViewModel
    {
        public string FullNameAndSurname { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string CellNo { get; set; } = string.Empty;
    }
}
