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
    private readonly ILogger<LoanController> _logger;   
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LoanController(IHttpClientFactory factory, ILogger<LoanController> logger)
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
        bool married = model.MarriedOrUnmarried == "Married";
        bool divorced = model.PreviouslyDivorced == "Yes";

        // Only require spouse / divorce details when they apply
        if (married)
        {
            if (string.IsNullOrWhiteSpace(model.SpouseNameAndSurname))
                ModelState.AddModelError(nameof(model.SpouseNameAndSurname), "Spouse name is required.");
            if (string.IsNullOrWhiteSpace(model.SpouseIdNumber))
                ModelState.AddModelError(nameof(model.SpouseIdNumber), "Spouse ID number is required.");
            if (string.IsNullOrWhiteSpace(model.SpouseTelNumber))
                ModelState.AddModelError(nameof(model.SpouseTelNumber), "Spouse phone number is required.");
        }
        if (divorced && string.IsNullOrWhiteSpace(model.DivorceYear))
            ModelState.AddModelError(nameof(model.DivorceYear), "Please enter the year of divorce.");

        if (!ModelState.IsValid) return View(model);

        // Clear values that don't apply (hidden fields can still post old input)
        if (!married)
        {
            model.MarriageCommunity = null;
            model.SpouseNameAndSurname = model.SpouseIdNumber = model.SpouseTelNumber = null;
            model.SpouseEmployerName = model.SpouseEmployerAddress = model.SpouseEmployerTelNumber = null;
        }
        if (!divorced)
        {
            model.DivorceYear = null;
            model.DivorceCommunity = null;
        }

        // The API rejects null strings, so anything left blank becomes "N/A"
        FillBlanks(model);

        model.ApplicantFormDate = DateTime.UtcNow;
        model.ReasonsForLoan = model.ReasonsForLoan?.Where(r => !string.IsNullOrWhiteSpace(r)).ToList() ?? new();

        var content = new StringContent(JsonSerializer.Serialize(model, _json), Encoding.UTF8, "application/json");
        var res = await Api().PostAsync("api/LoansApi", content);

        if (!res.IsSuccessStatusCode)
        {
            // Full detail goes to the log, not the page
            var err = await res.Content.ReadAsStringAsync();
            _logger.LogWarning("Loan submit failed: {Status} {Body}", (int)res.StatusCode, err);

            ModelState.AddModelError("", res.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? "Your session has expired. Please log in again."
                : "We couldn't submit your application right now. Please check your details and try again.");
            return View(model);
        }

        TempData["Success"] = "Your loan application was submitted.";
        return RedirectToAction(nameof(Index));
    }

    private static void FillBlanks(object obj)
    {
        foreach (var p in obj.GetType().GetProperties()
                     .Where(p => p.PropertyType == typeof(string) && p.CanWrite))
        {
            if (string.IsNullOrWhiteSpace((string?)p.GetValue(obj)))
                p.SetValue(obj, "N/A");
        }
    }
}