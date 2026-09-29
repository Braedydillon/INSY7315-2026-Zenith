using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using APIINSY7315.Models;

namespace APIINSY7315.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansApiController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly string _initError;

        public LoansApiController()
        {
            string projectId = "insy7315-37442";
            try
            {
                string credentialPath = Path.Combine(AppContext.BaseDirectory, "cred", "firebase-credentials.json");
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

                _firestoreDb = FirestoreDb.Create(projectId);
            }
            catch (Exception ex)
            {
                _firestoreDb = null;
                _initError = ex.Message;
            }
        }

        // GET: api/LoansApi/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingApplications()
        {
            if (_firestoreDb == null)
            {
                return BadRequest(new { status = "Error", message = $"Database connection failed: {_initError}" });
            }

            Query loanQuery = _firestoreDb.Collection("LoanApplications").WhereEqualTo("Status", "Submitted");
            QuerySnapshot querySnapshot = await loanQuery.GetSnapshotAsync();

            var applications = new List<LoanApplicationModel>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    LoanApplicationModel loan = documentSnapshot.ConvertTo<LoanApplicationModel>();
                    applications.Add(loan);
                }
            }

            return Ok(applications);
        }

        // POST: api/LoansApi
        [HttpPost]
        public async Task<IActionResult> SubmitLoanApplication([FromBody] LoanApplicationModel loanData)
        {
            if (loanData == null)
            {
                return BadRequest(new { status = "Error", message = "Invalid loan application data." });
            }

            if (_firestoreDb == null)
            {
                return BadRequest(new { status = "Error", message = $"Database initialization failed: {_initError}" });
            }

            DocumentReference docRef = _firestoreDb.Collection("LoanApplications").Document(loanData.ApplicationId);
            await docRef.SetAsync(loanData);

            return Ok(new { status = "Success", message = "Loan application successfully stored in Firebase Firestore", applicationId = loanData.ApplicationId });
        }
    }
}