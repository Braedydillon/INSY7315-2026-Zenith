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
    }
}
