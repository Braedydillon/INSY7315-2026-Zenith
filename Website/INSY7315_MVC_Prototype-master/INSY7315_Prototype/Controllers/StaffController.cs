using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using INSY7315_Prototype.Models;
using INSY7315_Prototype.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "staff")]   
public class StaffController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<StaffController> _logger;
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public StaffController(IHttpClientFactory factory, ILogger<StaffController> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    private HttpClient Api()
    {
        var client = _factory.CreateClient("LoanApi");
        var token = User.FindFirst("IdToken")?.Value;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new StaffDashboardViewModel();

        var res = await Api().GetAsync("api/LoansApi");
        if (!res.IsSuccessStatusCode)
        {
            TempData["Error"] = $"Could not load applications ({(int)res.StatusCode}).";
            return View(vm);
        }

        var all = JsonSerializer.Deserialize<List<LoanApiResponse>>(
            await res.Content.ReadAsStringAsync(), _json) ?? new();

        bool Is(LoanApiResponse l, string s) =>
            string.Equals(l.Status, s, StringComparison.OrdinalIgnoreCase);

        vm.PendingCount = all.Count(l => Is(l, "Pending"));
        vm.VerifiedCount = all.Count(l => Is(l, "Verified"));
        vm.RejectedCount = all.Count(l => Is(l, "Rejected"));

        vm.Applications = all
            .Where(l => Is(l, "Pending"))
            .OrderByDescending(l => l.ApplicantFormDate)
            .Select(l => new StaffLoanApplicationViewModel
            {
                ApplicationId = l.Id ?? "",
                ClientName = l.ClientDetails?.FullNameAndSurname ?? "",
                RequestedAmount = l.RequestedAmount,
                DateApplied = l.ApplicantFormDate,
                Status = l.Status
            }).ToList();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Verify(string id)
    {
        var vm = await BuildVerifyModel(id);
        if (vm == null) return NotFound();
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(StaffVerifyLoanViewModel model)
    {
        bool verify = model.Decision == "Verify";

        if (verify && !(model.IdVerified && model.AddressVerified &&
                        model.EmploymentVerified && model.BankVerified))
        {
            return await ReturnWithError(model, "Tick every verification check before sending to the manager.");
        }

        var newStatus = verify ? "Verified" : "Rejected";
        var body = new StringContent(
            JsonSerializer.Serialize(new { status = newStatus }, _json),
            Encoding.UTF8, "application/json");

        var res = await Api().PutAsync($"api/LoansApi/{model.ApplicationId}/status", body);

        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("Status update failed: {Status} {Body}",
                (int)res.StatusCode, await res.Content.ReadAsStringAsync());
            return await ReturnWithError(model, "We couldn't update the application. Please try again.");
        }

        TempData["Success"] = verify ? "Application sent to the manager." : "Application rejected.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> ReturnWithError(StaffVerifyLoanViewModel posted, string message)
    {
        var fresh = await BuildVerifyModel(posted.ApplicationId) ?? posted;
        fresh.IdVerified = posted.IdVerified;
        fresh.AddressVerified = posted.AddressVerified;
        fresh.EmploymentVerified = posted.EmploymentVerified;
        fresh.BankVerified = posted.BankVerified;
        fresh.StaffNotes = posted.StaffNotes;

        ModelState.AddModelError("", message);
        return View(fresh);
    }

    private async Task<StaffVerifyLoanViewModel?> BuildVerifyModel(string id)
    {
        var res = await Api().GetAsync($"api/LoansApi/{id}");
        if (!res.IsSuccessStatusCode) return null;

        var l = JsonSerializer.Deserialize<LoanApiResponse>(
            await res.Content.ReadAsStringAsync(), _json);
        if (l == null) return null;

        return new StaffVerifyLoanViewModel
        {
            ApplicationId = id,
            RequestedAmount = l.RequestedAmount,
            ReasonForLoan = l.ReasonForLoan,
            DateApplied = l.ApplicantFormDate,
            Status = l.Status,
            Client = new ClientDetailsViewModel
            {
                FullNameAndSurname = l.ClientDetails?.FullNameAndSurname ?? "",
                IdNumber = l.ClientDetails?.IdNumber ?? "",
                CellNo = l.ClientDetails?.CellNo ?? "",
                HomeTelNo = l.HomeTelNo ?? "",
                MaritalStatus = l.MarriedOrUnmarried ?? "",
                CurrentPhysicalAddress = l.CurrentPhysicalAddress,
                PostalAddress = l.PostalAddress ?? "",
                ResidenceDuration = $"{l.ResidenceYears} years {l.ResidenceMonths} months",
                CompanyName = l.CompanyName,
                Occupation = l.Occupation,
                WorkTelephone = l.WorkTelephone ?? "",
                BankName = l.BankName,
                AccountType = l.AccountType,
                AccountNumber = l.AccountNumber,
                SpouseNameAndSurname = l.SpouseNameAndSurname ?? "",
                Relative1Name = l.Relative1Name ?? "",
                Relative1TelNumber = l.Relative1TelNumber ?? ""
            }
        };
    }
}