using INSY7315_Prototype.ViewModels;

namespace INSY7315_Prototype.Models
{
    public static class LoanMapper
    {
        public static StaffDetailsViewModel ToDetails(LoanApiResponse l) => new()
        {
            FullNameAndSurname = l.ClientDetails?.FullNameAndSurname ?? "",
            IdNumber = l.ClientDetails?.IdNumber ?? "",
            CellNo = l.ClientDetails?.CellNo ?? "",
            HomeTelNo = l.HomeTelNo ?? "",
            MaritalStatus = l.MarriedOrUnmarried ?? "",
            CurrentPhysicalAddress = l.CurrentPhysicalAddress ?? "",
            PostalAddress = l.PostalAddress ?? "",
            ResidenceDuration = $"{l.ResidenceYears ?? 0} years {l.ResidenceMonths ?? 0} months",
            CompanyName = l.CompanyName ?? "",
            Occupation = l.Occupation ?? "",
            WorkTelephone = l.WorkTelephone ?? "",
            BankName = l.BankName ?? "",
            AccountType = l.AccountType ?? "",
            AccountNumber = l.AccountNumber ?? "",
            SpouseNameAndSurname = l.SpouseNameAndSurname ?? "",
            Relative1Name = l.Relative1Name ?? "",
            Relative1TelNumber = l.Relative1TelNumber ?? ""
        };
    }
}