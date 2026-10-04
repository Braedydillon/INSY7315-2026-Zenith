namespace INSY7315_Prototype.Models
{
    public static class LoanRules
    {
        public const decimal ManagerThreshold = 7000m;

        // R7,000 and above needs the manager
        public static bool RequiresManager(decimal amount) => amount >= ManagerThreshold;

        public static bool Is(string? status, string expected) =>
            string.Equals(status?.Trim(), expected, StringComparison.OrdinalIgnoreCase);

        // blank / unknown statuses count as pending
        public static bool IsPending(string? status)
        {
            var s = (status ?? "").Trim().ToLowerInvariant();
            return s is "" or "pending" or "submitted" or "under review" or "new";
        }
    }
}