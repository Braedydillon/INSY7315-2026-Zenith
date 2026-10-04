namespace INSY7315_Prototype.Models
{
    public static class LoanStatus
    {
        public const string Submitted = "Submitted";
        public const string UnderReview = "UnderReview";   // verified, waiting for approval
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }

    public static class LoanRules
    {
        // Staff can handle loans up to and including this amount
        public const decimal StaffLimit = 7000m;

        public static bool StaffCanHandle(decimal amount) => amount <= StaffLimit;
        public static bool RequiresManager(decimal amount) => amount > StaffLimit;

        public static bool Is(string? status, string expected) =>
            string.Equals(status?.Trim(), expected, StringComparison.OrdinalIgnoreCase);
    }
}