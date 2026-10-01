using APIINSY7315.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace APIINSY7315.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FirebaseAuthorize]
    public class LoansApiController : ControllerBase
    {
        private const string CollectionName = "LoanApplications";

        private readonly FirestoreDb _db;
        private readonly ILogger<LoansApiController> _logger;

        public LoansApiController(
            FirestoreDb db,
            ILogger<LoansApiController> logger)
        {
            _db = db;
            _logger = logger;
        }


        // ============================================================
        // POST /api/LoansApi
        // ============================================================

        [HttpPost]
        [FirebaseAuthorize(Roles.Client)]
        public async Task<IActionResult> Submit(
            [FromBody] SubmitLoanRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    error = "Request body is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var applicationId =
                Guid.NewGuid().ToString();

            var userId =
                HttpContext.GetUserId();

            var userEmail =
                HttpContext.GetEmail() ?? "";

            try
            {
                // ----------------------------------------------------
                // Convert client supplied DateTime to Firestore
                // Timestamp safely.
                // ----------------------------------------------------

                Timestamp? applicantFormTimestamp = null;

                if (request.ApplicantFormDate.HasValue)
                {
                    var date =
                        request.ApplicantFormDate.Value;

                    if (date.Kind == DateTimeKind.Unspecified)
                    {
                        date =
                            DateTime.SpecifyKind(
                                date,
                                DateTimeKind.Utc);
                    }
                    else
                    {
                        date =
                            date.ToUniversalTime();
                    }

                    applicantFormTimestamp =
                        Timestamp.FromDateTime(date);
                }


                // ----------------------------------------------------
                // Build strongly typed Firestore object.
                // ----------------------------------------------------

                var application =
                    new LoanApplicationModel
                    {
                        ApplicationId =
                            applicationId,

                        UserId =
                            userId,

                        UserEmail =
                            userEmail,

                        Status =
                            LoanStatus.Submitted,

                        ApplicationDate =
                            Timestamp.GetCurrentTimestamp(),

                        RequestedAmount =
                            request.RequestedAmount,

                        ReasonForLoan =
                            request.ReasonForLoan ?? "",

                        ClientDetails =
                            new ClientDetailsModel
                            {
                                FullNameAndSurname =
                                    request.ClientDetails
                                        ?.FullNameAndSurname ?? "",

                                IdNumber =
                                    request.ClientDetails
                                        ?.IdNumber ?? "",

                                CellNo =
                                    request.ClientDetails
                                        ?.CellNo ?? ""
                            },

                        HomeTelNo =
                            request.HomeTelNo ?? "",

                        MarriedOrUnmarried =
                            request.MarriedOrUnmarried ?? "",

                        MarriageCommunity =
                            request.MarriageCommunity ?? "",

                        PreviouslyDivorced =
                            request.PreviouslyDivorced ?? "",

                        DivorceYear =
                            request.DivorceYear ?? "",

                        DivorceCommunity =
                            request.DivorceCommunity ?? "",

                        CurrentPhysicalAddress =
                            request.CurrentPhysicalAddress ?? "",

                        PostalAddress =
                            request.PostalAddress ?? "",

                        ParentsAddress =
                            request.ParentsAddress ?? "",

                        ResidenceYears =
                            request.ResidenceYears,

                        ResidenceMonths =
                            request.ResidenceMonths,

                        CompanyName =
                            request.CompanyName ?? "",

                        WorkTelephone =
                            request.WorkTelephone ?? "",

                        Occupation =
                            request.Occupation ?? "",

                        WorkAddress =
                            request.WorkAddress ?? "",

                        BankName =
                            request.BankName ?? "",

                        AccountType =
                            request.AccountType ?? "",

                        AccountNumber =
                            request.AccountNumber ?? "",

                        BranchName =
                            request.BranchName ?? "",

                        AccountName =
                            request.AccountName ?? "",

                        BranchCode =
                            request.BranchCode ?? "",

                        SpouseNameAndSurname =
                            request.SpouseNameAndSurname ?? "",

                        SpouseIdNumber =
                            request.SpouseIdNumber ?? "",

                        SpouseTelNumber =
                            request.SpouseTelNumber ?? "",

                        SpouseEmployerName =
                            request.SpouseEmployerName ?? "",

                        SpouseEmployerAddress =
                            request.SpouseEmployerAddress ?? "",

                        SpouseEmployerTelNumber =
                            request.SpouseEmployerTelNumber ?? "",

                        Relative1Name =
                            request.Relative1Name ?? "",

                        Relative1Relationship =
                            request.Relative1Relationship ?? "",

                        Relative1TelNumber =
                            request.Relative1TelNumber ?? "",

                        Relative1Address =
                            request.Relative1Address ?? "",

                        Relative2Name =
                            request.Relative2Name ?? "",

                        Relative2Relationship =
                            request.Relative2Relationship ?? "",

                        Relative2TelNumber =
                            request.Relative2TelNumber ?? "",

                        Relative2Address =
                            request.Relative2Address ?? "",

                        ReasonsForLoan =
                            request.ReasonsForLoan
                            ?? new List<string>(),

                        OtherReason =
                            request.OtherReason ?? "",

                        ApplicantSignature =
                            request.ApplicantSignature ?? "",

                        ApplicantFormDate =
                            applicantFormTimestamp,

                        ReviewedBy =
                            "",

                        ReviewedAt =
                            null,

                        ReviewNote =
                            ""
                    };


                // ----------------------------------------------------
                // Firestore document
                // ----------------------------------------------------

                var document =
                    _db
                        .Collection(CollectionName)
                        .Document(applicationId);


                // IMPORTANT:
                // Save the strongly typed Firestore model.
                await document.CreateAsync(application);


                _logger.LogInformation(
                    "Loan application {ApplicationId} created for user {UserId}",
                    applicationId,
                    userId);


                return Created(
                    $"/api/LoansApi/{applicationId}",
                    new
                    {
                        applicationId,
                        status =
                            LoanStatus.Submitted
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "LOAN SAVE FAILED. ApplicationId={ApplicationId}, UserId={UserId}",
                    applicationId,
                    userId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error =
                            "LOAN_SAVE_ERROR",

                        message =
                            ex.Message,

                        exceptionType =
                            ex.GetType().FullName,

                        innerMessage =
                            ex.InnerException?.Message,

                        traceId =
                            HttpContext.TraceIdentifier
                    });
            }
        }


        // ============================================================
        // GET /api/LoansApi/mine
        // ============================================================

        [HttpGet("mine")]
        [FirebaseAuthorize(Roles.Client)]
        public async Task<IActionResult> GetMine()
        {
            try
            {
                var userId =
                    HttpContext.GetUserId();

                var snapshot =
                    await _db
                        .Collection(CollectionName)
                        .WhereEqualTo(
                            "UserId",
                            userId)
                        .GetSnapshotAsync();

                var result =
                    snapshot.Documents
                        .Where(x => x.Exists)
                        .Select(MapDocument)
                        .OrderByDescending(
                            x => x.ApplicationDate)
                        .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed loading applications for {UserId}",
                    HttpContext.GetUserId());

                return StatusCode(
                    500,
                    new
                    {
                        error =
                            "Could not load your applications.",

                        detail =
                            ex.Message,

                        traceId =
                            HttpContext.TraceIdentifier
                    });
            }
        }


        // ============================================================
        // GET /api/LoansApi
        // ============================================================

        [HttpGet]
        [FirebaseAuthorize(
            Roles.Admin,
            Roles.Management)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status)
        {
            try
            {
                Query query =
                    _db.Collection(CollectionName);

                if (!string.IsNullOrWhiteSpace(status))
                {
                    var validStatus =
                        LoanStatus.All.FirstOrDefault(
                            x => x.Equals(
                                status,
                                StringComparison.OrdinalIgnoreCase));

                    if (validStatus == null)
                    {
                        return BadRequest(new
                        {
                            error =
                                "Invalid loan status."
                        });
                    }

                    query =
                        query.WhereEqualTo(
                            "Status",
                            validStatus);
                }

                var snapshot =
                    await query.GetSnapshotAsync();

                var result =
                    snapshot.Documents
                        .Where(x => x.Exists)
                        .Select(MapDocument)
                        .OrderByDescending(
                            x => x.ApplicationDate)
                        .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed loading all applications.");

                return StatusCode(
                    500,
                    new
                    {
                        error =
                            "Could not load applications.",

                        detail =
                            ex.Message,

                        traceId =
                            HttpContext.TraceIdentifier
                    });
            }
        }


        // ============================================================
        // GET /api/LoansApi/pending
        // ============================================================

        [HttpGet("pending")]
        [FirebaseAuthorize(
            Roles.Admin,
            Roles.Management)]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var snapshot =
                    await _db
                        .Collection(CollectionName)
                        .WhereEqualTo(
                            "Status",
                            LoanStatus.Submitted)
                        .GetSnapshotAsync();

                var result =
                    snapshot.Documents
                        .Where(x => x.Exists)
                        .Select(MapDocument)
                        .OrderByDescending(
                            x => x.ApplicationDate)
                        .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed loading pending applications.");

                return StatusCode(
                    500,
                    new
                    {
                        error =
                            "Could not load pending applications.",

                        detail =
                            ex.Message,

                        traceId =
                            HttpContext.TraceIdentifier
                    });
            }
        }


        // ============================================================
        // GET /api/LoansApi/{id}
        // ============================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            string id)
        {
            try
            {
                var snapshot =
                    await _db
                        .Collection(CollectionName)
                        .Document(id)
                        .GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound();
                }

                var application =
                    MapDocument(snapshot);

                var currentUser =
                    HttpContext.GetUserId();

                var role =
                    HttpContext.GetRole();

                if (application.UserId != currentUser &&
                    !Roles.IsStaff(role))
                {
                    return NotFound();
                }

                return Ok(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed loading application {Id}",
                    id);

                return StatusCode(
                    500,
                    new
                    {
                        error =
                            "Could not load the application.",

                        detail =
                            ex.Message,

                        traceId =
                            HttpContext.TraceIdentifier
                    });
            }
        }


        // ============================================================
        // PUT /api/LoansApi/{id}/status
        // ============================================================

        [HttpPut("{id}/status")]
        [FirebaseAuthorize(
            Roles.Admin,
            Roles.Management)]
        public async Task<IActionResult> UpdateStatus(
            string id,
            [FromBody] UpdateStatusRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    error =
                        "Request body is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var validStatus =
                LoanStatus.All.FirstOrDefault(
                    x => x.Equals(
                        request.Status,
                        StringComparison.OrdinalIgnoreCase));

            if (validStatus == null)
            {
                return BadRequest(new
                {
                    error =
                        "Invalid loan status."
                });
            }

            try
            {
                var document =
                    _db
                        .Collection(CollectionName)
                        .Document(id);

                var snapshot =
                    await document.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound();
                }

                var updates =
                    new Dictionary<string, object>
                    {
                        ["Status"] =
                            validStatus,

                        ["ReviewedBy"] =
                            HttpContext.GetUserId(),

                        ["ReviewedAt"] =
                            Timestamp.GetCurrentTimestamp(),

                        ["ReviewNote"] =
                            request.Note ?? ""
                    };

                await document.UpdateAsync(updates);

                return Ok(new
                {
                    applicationId = id,
                    status = validStatus
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed updating application {Id}",
                    id);

                return StatusCode(
                    500,
                    new
                    {
                        error =
                            "Could not update the application.",

                        detail =
                            ex.Message,

                        exception =
                            ex.GetType().FullName,

                        inner =
                            ex.InnerException?.Message,

                        traceId =
                            HttpContext.TraceIdentifier
                    });
            }
        }


        // ============================================================
        // FIRESTORE DOCUMENT -> DTO
        // ============================================================

        private static LoanApplicationDto MapDocument(
            DocumentSnapshot document)
        {
            var data =
                document.ToDictionary();

            return new LoanApplicationDto
            {
                ApplicationId =
                    GetString(
                        data,
                        "ApplicationId"),

                UserId =
                    GetString(
                        data,
                        "UserId"),

                UserEmail =
                    GetString(
                        data,
                        "UserEmail"),

                Status =
                    GetString(
                        data,
                        "Status"),

                ApplicationDate =
                    GetDateTime(
                        data,
                        "ApplicationDate"),

                RequestedAmount =
                    GetDouble(
                        data,
                        "RequestedAmount"),

                ReasonForLoan =
                    GetString(
                        data,
                        "ReasonForLoan"),

                ClientDetails =
                    GetClientDetails(data),

                HomeTelNo =
                    GetString(
                        data,
                        "HomeTelNo"),

                MarriedOrUnmarried =
                    GetString(
                        data,
                        "MarriedOrUnmarried"),

                MarriageCommunity =
                    GetString(
                        data,
                        "MarriageCommunity"),

                PreviouslyDivorced =
                    GetString(
                        data,
                        "PreviouslyDivorced"),

                DivorceYear =
                    GetString(
                        data,
                        "DivorceYear"),

                DivorceCommunity =
                    GetString(
                        data,
                        "DivorceCommunity"),

                CurrentPhysicalAddress =
                    GetString(
                        data,
                        "CurrentPhysicalAddress"),

                PostalAddress =
                    GetString(
                        data,
                        "PostalAddress"),

                ParentsAddress =
                    GetString(
                        data,
                        "ParentsAddress"),

                ResidenceYears =
                    GetInt(
                        data,
                        "ResidenceYears"),

                ResidenceMonths =
                    GetInt(
                        data,
                        "ResidenceMonths"),

                CompanyName =
                    GetString(
                        data,
                        "CompanyName"),

                WorkTelephone =
                    GetString(
                        data,
                        "WorkTelephone"),

                Occupation =
                    GetString(
                        data,
                        "Occupation"),

                WorkAddress =
                    GetString(
                        data,
                        "WorkAddress"),

                BankName =
                    GetString(
                        data,
                        "BankName"),

                AccountType =
                    GetString(
                        data,
                        "AccountType"),

                AccountNumber =
                    GetString(
                        data,
                        "AccountNumber"),

                BranchName =
                    GetString(
                        data,
                        "BranchName"),

                AccountName =
                    GetString(
                        data,
                        "AccountName"),

                BranchCode =
                    GetString(
                        data,
                        "BranchCode"),

                SpouseNameAndSurname =
                    GetString(
                        data,
                        "SpouseNameAndSurname"),

                SpouseIdNumber =
                    GetString(
                        data,
                        "SpouseIdNumber"),

                SpouseTelNumber =
                    GetString(
                        data,
                        "SpouseTelNumber"),

                SpouseEmployerName =
                    GetString(
                        data,
                        "SpouseEmployerName"),

                SpouseEmployerAddress =
                    GetString(
                        data,
                        "SpouseEmployerAddress"),

                SpouseEmployerTelNumber =
                    GetString(
                        data,
                        "SpouseEmployerTelNumber"),

                Relative1Name =
                    GetString(
                        data,
                        "Relative1Name"),

                Relative1Relationship =
                    GetString(
                        data,
                        "Relative1Relationship"),

                Relative1TelNumber =
                    GetString(
                        data,
                        "Relative1TelNumber"),

                Relative1Address =
                    GetString(
                        data,
                        "Relative1Address"),

                Relative2Name =
                    GetString(
                        data,
                        "Relative2Name"),

                Relative2Relationship =
                    GetString(
                        data,
                        "Relative2Relationship"),

                Relative2TelNumber =
                    GetString(
                        data,
                        "Relative2TelNumber"),

                Relative2Address =
                    GetString(
                        data,
                        "Relative2Address"),

                ReasonsForLoan =
                    GetStringList(
                        data,
                        "ReasonsForLoan"),

                OtherReason =
                    GetString(
                        data,
                        "OtherReason"),

                ApplicantSignature =
                    GetString(
                        data,
                        "ApplicantSignature"),

                ApplicantFormDate =
                    GetNullableDateTime(
                        data,
                        "ApplicantFormDate"),

                ReviewedBy =
                    GetString(
                        data,
                        "ReviewedBy"),

                ReviewedAt =
                    GetNullableTimestamp(
                        data,
                        "ReviewedAt"),

                ReviewNote =
                    GetString(
                        data,
                        "ReviewNote")
            };
        }


        // ============================================================
        // STRING
        // ============================================================

        private static string GetString(
            Dictionary<string, object> data,
            string key)
        {
            if (!data.TryGetValue(
                    key,
                    out var value) ||
                value == null)
            {
                return "";
            }

            return value.ToString() ?? "";
        }


        // ============================================================
        // DOUBLE
        // ============================================================

        private static double GetDouble(
            Dictionary<string, object> data,
            string key)
        {
            if (!data.TryGetValue(
                    key,
                    out var value) ||
                value == null)
            {
                return 0;
            }

            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0;
            }
        }


        // ============================================================
        // INT
        // ============================================================

        private static int GetInt(
            Dictionary<string, object> data,
            string key)
        {
            if (!data.TryGetValue(
                    key,
                    out var value) ||
                value == null)
            {
                return 0;
            }

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }


        // ============================================================
        // DATETIME
        // ============================================================

        private static DateTime GetDateTime(
            Dictionary<string, object> data,
            string key)
        {
            if (!data.TryGetValue(
                    key,
                    out var value) ||
                value == null)
            {
                return DateTime.MinValue;
            }

            if (value is Timestamp timestamp)
            {
                return timestamp.ToDateTime();
            }

            if (value is DateTime dateTime)
            {
                return dateTime;
            }

            if (DateTime.TryParse(
                    value.ToString(),
                    out var parsed))
            {
                return parsed;
            }

            return DateTime.MinValue;
        }


        // ============================================================
        // NULLABLE DATETIME
        // ============================================================

        private static DateTime? GetNullableDateTime(
            Dictionary<string, object> data,
            string key)
        {
            if (!data.TryGetValue(
                    key,
                    out var value) ||
                value == null)
            {
                return null;
            }

            if (value is Timestamp timestamp)
            {
                return timestamp.ToDateTime();
            }

            if (value is DateTime dateTime)
            {
                return dateTime;
            }

            if (DateTime.TryParse(
                    value.ToString(),
                    out var parsed))
            {
                return parsed;
            }

            return null;
        }


        // ============================================================
        // NULLABLE TIMESTAMP
        // ============================================================

        private static Timestamp? GetNullableTimestamp(
            Dictionary<string, object> data,
            string key)
        {
            if (!data.TryGetValue(
                    key,
                    out var value) ||
                value == null)
            {
                return null;
            }

            if (value is Timestamp timestamp)
            {
                return timestamp;
            }

            return null;
        }


        // ============================================================
        // STRING LIST
        // ============================================================

        private static List<string> GetStringList(
            Dictionary<string, object> data,
            string key)
        {
            if (!data.TryGetValue(
                    key,
                    out var value) ||
                value == null)
            {
                return new List<string>();
            }

            if (value is IEnumerable<string> strings)
            {
                return strings
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }

            if (value is IEnumerable<object> objects)
            {
                return objects
                    .Where(x => x != null)
                    .Select(x => x?.ToString() ?? "")
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }

            return new List<string>();
        }


        // ============================================================
        // CLIENT DETAILS
        // ============================================================

        private static ClientDetailsDto GetClientDetails(
            Dictionary<string, object> data)
        {
            if (!data.TryGetValue(
                    "ClientDetails",
                    out var value) ||
                value == null)
            {
                return new ClientDetailsDto();
            }

            if (value is Dictionary<string, object> client)
            {
                return new ClientDetailsDto
                {
                    FullNameAndSurname =
                        GetString(
                            client,
                            "FullNameAndSurname"),

                    IdNumber =
                        GetString(
                            client,
                            "IdNumber"),

                    CellNo =
                        GetString(
                            client,
                            "CellNo")
                };
            }

            return new ClientDetailsDto();
        }
    }
}