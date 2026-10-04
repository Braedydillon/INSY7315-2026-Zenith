using INSY7315_Prototype.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
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

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

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

                PendingApplications = applications.Count(a => 
                    a.Status.Equals("Submitted",StringComparison.OrdinalIgnoreCase) ||
                    a.Status.Equals("UnderReview",StringComparison.OrdinalIgnoreCase)),

                ApprovedApplications = applications.Count(a => 
                    a.Status.Equals("Approved",StringComparison.OrdinalIgnoreCase)),

                RejectedApplications = applications.Count(a => 
                    a.Status.Equals("Rejected",StringComparison.OrdinalIgnoreCase)),

                RecentApplications = applications.Take(4).ToList()
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

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(ManagerDecisionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ApplicationId))
            {
                return BadRequest();
            }

            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            string status;

            if (model.Decision == "approve")
            {
                status = "Approved";
            }
            else if (model.Decision == "decline")
            {
                status = "Rejected";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid application decision.";

                return RedirectToAction("Review", new { id = model.ApplicationId });
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                status = status,
                note = model.Note
            };

            var response = await client.PutAsJsonAsync($"api/LoansApi/{model.ApplicationId}/status", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] = $"Unable to update application: {error}";

                return RedirectToAction("Review", new { id = model.ApplicationId });
            }

            TempData["SuccessMessage"] = status == "Approved" ? "Application approved successfully." : "Application declined successfully.";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Applications()
        {
            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

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
        public async Task<IActionResult> CompleteReview(string ApplicationId,string? Note)
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

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                status = "UnderReview",
                note = Note ?? string.Empty
            };

            var response = await client.PutAsJsonAsync($"api/LoansApi/{ApplicationId}/status",request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] = $"Unable to verify application: {error}";

                return RedirectToAction("Review",new { id = ApplicationId });
            }

            TempData["SuccessMessage"] = "Application verified successfully and sent for loan approval.";

            return RedirectToAction("Applications");
        }

        public async Task<IActionResult> Approvals()
        {
            var token = User.FindFirst("IdToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

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

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

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

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://apiinsy7315-latest.onrender.com/");

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

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(
                "https://apiinsy7315-latest.onrender.com/");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/LoansApi");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to load loan applications.";

                return View(
                    new List<ManagerLoanApplicationViewModel>());
            }

            var applications =
                await response.Content
                    .ReadFromJsonAsync<List<ManagerLoanApplicationViewModel>>()
                ?? new List<ManagerLoanApplicationViewModel>();

            return View(applications);
        }
    }
}
