using System.Net.Http.Headers;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers;

[Authorize(Roles = "client,staff,admin")]
[Route("LoanDocuments")]
public class LoanDocumentsController : Controller
{
    private readonly IHttpClientFactory _clients;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LoanDocumentsController> _logger;
    private const long MaxBytes = 5 * 1024 * 1024;
    private const string RequestsFile = "document-requests.json";
    private static readonly SemaphoreSlim RequestLock = new(1, 1);
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".jpg", ".jpeg", ".png" };

    public LoanDocumentsController(IHttpClientFactory clients, IWebHostEnvironment environment,
        ILogger<LoanDocumentsController> logger)
    {
        _clients = clients;
        _environment = environment;
        _logger = logger;
    }

    // Existing LoanController calls this method after a new application succeeds.
    public static async Task<List<string>> SaveDemoDocumentsAsync(
        IWebHostEnvironment environment, string loanId, IEnumerable<IFormFile> documents)
    {
        var errors = new List<string>();
        var folder = Path.Combine(environment.ContentRootPath, "DemoDocuments", SafeFolderName(loanId));
        foreach (var document in documents.Where(f => f.Length > 0))
        {
            if (document.Length > MaxBytes || !AllowedExtensions.Contains(Path.GetExtension(document.FileName)))
            {
                errors.Add($"{Path.GetFileName(document.FileName)} must be PDF/JPG/PNG and under 5 MB.");
                continue;
            }
            Directory.CreateDirectory(folder);
            var originalName = Path.GetFileName(document.FileName);
            var safeName = string.Concat(originalName.Select(c =>
                char.IsLetterOrDigit(c) || c is '.' or '-' or '_' ? c : '_'));
            var storedName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{safeName}";
            await using var output = System.IO.File.Create(Path.Combine(folder, storedName));
            await document.CopyToAsync(output);
        }
        return errors;
    }

    private string Root => Path.Combine(_environment.ContentRootPath, "DemoDocuments");
    private static string SafeFolderName(string loanId) => Convert.ToHexString(
        System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(loanId)));
    private string LoanFolder(string loanId) => Path.Combine(Root, SafeFolderName(loanId));
    private string RequestPath(string loanId) => Path.Combine(LoanFolder(loanId), RequestsFile);
    private bool IsClient => User.IsInRole("client");
    private bool IsReviewer => User.IsInRole("staff") || User.IsInRole("admin");

    private async Task<bool> CanAccessLoan(string loanId)
    {
        if (string.IsNullOrWhiteSpace(loanId)) return false;
        var token = User.FindFirst("IdToken")?.Value;
        if (string.IsNullOrWhiteSpace(token)) return false;
        try
        {
            using var client = _clients.CreateClient("LoanApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Clients must prove ownership; staff/admin must prove the API lets them review the loan.
            var url = IsClient ? "api/LoansApi/mine" : $"api/LoansApi/{Uri.EscapeDataString(loanId)}";
            using var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return false;
            if (User.IsInRole("admin")) return true;
            // Staff may view and request documents for every loan, including
            // applications above R7,000. Approval restrictions are separate.
            if (User.IsInRole("staff")) return true;
            using var stream = await response.Content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(stream);
            return json.RootElement.ValueKind == JsonValueKind.Array &&
                json.RootElement.EnumerateArray().Any(item =>
                    item.TryGetProperty("applicationId", out var id) &&
                    string.Equals(id.ToString(), loanId, StringComparison.Ordinal));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning("Unable to verify loan access: {Type}", ex.GetType().Name);
            return false;
        }
    }

    public class DocumentRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string DocumentType { get; set; } = "";
        public string Note { get; set; } = "";
        public string RequestedBy { get; set; } = "";
        public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Requested";
        public string? UploadedFileName { get; set; }
    }

    private async Task<List<DocumentRequest>> ReadRequestsAsync(string loanId)
    {
        var path = RequestPath(loanId);
        if (!System.IO.File.Exists(path)) return new();
        var json = await System.IO.File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<DocumentRequest>>(json) ?? new();
    }

    private async Task WriteRequestsAsync(string loanId, List<DocumentRequest> requests)
    {
        Directory.CreateDirectory(LoanFolder(loanId));
        var path = RequestPath(loanId);
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        await System.IO.File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(requests));
        System.IO.File.Move(temporary, path, true);
    }

    [HttpGet("{loanId}")]
    public async Task<IActionResult> Index(string loanId)
    {
        if (!await CanAccessLoan(loanId)) return Forbid();
        var folder = LoanFolder(loanId);
        var files = Directory.Exists(folder)
            ? Directory.GetFiles(folder)
                .Select(Path.GetFileName)
                .Where(n => n != null && AllowedExtensions.Contains(Path.GetExtension(n)))
                .Cast<string>().OrderBy(n => n).ToList()
            : new List<string>();
        await RequestLock.WaitAsync();
        List<DocumentRequest> requests;
        try { requests = await ReadRequestsAsync(loanId); }
        finally { RequestLock.Release(); }
        ViewBag.LoanId = loanId;
        ViewBag.Files = files;
        ViewBag.Requests = requests.OrderByDescending(r => r.RequestedAtUtc).ToList();
        return View();
    }

    [Authorize(Roles = "staff,admin")]
    [HttpPost("{loanId}/request")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestDocument(string loanId, string documentType, string? note)
    {
        if (!await CanAccessLoan(loanId)) return Forbid();
        var allowedTypes = new[] { "ID document", "Proof of address", "Bank statement", "Payslip", "Other" };
        if (!allowedTypes.Contains(documentType) || (documentType == "Other" && string.IsNullOrWhiteSpace(note)))
        {
            TempData["DocumentError"] = "Select a document type (and add a note for Other).";
            return RedirectToAction(nameof(Index), new { loanId });
        }
        await RequestLock.WaitAsync();
        try
        {
            var requests = await ReadRequestsAsync(loanId);
            requests.Add(new DocumentRequest
            {
                DocumentType = documentType,
                Note = (note ?? "").Trim().Length > 500 ? note!.Trim()[..500] : (note ?? "").Trim(),
                RequestedBy = User.IsInRole("admin") ? "Manager" : "Staff"
            });
            await WriteRequestsAsync(loanId, requests);
        }
        finally { RequestLock.Release(); }
        TempData["DocumentSuccess"] = "Document requested successfully.";
        return RedirectToAction(nameof(Index), new { loanId });
    }

    [Authorize(Roles = "client")]
    [HttpPost("{loanId}/upload")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxBytes + 1024 * 1024)]
    public async Task<IActionResult> Upload(string loanId, IFormFile? document, string? requestId)
    {
        if (!await CanAccessLoan(loanId)) return Forbid();
        if (document == null || document.Length == 0 || document.Length > MaxBytes ||
            !AllowedExtensions.Contains(Path.GetExtension(document.FileName)))
        {
            TempData["DocumentError"] = "Choose a PDF, JPG or PNG file smaller than 5 MB.";
            return RedirectToAction(nameof(Index), new { loanId });
        }
        await RequestLock.WaitAsync();
        try
        {
            var requests = await ReadRequestsAsync(loanId);
            DocumentRequest? request = null;
            if (!string.IsNullOrWhiteSpace(requestId))
            {
                request = requests.FirstOrDefault(r => r.Id == requestId && r.Status == "Requested");
                if (request == null)
                {
                    TempData["DocumentError"] = "This document request is no longer outstanding.";
                    return RedirectToAction(nameof(Index), new { loanId });
                }
            }
            var folder = LoanFolder(loanId);
            Directory.CreateDirectory(folder);
            var originalName = Path.GetFileName(document.FileName);
            var safeName = string.Concat(originalName.Select(c =>
                char.IsLetterOrDigit(c) || c is '.' or '-' or '_' ? c : '_'));
            var storedName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{safeName}";
            await using (var output = System.IO.File.Create(Path.Combine(folder, storedName)))
                await document.CopyToAsync(output);
            if (request != null)
            {
                request.Status = "Submitted";
                request.UploadedFileName = storedName;
                await WriteRequestsAsync(loanId, requests);
            }
            TempData["DocumentSuccess"] = request == null
                ? "Demo document uploaded successfully."
                : "Requested document uploaded successfully.";
        }
        finally { RequestLock.Release(); }
        return RedirectToAction(nameof(Index), new { loanId });
    }

    [HttpGet("{loanId}/download/{fileName}")]
    public async Task<IActionResult> Download(string loanId, string fileName)
    {
        if (!await CanAccessLoan(loanId)) return Forbid();
        if (fileName != Path.GetFileName(fileName) ||
            !AllowedExtensions.Contains(Path.GetExtension(fileName))) return BadRequest();
        var path = Path.Combine(LoanFolder(loanId), fileName);
        if (!System.IO.File.Exists(path)) return NotFound();
        var contentType = Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
        return PhysicalFile(path, contentType, fileName);
    }
}
