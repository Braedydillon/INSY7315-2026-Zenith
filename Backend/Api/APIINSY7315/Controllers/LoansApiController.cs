using APIINSY7315.Models;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace APIINSY7315.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FirebaseAuthorize] // any signed-in user; stricter rules per action below
    public class LoansApiController : ControllerBase
    {
        private const string Collection = "LoanApplications";
        private readonly FirestoreDb _db;

        public LoansApiController(FirestoreDb db) => _db = db;

        // POST api/LoansApi  (clients submit; the application is linked to THEIR account)
        [HttpPost]
        [FirebaseAuthorize(Roles.Client)]
        public async Task<IActionResult> Submit([FromBody] SubmitLoanRequest req)
        {
            var loan = new LoanApplicationModel
            {
                ApplicationId = Guid.NewGuid().ToString(),
                UserId = HttpContext.GetUserId(),        // from the verified token, never from the body
                UserEmail = HttpContext.GetEmail(),
                Status = LoanStatus.Submitted,
                ApplicationDate = Timestamp.GetCurrentTimestamp(),
                RequestedAmount = req.RequestedAmount,
                ReasonForLoan = req.ReasonForLoan.Trim(),
                ClientDetails = new ClientDetailsModel
                {
                    FullNameAndSurname = req.ClientDetails.FullNameAndSurname.Trim(),
                    IdNumber = req.ClientDetails.IdNumber,
                    CellNo = req.ClientDetails.CellNo
                }
            };

            await _db.Collection(Collection).Document(loan.ApplicationId).CreateAsync(loan);
            return CreatedAtAction(nameof(GetById), new { id = loan.ApplicationId }, loan);
        }

        // GET api/LoansApi/mine  (same result on web and mobile because it's keyed on the Firebase UID)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var snap = await _db.Collection(Collection)
                .WhereEqualTo("UserId", HttpContext.GetUserId())
                .GetSnapshotAsync();

            return Ok(ToList(snap));
        }

        // GET api/LoansApi?status=Submitted  (staff: everything, optional filter)
        [HttpGet]
        [FirebaseAuthorize(Roles.Admin, Roles.Management)]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            Query q = _db.Collection(Collection);
            if (!string.IsNullOrWhiteSpace(status))
            {
                var canonical = LoanStatus.All.FirstOrDefault(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
                if (canonical == null) return BadRequest(new { error = "Unknown status." });
                q = q.WhereEqualTo("Status", canonical);
            }
            return Ok(ToList(await q.GetSnapshotAsync()));
        }

        // GET api/LoansApi/pending  (staff)
        [HttpGet("pending")]
        [FirebaseAuthorize(Roles.Admin, Roles.Management)]
        public async Task<IActionResult> GetPending()
        {
            var snap = await _db.Collection(Collection)
                .WhereEqualTo("Status", LoanStatus.Submitted)
                .GetSnapshotAsync();
            return Ok(ToList(snap));
        }

        // GET api/LoansApi/{id}  (owner or staff)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var snap = await _db.Collection(Collection).Document(id).GetSnapshotAsync();
            if (!snap.Exists) return NotFound();

            var loan = snap.ConvertTo<LoanApplicationModel>();
            bool allowed = loan.UserId == HttpContext.GetUserId() || Roles.IsStaff(HttpContext.GetRole());
            return allowed ? Ok(loan) : NotFound(); // 404 so we don't reveal that other people's IDs exist
        }

        // PUT api/LoansApi/{id}/status  (staff review)
        [HttpPut("{id}/status")]
        [FirebaseAuthorize(Roles.Admin, Roles.Management)]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateStatusRequest req)
        {
            var canonical = LoanStatus.All.FirstOrDefault(s => s.Equals(req.Status, StringComparison.OrdinalIgnoreCase));
            if (canonical == null) return BadRequest(new { error = "Status must be one of: " + string.Join(", ", LoanStatus.All) });

            var docRef = _db.Collection(Collection).Document(id);
            if (!(await docRef.GetSnapshotAsync()).Exists) return NotFound();

            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "Status", canonical },
                { "ReviewedBy", HttpContext.GetUserId() },
                { "ReviewedAt", Timestamp.GetCurrentTimestamp() },
                { "ReviewNote", req.Note ?? "" }
            });

            return Ok(new { applicationId = id, status = canonical });
        }

        private static List<LoanApplicationModel> ToList(QuerySnapshot snap) =>
            snap.Documents
                .Where(d => d.Exists)
                .Select(d => d.ConvertTo<LoanApplicationModel>())
                .OrderByDescending(l => l.ApplicationDate.ToDateTime())
                .ToList();
    }
}
