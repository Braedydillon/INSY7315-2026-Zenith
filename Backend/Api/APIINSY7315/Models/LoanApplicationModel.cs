using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace APIINSY7315.Models
{
    // ============================================================
    // FIRESTORE MODEL
    // ============================================================

    [FirestoreData]
    public class LoanApplicationModel
    {
        [FirestoreProperty]
        public string ApplicationId { get; set; } = "";

        [FirestoreProperty]
        public string UserId { get; set; } = "";

        [FirestoreProperty]
        public string UserEmail { get; set; } = "";

        [FirestoreProperty]
        public string Status { get; set; } = LoanStatus.Submitted;

        [FirestoreProperty]
        public Timestamp ApplicationDate { get; set; }

        [FirestoreProperty]
        public double RequestedAmount { get; set; }

        [FirestoreProperty]
        public string ReasonForLoan { get; set; } = "";

        [FirestoreProperty]
        public ClientDetailsModel ClientDetails { get; set; } = new();

        [FirestoreProperty]
        public string HomeTelNo { get; set; } = "";

        [FirestoreProperty]
        public string MarriedOrUnmarried { get; set; } = "";

        [FirestoreProperty]
        public string MarriageCommunity { get; set; } = "";

        [FirestoreProperty]
        public string PreviouslyDivorced { get; set; } = "";

        [FirestoreProperty]
        public string DivorceYear { get; set; } = "";

        [FirestoreProperty]
        public string DivorceCommunity { get; set; } = "";

        [FirestoreProperty]
        public string CurrentPhysicalAddress { get; set; } = "";

        [FirestoreProperty]
        public string PostalAddress { get; set; } = "";

        [FirestoreProperty]
        public string ParentsAddress { get; set; } = "";

        [FirestoreProperty]
        public int ResidenceYears { get; set; }

        [FirestoreProperty]
        public int ResidenceMonths { get; set; }

        [FirestoreProperty]
        public string CompanyName { get; set; } = "";

        [FirestoreProperty]
        public string WorkTelephone { get; set; } = "";

        [FirestoreProperty]
        public string Occupation { get; set; } = "";

        [FirestoreProperty]
        public string WorkAddress { get; set; } = "";

        [FirestoreProperty]
        public string BankName { get; set; } = "";

        [FirestoreProperty]
        public string AccountType { get; set; } = "";

        [FirestoreProperty]
        public string AccountNumber { get; set; } = "";

        [FirestoreProperty]
        public string BranchName { get; set; } = "";

        [FirestoreProperty]
        public string AccountName { get; set; } = "";

        [FirestoreProperty]
        public string BranchCode { get; set; } = "";

        [FirestoreProperty]
        public string SpouseNameAndSurname { get; set; } = "";

        [FirestoreProperty]
        public string SpouseIdNumber { get; set; } = "";

        [FirestoreProperty]
        public string SpouseTelNumber { get; set; } = "";

        [FirestoreProperty]
        public string SpouseEmployerName { get; set; } = "";

        [FirestoreProperty]
        public string SpouseEmployerAddress { get; set; } = "";

        [FirestoreProperty]
        public string SpouseEmployerTelNumber { get; set; } = "";

        [FirestoreProperty]
        public string Relative1Name { get; set; } = "";

        [FirestoreProperty]
        public string Relative1Relationship { get; set; } = "";

        [FirestoreProperty]
        public string Relative1TelNumber { get; set; } = "";

        [FirestoreProperty]
        public string Relative1Address { get; set; } = "";

        [FirestoreProperty]
        public string Relative2Name { get; set; } = "";

        [FirestoreProperty]
        public string Relative2Relationship { get; set; } = "";

        [FirestoreProperty]
        public string Relative2TelNumber { get; set; } = "";

        [FirestoreProperty]
        public string Relative2Address { get; set; } = "";

        [FirestoreProperty]
        public List<string> ReasonsForLoan { get; set; } = new();

        [FirestoreProperty]
        public string OtherReason { get; set; } = "";

        [FirestoreProperty]
        public string ApplicantSignature { get; set; } = "";

        // IMPORTANT:
        // Firestore stores this as Timestamp, not DateTime.
        [FirestoreProperty]
        public Timestamp? ApplicantFormDate { get; set; }

        [FirestoreProperty]
        public string ReviewedBy { get; set; } = "";

        [FirestoreProperty]
        public Timestamp? ReviewedAt { get; set; }

        [FirestoreProperty]
        public string ReviewNote { get; set; } = "";
    }


    // ============================================================
    // CLIENT DETAILS
    // ============================================================

    [FirestoreData]
    public class ClientDetailsModel
    {
        [FirestoreProperty]
        public string FullNameAndSurname { get; set; } = "";

        [FirestoreProperty]
        public string IdNumber { get; set; } = "";

        [FirestoreProperty]
        public string CellNo { get; set; } = "";
    }


    // ============================================================
    // LOAN STATUS
    // ============================================================

    public static class LoanStatus
    {
        public const string Submitted = "Submitted";
        public const string UnderReview = "UnderReview";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";

        public static readonly string[] All =
        {
            Submitted,
            UnderReview,
            Approved,
            Rejected
        };
    }


    // ============================================================
    // SUBMIT REQUEST
    // ============================================================

    public class SubmitLoanRequest
    {
        [Range(1, 10_000_000)]
        public double RequestedAmount { get; set; }

        [Required]
        [StringLength(1000)]
        public string ReasonForLoan { get; set; } = "";

        [Required]
        public ClientDetailsRequest ClientDetails { get; set; } = new();

        public string HomeTelNo { get; set; } = "";

        public string MarriedOrUnmarried { get; set; } = "";

        public string MarriageCommunity { get; set; } = "";

        public string PreviouslyDivorced { get; set; } = "";

        public string DivorceYear { get; set; } = "";

        public string DivorceCommunity { get; set; } = "";

        public string CurrentPhysicalAddress { get; set; } = "";

        public string PostalAddress { get; set; } = "";

        public string ParentsAddress { get; set; } = "";

        public int ResidenceYears { get; set; }

        public int ResidenceMonths { get; set; }

        public string CompanyName { get; set; } = "";

        public string WorkTelephone { get; set; } = "";

        public string Occupation { get; set; } = "";

        public string WorkAddress { get; set; } = "";

        public string BankName { get; set; } = "";

        public string AccountType { get; set; } = "";

        public string AccountNumber { get; set; } = "";

        public string BranchName { get; set; } = "";

        public string AccountName { get; set; } = "";

        public string BranchCode { get; set; } = "";

        public string SpouseNameAndSurname { get; set; } = "";

        public string SpouseIdNumber { get; set; } = "";

        public string SpouseTelNumber { get; set; } = "";

        public string SpouseEmployerName { get; set; } = "";

        public string SpouseEmployerAddress { get; set; } = "";

        public string SpouseEmployerTelNumber { get; set; } = "";

        public string Relative1Name { get; set; } = "";

        public string Relative1Relationship { get; set; } = "";

        public string Relative1TelNumber { get; set; } = "";

        public string Relative1Address { get; set; } = "";

        public string Relative2Name { get; set; } = "";

        public string Relative2Relationship { get; set; } = "";

        public string Relative2TelNumber { get; set; } = "";

        public string Relative2Address { get; set; } = "";

        public List<string> ReasonsForLoan { get; set; } = new();

        public string OtherReason { get; set; } = "";

        public string ApplicantSignature { get; set; } = "";

        public DateTime? ApplicantFormDate { get; set; }
    }


    // ============================================================
    // CLIENT DETAILS REQUEST
    // ============================================================

    public class ClientDetailsRequest
    {
        [Required]
        [StringLength(150)]
        public string FullNameAndSurname { get; set; } = "";

        [Required]
        [RegularExpression(
            @"^\d{13}$",
            ErrorMessage = "ID number must be 13 digits.")]
        public string IdNumber { get; set; } = "";

        [Required]
        [RegularExpression(
            @"^\+?\d{9,15}$",
            ErrorMessage = "Invalid cell number.")]
        public string CellNo { get; set; } = "";
    }


    // ============================================================
    // UPDATE STATUS REQUEST
    // ============================================================

    public class UpdateStatusRequest
    {
        [Required]
        public string Status { get; set; } = "";

        [StringLength(1000)]
        public string? Note { get; set; }
    }


    // ============================================================
    // API RESPONSE DTO
    // ============================================================

    public class LoanApplicationDto
    {
        public string ApplicationId { get; set; } = "";

        public string UserId { get; set; } = "";

        public string UserEmail { get; set; } = "";

        public string Status { get; set; } = "";

        public DateTime ApplicationDate { get; set; }

        public double RequestedAmount { get; set; }

        public string ReasonForLoan { get; set; } = "";

        public ClientDetailsDto ClientDetails { get; set; } = new();

        public string HomeTelNo { get; set; } = "";

        public string MarriedOrUnmarried { get; set; } = "";

        public string MarriageCommunity { get; set; } = "";

        public string PreviouslyDivorced { get; set; } = "";

        public string DivorceYear { get; set; } = "";

        public string DivorceCommunity { get; set; } = "";

        public string CurrentPhysicalAddress { get; set; } = "";

        public string PostalAddress { get; set; } = "";

        public string ParentsAddress { get; set; } = "";

        public int ResidenceYears { get; set; }

        public int ResidenceMonths { get; set; }

        public string CompanyName { get; set; } = "";

        public string WorkTelephone { get; set; } = "";

        public string Occupation { get; set; } = "";

        public string WorkAddress { get; set; } = "";

        public string BankName { get; set; } = "";

        public string AccountType { get; set; } = "";

        public string AccountNumber { get; set; } = "";

        public string BranchName { get; set; } = "";

        public string AccountName { get; set; } = "";

        public string BranchCode { get; set; } = "";

        public string SpouseNameAndSurname { get; set; } = "";

        public string SpouseIdNumber { get; set; } = "";

        public string SpouseTelNumber { get; set; } = "";

        public string SpouseEmployerName { get; set; } = "";

        public string SpouseEmployerAddress { get; set; } = "";

        public string SpouseEmployerTelNumber { get; set; } = "";

        public string Relative1Name { get; set; } = "";

        public string Relative1Relationship { get; set; } = "";

        public string Relative1TelNumber { get; set; } = "";

        public string Relative1Address { get; set; } = "";

        public string Relative2Name { get; set; } = "";

        public string Relative2Relationship { get; set; } = "";

        public string Relative2TelNumber { get; set; } = "";

        public string Relative2Address { get; set; } = "";

        public List<string> ReasonsForLoan { get; set; } = new();

        public string OtherReason { get; set; } = "";

        public string ApplicantSignature { get; set; } = "";

        public DateTime? ApplicantFormDate { get; set; }

        public string ReviewedBy { get; set; } = "";

        public Timestamp? ReviewedAt { get; set; }

        public string ReviewNote { get; set; } = "";
    }


    // ============================================================
    // CLIENT DETAILS DTO
    // ============================================================

    public class ClientDetailsDto
    {
        public string FullNameAndSurname { get; set; } = "";

        public string IdNumber { get; set; } = "";

        public string CellNo { get; set; } = "";
    }
}