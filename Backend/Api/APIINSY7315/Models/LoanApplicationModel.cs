using Google.Cloud.Firestore;

namespace APIINSY7315.Models

{
    [FirestoreData]
    public class LoanApplicationModel
    {
        [FirestoreProperty]
        public string ApplicationId { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Status { get; set; } = "Submitted";

        [FirestoreProperty]
        public Timestamp ApplicationDate { get; set; } = Timestamp.GetCurrentTimestamp();

        [FirestoreProperty]
        public decimal RequestedAmount { get; set; }

        [FirestoreProperty]
        public string ReasonForLoan { get; set; }

        [FirestoreProperty]
        public ClientDetailsModel ClientDetails { get; set; }
    }

    [FirestoreData]
    public class ClientDetailsModel
    {
        [FirestoreProperty]
        public string FullNameAndSurname { get; set; }

        [FirestoreProperty]
        public string IdNumber { get; set; }

        [FirestoreProperty]
        public string CellNo { get; set; }
    }
}

