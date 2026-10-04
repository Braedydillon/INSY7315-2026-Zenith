using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace INSY7315_Prototype.Models
{
    public class Loan
    {
        [JsonPropertyName("applicationId")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("requestedAmount")]
        public decimal RequestedAmount { get; set; }

        [JsonPropertyName("reasonForLoan")]
        public string ReasonForLoan { get; set; } = string.Empty;

        [JsonPropertyName("applicantFormDate")]
        public DateTime ApplicantFormDate { get; set; }
    }

}
