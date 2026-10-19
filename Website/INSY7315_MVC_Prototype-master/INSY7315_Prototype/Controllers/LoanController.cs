
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using INSY7315_Prototype.Models;
using INSY7315_Prototype.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using INSY7315_Prototype.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "client")]
public class LoanController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<LoanController> _logger;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LoanController(
        IHttpClientFactory factory,
        ILogger<LoanController> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    private HttpClient Api()
    {
        var client = _factory.CreateClient("LoanApi");
        var token = User.FindFirst("IdToken")?.Value;

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    // CLIENT LOAN HISTORY
    public async Task<IActionResult> Index()
    {
        var vm = new MyLoansViewModel();

        try
        {
            using var client = Api();
            using var response =
                await client.GetAsync("api/LoansApi/mine");

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();

                vm.Loans =
                    JsonSerializer.Deserialize<List<Loan>>(body, _json)
                    ?? new List<Loan>();
            }
            else
            {
                _logger.LogWarning(
                    "Unable to retrieve client loans. Status: {Status}",
                    (int)response.StatusCode);

                TempData["Error"] =
                    "We couldn't load your loans right now. Please try again.";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                "Loan API connection failed: {ExceptionType}",
                ex.GetType().Name);

            TempData["Error"] =
                "The loan service is temporarily unavailable.";
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] =
                "The loan service took too long to respond. Please try again.";
        }
        catch (JsonException)
        {
            _logger.LogWarning("Unexpected loan history response format.");

            TempData["Error"] =
                "We couldn't read your loan information right now.";
        }

        return View(vm);
    }

    // DISPLAY APPLICATION FORM
    [HttpGet]
    public IActionResult Apply()
    {
        return View(new LoanApplyRequest());
    }

    // SUBMIT APPLICATION
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(LoanApplyRequest model, List<IFormFile>? supportingDocuments)
    {
        bool married = string.Equals(
            model.MarriedOrUnmarried,
            "Married",
            StringComparison.OrdinalIgnoreCase);

        bool divorced = string.Equals(
            model.PreviouslyDivorced,
            "Yes",
            StringComparison.OrdinalIgnoreCase);

        // Validate the temporary loan amount range.
        // The model also enforces this with its Range attribute.
        if (model.RequestedAmount < 300 ||
            model.RequestedAmount > 500000)
        {
            ModelState.AddModelError(
                nameof(model.RequestedAmount),
                "Loan amount must be between R300 and R500,000.");
        }

        // Validate conditional spouse fields.
        if (married)
        {
            if (string.IsNullOrWhiteSpace(model.SpouseNameAndSurname))
            {
                ModelState.AddModelError(
                    nameof(model.SpouseNameAndSurname),
                    "Spouse full name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SpouseIdNumber))
            {
                ModelState.AddModelError(
                    nameof(model.SpouseIdNumber),
                    "Spouse ID number is required.");
            }

            if (string.IsNullOrWhiteSpace(model.SpouseTelNumber))
            {
                ModelState.AddModelError(
                    nameof(model.SpouseTelNumber),
                    "Spouse telephone number is required.");
            }
        }

        // Validate divorce details only when applicable.
        if (divorced)
        {
            if (string.IsNullOrWhiteSpace(model.DivorceYear))
            {
                ModelState.AddModelError(
                    nameof(model.DivorceYear),
                    "Please enter the year of divorce.");
            }
            else if (!int.TryParse(model.DivorceYear, out int year) ||
                     year < 1900 ||
                     year > DateTime.UtcNow.Year)
            {
                ModelState.AddModelError(
                    nameof(model.DivorceYear),
                    "Please enter a valid divorce year.");
            }
        }

        // Require an explanation if Other was selected.
        bool selectedOther =
            model.ReasonsForLoan?.Any(r =>
                string.Equals(
                    r,
                    "Other",
                    StringComparison.OrdinalIgnoreCase)) == true;

        if (selectedOther && string.IsNullOrWhiteSpace(model.OtherReason))
        {
            ModelState.AddModelError(
                nameof(model.OtherReason),
                "Please specify your other reason for the loan.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Clear fields that do not apply to the applicant.
        if (!married)
        {
            model.MarriageCommunity = null;

            model.SpouseNameAndSurname = null;
            model.SpouseIdNumber = null;
            model.SpouseTelNumber = null;

            model.SpouseEmployerName = null;
            model.SpouseEmployerAddress = null;
            model.SpouseEmployerTelNumber = null;
        }

        if (!divorced)
        {
            model.DivorceYear = null;
            model.DivorceCommunity = null;
        }

        if (!selectedOther)
        {
            model.OtherReason = null;
        }

        // Remove empty or duplicate reasons.
        model.ReasonsForLoan =
            model.ReasonsForLoan?
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            ?? new List<string>();

        // Preserve the API's existing non-null string requirement.
        FillBlanks(model);

        model.ApplicantFormDate = DateTime.UtcNow;

        try
        {
            using var client = Api();

            var json = JsonSerializer.Serialize(model, _json);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            using var response = await client.PostAsync(
                "api/LoansApi",
                content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Loan submission failed. HTTP status: {Status}",
                    (int)response.StatusCode);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(
                        "",
                        "Your session has expired. Please log in again.");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ModelState.AddModelError(
                        "",
                        "The loan service rejected the application. " +
                        "Please check your details and try again.");
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "We couldn't submit your application right now. " +
                        "Please try again later.");
                }

                return View(model);
            }

            // The existing API returns the new applicationId in its 201 response.
            // Attach demo documents only after the loan has been created.
            if (supportingDocuments?.Any(f => f.Length > 0) == true)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                string? applicationId = null;
                try
                {
                    using var created = JsonDocument.Parse(responseJson);
                    if (created.RootElement.TryGetProperty("applicationId", out var id))
                        applicationId = id.GetString();
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Loan created but the application ID was not returned in the expected format.");
                }

                if (!string.IsNullOrWhiteSpace(applicationId))
                {
                    var uploadErrors = await LoanDocumentsController.SaveDemoDocumentsAsync(
                        HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>(),
                        applicationId,
                        supportingDocuments);
                    TempData["DocumentUploadResult"] = uploadErrors.Count == 0
                        ? "Supporting documents uploaded successfully."
                        : "Loan submitted, but some documents were not saved: " + string.Join(" ", uploadErrors);
                }
                else
                {
                    TempData["DocumentUploadResult"] =
                        "Loan submitted, but documents could not be attached. Please upload them from My Loans.";
                }
            }

            TempData["Success"] =
                "Your loan application was submitted successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            _logger.LogWarning("Loan submission API connection failed.");

            ModelState.AddModelError(
                "",
                "The loan service is temporarily unavailable. " +
                "Please try again later.");
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Loan submission request timed out.");

            ModelState.AddModelError(
                "",
                "The request timed out. Please check your application " +
                "history before trying again.");
        }

        return View(model);
    }

    // Replace blank strings with N/A because the current
    // API expects non-null string values.
    private static void FillBlanks(LoanApplyRequest model)
    {
        FillStringProperties(model);

        if (model.ClientDetails != null)
        {
            FillStringProperties(model.ClientDetails);
        }
    }

    private static void FillStringProperties(object obj)
    {
        foreach (var property in obj.GetType().GetProperties()
            .Where(p =>
                p.PropertyType == typeof(string) &&
                p.CanWrite))
        {
            var value = (string?)property.GetValue(obj);

            if (string.IsNullOrWhiteSpace(value))
            {
                property.SetValue(obj, "N/A");
            }
        }
    }
}
