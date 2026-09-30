using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace APIINSY7315.Models
{
    // What is stored in Firestore.
    [FirestoreData]
    public class LoanApplicationModel
    {
        [FirestoreProperty] public string ApplicationId { get; set; } = Guid.NewGuid().ToString();
        [FirestoreProperty] public string UserId { get; set; } = "";          // owner (Firebase UID) - set by the server only
        [FirestoreProperty] public string? UserEmail { get; set; }
        [FirestoreProperty] public string Status { get; set; } = LoanStatus.Submitted;
        [FirestoreProperty] public Timestamp ApplicationDate { get; set; } = Timestamp.GetCurrentTimestamp();
        [FirestoreProperty] public double RequestedAmount { get; set; }
        [FirestoreProperty] public string ReasonForLoan { get; set; } = "";
        [FirestoreProperty] public ClientDetailsModel ClientDetails { get; set; } = new();

        // Review info (set by management/admin)
        [FirestoreProperty] public string? ReviewedBy { get; set; }
        [FirestoreProperty] public Timestamp? ReviewedAt { get; set; }
        [FirestoreProperty] public string? ReviewNote { get; set; }
    }

    [FirestoreData]
    public class ClientDetailsModel
    {
        [FirestoreProperty] public string FullNameAndSurname { get; set; } = "";
        [FirestoreProperty] public string IdNumber { get; set; } = "";
        [FirestoreProperty] public string CellNo { get; set; } = "";
    }

    public static class LoanStatus
    {
        public const string Submitted = "Submitted";
        public const string UnderReview = "UnderReview";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";

        public static readonly string[] All = { Submitted, UnderReview, Approved, Rejected };
    }

    // What clients are allowed to send. No UserId / Status / ApplicationId here on purpose.
    public class SubmitLoanRequest
    {
        [Range(1, 10_000_000)] public double RequestedAmount { get; set; }
        [Required, StringLength(1000)] public string ReasonForLoan { get; set; } = "";
        [Required] public ClientDetailsRequest ClientDetails { get; set; } = new();
    }

    public class ClientDetailsRequest
    {
        [Required, StringLength(150)] public string FullNameAndSurname { get; set; } = "";
        [Required, RegularExpression(@"^\d{13}$", ErrorMessage = "ID number must be 13 digits.")] public string IdNumber { get; set; } = "";
        [Required, RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "Invalid cell number.")] public string CellNo { get; set; } = "";
    }

    public class UpdateStatusRequest
    {
        [Required] public string Status { get; set; } = "";
        [StringLength(1000)] public string? Note { get; set; }
    }
}
