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

    // ---------- All applications ----------
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

        vm.PendingCount = all.Count(l => LoanRules.IsPending(l.Status));
        vm.SentToManagerCount = all.Count(l => LoanRules.Is(l.Status, "Verified"));
        vm.ApprovedCount = all.Count(l => LoanRules.Is(l.Status, "Approved"));
        vm.DeclinedCount = all.Count(l => LoanRules.Is(l.Status, "Declined") || LoanRules.Is(l.Status, "Rejected"));

        vm.Applications = all
            .OrderByDescending(l => l.ApplicantFormDate)
            .Select(l => new StaffLoanApplicationViewModel
            {
                ApplicationId = l.Id ?? "",
                ClientName = l.ClientDetails?.FullNameAndSurname ?? "",
                RequestedAmount = l.RequestedAmount,
                DateApplied = l.ApplicantFormDate,
                Status = string.IsNullOrWhiteSpace(l.Status) ? "Pending" : l.Status,
                RequiresManager = LoanRules.RequiresManager(l.RequestedAmount),
                IsPending = LoanRules.IsPending(l.Status)
            })
            .ToList();

        return View(vm);
    }

    // ---------- Review one application ----------
    [HttpGet]
    public async Task<IActionResult> Verify(string id)
    {
        var vm = await BuildVerifyModel(id);
        if (vm == null) return NotFound();

        if (!LoanRules.IsPending(vm.Status))
        {
            TempData["Error"] = "This application has already been processed.";
            return RedirectToAction(nameof(Index));
        }
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(StaffVerifyLoanViewModel model)
    {
        // Re-read the loan from the API: the amount and status are never trusted from the form
        var current = await BuildVerifyModel(model.ApplicationId);
        if (current == null) return NotFound();

        if (!LoanRules.IsPending(current.Status))
        {
            TempData["Error"] = "This application has already been processed.";
            return RedirectToAction(nameof(Index));
        }

        bool big = current.RequiresManager;

        // Which decisions staff may make depends on the amount
        string? newStatus = (model.Decision, big) switch
        {
            ("Approve", false) => "Approved",
            ("Decline", false) => "Declined",
            ("Verify", true) => "Verified",
            ("Reject", true) => "Rejected",
            _ => null
        };

        if (newStatus == null)
        {
            TempData["Error"] = big
                ? $"Loans of R{LoanRules.ManagerThreshold:N0} and above must be verified and sent to the manager."
                : $"Loans under R{LoanRules.ManagerThreshold:N0} are approved or declined by staff.";
            return RedirectToAction(nameof(Index));
        }

        bool positive = model.Decision is "Approve" or "Verify";
        if (positive && !(model.IdVerified && model.AddressVerified &&
                          model.EmploymentVerified && model.BankVerified))
        {
            return await ReturnWithError(model, "Tick every verification check before continuing.");
        }

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

        TempData["Success"] = newStatus switch
        {
            "Approved" => "Loan approved.",
            "Declined" => "Loan declined.",
            "Verified" => "Application verified and sent to the manager.",
            _ => "Application rejected."
        };

        return RedirectToAction(nameof(Index));
    }

    // ---------- Helpers ----------
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

        var json = await res.Content.ReadAsStringAsync();

        var all = JsonSerializer.Deserialize<List<LoanApiResponse>>(json, _json) ?? new();
        var l = all.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
        if (l == null) return null;

        return new StaffVerifyLoanViewModel
        {
            ApplicationId = l.Id ?? "",
            RequestedAmount = l.RequestedAmount,
            ReasonForLoan = l.ReasonForLoan ?? "",
            DateApplied = l.ApplicantFormDate,
            Status = l.Status ?? "",
            RequiresManager = LoanRules.RequiresManager(l.RequestedAmount),
            Client = LoanMapper.ToDetails(l)
        };
    }
}