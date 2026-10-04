using System.Net.Http.Headers;
using INSY7315_Prototype.Models;
using INSY7315_Prototype.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{
    [Authorize(Roles = "staff")]
    public class StaffController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StaffController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private static string ManagerOnlyMessage =>
            $"Loans above R{LoanRules.StaffLimit:N0} are handled by the manager. This application has been sent to the manager.";

        // ---------- helpers ----------
        private HttpClient? CreateClient()
        {
            var token = User.FindFirst("IdToken")?.Value;
            if (string.IsNullOrEmpty(token)) return null;

            var client = _httpClientFactory.CreateClient("LoanApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task<List<ManagerLoanApplicationViewModel>?> GetAllLoans(HttpClient client)
        {
            var response = await client.GetAsync("api/LoansApi");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<List<ManagerLoanApplicationViewModel>>()
                   ?? new List<ManagerLoanApplicationViewModel>();
        }

        private async Task<ManagerLoanApplicationViewModel?> GetLoan(HttpClient client, string id)
        {
            var response = await client.GetAsync($"api/LoansApi/{id}");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<ManagerLoanApplicationViewModel>();
        }

        // returns null on success, or the error text
        private async Task<string?> UpdateStatus(HttpClient client, string id, string status, string? note)
        {
            var response = await client.PutAsJsonAsync(
                $"api/LoansApi/{id}/status",
                new { status = status, note = note ?? string.Empty });

            if (response.IsSuccessStatusCode) return null;

            var error = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(error) ? ((int)response.StatusCode).ToString() : error;
        }

        // ---------- dashboard ----------
        public async Task<IActionResult> Index()
        {
            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            var all = await GetAllLoans(client);
            if (all == null)
            {
                ViewBag.ErrorMessage = "Unable to load loan applications.";
                return View(new StaffDashboardViewModel());
            }

            // Only loans staff are allowed to handle are counted as theirs
            var mine = all.Where(a => LoanRules.StaffCanHandle(a.RequestedAmount)).ToList();

            var model = new StaffDashboardViewModel
            {
                TotalApplications = mine.Count,
                AwaitingVerification = mine.Count(a => LoanRules.Is(a.Status, LoanStatus.Submitted)),
                AwaitingApproval = mine.Count(a => LoanRules.Is(a.Status, LoanStatus.UnderReview)),
                ApprovedApplications = mine.Count(a => LoanRules.Is(a.Status, LoanStatus.Approved)),
                RejectedApplications = mine.Count(a => LoanRules.Is(a.Status, LoanStatus.Rejected)),
                SentToManager = all.Count(a => LoanRules.RequiresManager(a.RequestedAmount)),
                RecentApplications = mine.OrderByDescending(a => a.DateApplied).Take(6).ToList()
            };
            model.PendingApplications = model.AwaitingVerification + model.AwaitingApproval;

            return View(model);
        }

        // ---------- verify ----------
        public async Task<IActionResult> Applications()
        {
            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            var all = await GetAllLoans(client);
            if (all == null)
            {
                ViewBag.ErrorMessage = "Unable to load loan applications.";
                return View(new List<ManagerLoanApplicationViewModel>());
            }

            var toVerify = all
                .Where(a => LoanRules.StaffCanHandle(a.RequestedAmount)
                         && LoanRules.Is(a.Status, LoanStatus.Submitted))
                .OrderByDescending(a => a.DateApplied)
                .ToList();

            return View(toVerify);
        }

        public async Task<IActionResult> Review(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            var application = await GetLoan(client, id);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Unable to load the application.";
                return RedirectToAction(nameof(Index));
            }

            if (LoanRules.RequiresManager(application.RequestedAmount))
            {
                TempData["ErrorMessage"] = ManagerOnlyMessage;
                return RedirectToAction(nameof(Index));
            }

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteReview(string ApplicationId, string VerificationResult, string? Note)
        {
            if (string.IsNullOrWhiteSpace(ApplicationId)) return BadRequest();

            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            // Re-read the loan from the API; the amount and status are never trusted from the form
            var application = await GetLoan(client, ApplicationId);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Unable to load the application.";
                return RedirectToAction(nameof(Applications));
            }

            if (LoanRules.RequiresManager(application.RequestedAmount))
            {
                TempData["ErrorMessage"] = ManagerOnlyMessage;
                return RedirectToAction(nameof(Index));
            }

            if (!LoanRules.Is(application.Status, LoanStatus.Submitted))
            {
                TempData["ErrorMessage"] = "This application has already been verified.";
                return RedirectToAction(nameof(Applications));
            }

            string status;
            if (VerificationResult == "successful") status = LoanStatus.UnderReview;
            else if (VerificationResult == "unsuccessful") status = LoanStatus.Rejected;
            else
            {
                TempData["ErrorMessage"] = "Please select a valid verification result.";
                return RedirectToAction(nameof(Review), new { id = ApplicationId });
            }

            var error = await UpdateStatus(client, ApplicationId, status, Note);
            if (error != null)
            {
                TempData["ErrorMessage"] = $"Unable to update application: {error}";
                return RedirectToAction(nameof(Review), new { id = ApplicationId });
            }

            TempData["SuccessMessage"] = status == LoanStatus.UnderReview
                ? "Application verified successfully and is ready for loan approval."
                : "Application verification was unsuccessful and the application was rejected.";

            return RedirectToAction(nameof(Applications));
        }

        // ---------- approve ----------
        public async Task<IActionResult> Approvals()
        {
            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            var all = await GetAllLoans(client);
            if (all == null)
            {
                ViewBag.ErrorMessage = "Unable to load loans awaiting approval.";
                return View(new List<ManagerLoanApplicationViewModel>());
            }

            var awaiting = all
                .Where(a => LoanRules.StaffCanHandle(a.RequestedAmount)
                         && LoanRules.Is(a.Status, LoanStatus.UnderReview))
                .OrderByDescending(a => a.DateApplied)
                .ToList();

            return View(awaiting);
        }

        public async Task<IActionResult> Approval(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            var application = await GetLoan(client, id);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Unable to load the loan application.";
                return RedirectToAction(nameof(Approvals));
            }

            if (LoanRules.RequiresManager(application.RequestedAmount))
            {
                TempData["ErrorMessage"] = ManagerOnlyMessage;
                return RedirectToAction(nameof(Index));
            }

            if (!LoanRules.Is(application.Status, LoanStatus.UnderReview))
            {
                TempData["ErrorMessage"] = "This application is not waiting for approval.";
                return RedirectToAction(nameof(Approvals));
            }

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoanDecision(string ApplicationId, string Decision, string? Note)
        {
            if (string.IsNullOrWhiteSpace(ApplicationId)) return BadRequest();

            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            var application = await GetLoan(client, ApplicationId);
            if (application == null)
            {
                TempData["ErrorMessage"] = "Unable to load the loan application.";
                return RedirectToAction(nameof(Approvals));
            }

            if (LoanRules.RequiresManager(application.RequestedAmount))
            {
                TempData["ErrorMessage"] = ManagerOnlyMessage;
                return RedirectToAction(nameof(Index));
            }

            if (!LoanRules.Is(application.Status, LoanStatus.UnderReview))
            {
                TempData["ErrorMessage"] = "This application is not waiting for approval.";
                return RedirectToAction(nameof(Approvals));
            }

            string status;
            if (Decision == "approve") status = LoanStatus.Approved;
            else if (Decision == "decline") status = LoanStatus.Rejected;
            else
            {
                TempData["ErrorMessage"] = "Invalid loan decision.";
                return RedirectToAction(nameof(Approval), new { id = ApplicationId });
            }

            var error = await UpdateStatus(client, ApplicationId, status, Note);
            if (error != null)
            {
                TempData["ErrorMessage"] = $"Unable to update loan: {error}";
                return RedirectToAction(nameof(Approval), new { id = ApplicationId });
            }

            TempData["SuccessMessage"] = status == LoanStatus.Approved
                ? "Loan approved successfully."
                : "Loan declined successfully.";

            return RedirectToAction(nameof(Approvals));
        }

        // ---------- everything ----------
        public async Task<IActionResult> AllApplications()
        {
            var client = CreateClient();
            if (client == null) return RedirectToAction("Login", "Account");

            var all = await GetAllLoans(client);
            if (all == null)
            {
                ViewBag.ErrorMessage = "Unable to load loan applications.";
                return View(new List<ManagerLoanApplicationViewModel>());
            }

            return View(all.OrderByDescending(a => a.DateApplied).ToList());
        }
    }
}