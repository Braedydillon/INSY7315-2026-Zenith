namespace INSY7315_Prototype.Models
{
    public class LoanApiResponse : LoanApplyRequest
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string Status { get; set; } = "Pending";
    }
}