using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using INSY7315_Prototype.Models;
using INSY7315_Prototype.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class LoanController : Controller
{
    private readonly IHttpClientFactory _factory;
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LoanController(IHttpClientFactory factory) => _factory = factory;

    private HttpClient Api()
    {
        var client = _factory.CreateClient("LoanApi");
        var token = User.FindFirst("IdToken")?.Value;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new MyLoansViewModel();
        var res = await Api().GetAsync("api/LoansApi/mine");
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            vm.Loans = JsonSerializer.Deserialize<List<Loan>>(body, _json) ?? new();
        }
        else
        {
            TempData["Error"] = $"Could not load loans ({(int)res.StatusCode}).";
        }
        return View(vm);
    }

    [HttpGet]
    public IActionResult Apply() => View(new LoanApplyRequest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(LoanApplyRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        model.ApplicantFormDate = DateTime.UtcNow;
        model.ReasonsForLoan = model.ReasonsForLoan?.Where(r => !string.IsNullOrWhiteSpace(r)).ToList() ?? new();

        var content = new StringContent(JsonSerializer.Serialize(model, _json), Encoding.UTF8, "application/json");
        var res = await Api().PostAsync("api/LoansApi", content);

        if (!res.IsSuccessStatusCode)
        {
            var err = await res.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"API returned {(int)res.StatusCode}: {err}");
            return View(model);
        }

        TempData["Success"] = "Your loan application was submitted.";
        return RedirectToAction(nameof(Index));
    }
}