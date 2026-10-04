using INSY7315_Prototype.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace INSY7315_Prototype.Controllers
{
    [Authorize(Roles = "admin")]
    public class ManagerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ManagerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login", "Account");
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/LoansApi");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load loan applications";

                return View(new ManagerDashboardViewModel());
            }

            var applications = await response.Content.ReadFromJsonAsync<List<ManagerLoanApplicationViewModel>>() ?? new List<ManagerLoanApplicationViewModel>();

            var model = new ManagerDashboardViewModel
            {
                TotalApplications = applications.Count,

                AwaitingVerification = applications.Count(a => 
                    a.Status.Equals("Submitted", StringComparison.OrdinalIgnoreCase)),

                AwaitingApproval = applications.Count(a =>
                    a.Status.Equals("UnderReview", StringComparison.OrdinalIgnoreCase)),

                PendingApplications = applications.Count(a => 
                    a.Status.Equals("Submitted",StringComparison.OrdinalIgnoreCase) ||
                    a.Status.Equals("UnderReview",StringComparison.OrdinalIgnoreCase)),

                ApprovedApplications = applications.Count(a => 
                    a.Status.Equals("Approved",StringComparison.OrdinalIgnoreCase)),

                RejectedApplications = applications.Count(a => 
                    a.Status.Equals("Rejected",StringComparison.OrdinalIgnoreCase)),

                RecentApplications = applications.Take(6).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Review(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/LoansApi/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ViewBag.ErrorMessage = $"Unable to load application. {response.StatusCode}: {error}";

                return View(new ManagerLoanApplicationViewModel());
            }

            var application = await response.Content.ReadFromJsonAsync<ManagerLoanApplicationViewModel>();

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }


        public async Task<IActionResult> Applications()
        {
            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/LoansApi");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load loan applications.";

                return View(new List<ManagerLoanApplicationViewModel>());
            }

            var applications = await response.Content.ReadFromJsonAsync<List<ManagerLoanApplicationViewModel>>()?? new List<ManagerLoanApplicationViewModel>();

            return View(applications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteReview(string ApplicationId,string VerificationResult,string? Note)
        {
            if (string.IsNullOrWhiteSpace(ApplicationId))
            {
                return BadRequest();
            }

            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            string status;

            if (VerificationResult == "successful")
            {
                status = "UnderReview";
            }
            else if (VerificationResult == "unsuccessful")
            {
                status = "Rejected";
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a valid verification result.";

                return RedirectToAction("Review", new { id = ApplicationId });
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                status = status,
                note = Note ?? string.Empty
            };

            var response = await client.PutAsJsonAsync($"api/LoansApi/{ApplicationId}/status",request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] = $"Unable to update application: {error}";

                return RedirectToAction("Review", new { id = ApplicationId });
            }

            if (status == "UnderReview")
            {
                TempData["SuccessMessage"] = "Application verified successfully and sent for loan approval.";
            }
            else
            {
                TempData["SuccessMessage"] = "Application verification was unsuccessful and the application was rejected.";
            }

            return RedirectToAction("Applications");
        }


        public async Task<IActionResult> Approvals()
        {
            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/LoansApi");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load loans awaiting approval.";

                return View(new List<ManagerLoanApplicationViewModel>());
            }

            var applications = await response.Content.ReadFromJsonAsync<List<ManagerLoanApplicationViewModel>>()?? new List<ManagerLoanApplicationViewModel>();

            var awaitingApproval = applications.Where(x => x.Status.Equals("UnderReview", StringComparison.OrdinalIgnoreCase)).ToList();

            return View(awaitingApproval);
        }

        public async Task<IActionResult> Approval(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/LoansApi/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to load the loan application.";

                return RedirectToAction("Approvals");
            }

            var application = await response.Content.ReadFromJsonAsync<ManagerLoanApplicationViewModel>();

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoanDecision(string ApplicationId,string Decision,string? Note)
        {
            if (string.IsNullOrWhiteSpace(ApplicationId))
            {
                return BadRequest();
            }

            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            string status;

            if (Decision == "approve")
            {
                status = "Approved";
            }
            else if (Decision == "decline")
            {
                status = "Rejected";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid loan decision.";

                return RedirectToAction("Approval", new { id = ApplicationId });
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                status = status,
                note = Note ?? string.Empty
            };

            var response = await client.PutAsJsonAsync($"api/LoansApi/{ApplicationId}/status",request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] = $"Unable to update loan: {error}";

                return RedirectToAction("Approval",new { id = ApplicationId });
            }

            TempData["SuccessMessage"] = status == "Approved"? "Loan approved successfully.": "Loan declined successfully.";

            return RedirectToAction("Approvals");
        }

        public async Task<IActionResult> AllApplications()
        {
            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("LoanApi");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/LoansApi");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load loan applications.";

                return View(new List<ManagerLoanApplicationViewModel>());
            }

            var applications = await response.Content.ReadFromJsonAsync<List<ManagerLoanApplicationViewModel>>()?? new List<ManagerLoanApplicationViewModel>();

            return View(applications);
        }
    }
}
