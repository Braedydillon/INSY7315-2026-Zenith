using System.ComponentModel.DataAnnotations;

namespace INSY7315_Prototype.Models
{
    public class ApplicationForm
    {
            [Required, Range(1, 10000000)]
            [Display(Name = "Loan amount required (R)")]
            public double RequestedAmount { get; set; }

            [Required, StringLength(150)]
            [Display(Name = "Full name and surname")]
            public string FullNameAndSurname { get; set; } = "";

            [Required, RegularExpression(@"^\d{13}$", ErrorMessage = "ID number must be 13 digits.")]
            [Display(Name = "ID number")]
            public string IdNumber { get; set; } = "";

            [Required, RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "Enter a valid cell number.")]
            [Display(Name = "Cell number")]
            public string CellNo { get; set; } = "";

            [Required(ErrorMessage = "Please enter your current physical address.")]
            [Display(Name = "Current physical address")]
            public string CurrentPhysicalAddress { get; set; } = "";

            [Required(ErrorMessage = "Please enter your company name.")]
            [Display(Name = "Company name")]
            public string CompanyName { get; set; } = "";

            [Required(ErrorMessage = "Please enter your occupation.")]
            public string Occupation { get; set; } = "";
    }
}
