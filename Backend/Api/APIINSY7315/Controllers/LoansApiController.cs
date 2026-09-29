using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public LoansApiController()
        {
            // Ensure your GOOGLE_APPLICATION_CREDENTIALS environment variable is set
            // or pass the path to your Firebase service account JSON key file here.
            string projectId = "bridge-anchor-loans"; // Replace with your actual Firebase Project ID
            try
            {
                _firestoreDb = FirestoreDb.Create(projectId);
            }
            catch
            {
                // Fallback for initialization during local mocking if needed
                _firestoreDb = null;
            }
        }

        // GET: api/LoansApi/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingApplications()
        {
            if (_firestoreDb == null)
            {
                return BadRequest(new { status = "Error", message = "Database connection not initialized." });
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
                // Simulated success if Firestore is not locally configured yet
                return Ok(new { status = "Success", message = "Loan application received (Mock Mode)" });
            }

            DocumentReference docRef = _firestoreDb.Collection("LoanApplications").Document(loanData.ApplicationId);
            await docRef.SetAsync(loanData);

            return Ok(new { status = "Success", message = "Loan application successfully stored in Firebase Firestore", applicationId = loanData.ApplicationId });
        }
    }
}