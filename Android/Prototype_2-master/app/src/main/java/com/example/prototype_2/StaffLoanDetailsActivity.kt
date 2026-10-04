package com.example.prototype_2

import android.os.Bundle
import android.view.View
import android.widget.Button
import android.widget.ImageButton
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.example.prototype_2.API.RetrofitClient
import com.example.prototype_2.API.UpdateStatusRequest
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class StaffLoanDetailsActivity : AppCompatActivity() {

    private lateinit var applicationId: TextView
    private lateinit var applicationStatus: TextView
    private lateinit var submissionDate: TextView

    private lateinit var clientName: TextView
    private lateinit var clientDetails: TextView

    private lateinit var loanAmount: TextView
    private lateinit var loanReason: TextView
    private lateinit var loanNote: TextView

    private lateinit var verifyButton: Button

    private var loanId: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_staff_loan_details)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars =
                insets.getInsets(WindowInsetsCompat.Type.systemBars())

            v.setPadding(
                systemBars.left,
                systemBars.top,
                systemBars.right,
                systemBars.bottom
            )

            insets
        }

        applicationId = findViewById(R.id.ApplicationId)
        applicationStatus = findViewById(R.id.ApplicationStatus)
        submissionDate = findViewById(R.id.SubmissionDate)

        clientName = findViewById(R.id.ClientName)
        clientDetails = findViewById(R.id.ClientDetails)

        loanAmount = findViewById(R.id.LoanAmount)
        loanReason = findViewById(R.id.LoanReason)
        loanNote = findViewById(R.id.LoanNote)

        verifyButton = findViewById(R.id.btnApproveLoan)

        findViewById<View>(R.id.btnBack).setOnClickListener {
            finish()
        }

        loanId = intent.getStringExtra("LOAN_ID")

        android.util.Log.d(
            "STAFF_DETAILS",
            "Received LOAN_ID: $loanId"
        )

        if (loanId.isNullOrEmpty()) {
            Toast.makeText(
                this,
                "Loan application could not be found",
                Toast.LENGTH_SHORT
            ).show()

            finish()
            return
        }

        loadLoanDetails()
    }

    private fun loadLoanDetails() {

        CoroutineScope(Dispatchers.IO).launch {

            try {

                val token = RetrofitClient.authToken

                if (token.isNullOrEmpty()) {

                    withContext(Dispatchers.Main) {
                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Please log in again",
                            Toast.LENGTH_SHORT
                        ).show()
                    }

                    return@launch
                }

                val response =
                    RetrofitClient.apiService.getLoanById(
                        id = loanId!!
                    )

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        val loan = response.body()

                        if (loan != null) {

                            // -------------------------
                            // APPLICATION INFORMATION
                            // -------------------------

                            applicationId.text =
                                "Application ID: ${loan.applicationId ?: "--"}"

                            applicationStatus.text =
                                "Status: ${loan.status ?: "Pending"}"

                            submissionDate.text =
                                "Submission Date: ${
                                    loan.applicationDate ?: "--"
                                }"

                            // -------------------------
                            // CLIENT INFORMATION
                            // -------------------------

                            val name =
                                loan.clientDetails?.fullNameAndSurname
                                    ?: "--"

                            clientName.text =
                                "Client Name: $name"

                            clientDetails.text =
                                """
                                ID Number: ${
                                    loan.clientDetails?.idNumber ?: "--"
                                }
                                
                                Cell Number: ${
                                    loan.clientDetails?.cellNo ?: "--"
                                }
                                
                                Home Telephone: ${
                                    loan.homeTelNo ?: "--"
                                }
                                
                                Marital Status: ${
                                    loan.marriedOrUnmarried ?: "--"
                                }
                                
                                Current Address: ${
                                    loan.currentPhysicalAddress ?: "--"
                                }
                                
                                Postal Address: ${
                                    loan.postalAddress ?: "--"
                                }
                                
                                Parents Address: ${
                                    loan.parentsAddress ?: "--"
                                }
                                
                                Residence: ${
                                    loan.residenceYears ?: "--"
                                } years ${
                                    loan.residenceMonths ?: "--"
                                } months
                                
                                Company: ${
                                    loan.companyName ?: "--"
                                }
                                
                                Occupation: ${
                                    loan.occupation ?: "--"
                                }
                                
                                Work Telephone: ${
                                    loan.workTelephone ?: "--"
                                }
                                
                                Work Address: ${
                                    loan.workAddress ?: "--"
                                }
                                
                                Bank: ${
                                    loan.bankName ?: "--"
                                }
                                
                                Account Type: ${
                                    loan.accountType ?: "--"
                                }
                                
                                Account Number: ${
                                    loan.accountNumber ?: "--"
                                }
                                
                                Account Name: ${
                                    loan.accountName ?: "--"
                                }
                                
                                Branch Name: ${
                                    loan.branchName ?: "--"
                                }
                                
                                Branch Code: ${
                                    loan.branchCode ?: "--"
                                }
                                """.trimIndent()

                            // -------------------------
                            // LOAN INFORMATION
                            // -------------------------

                            loanAmount.text =
                                if (loan.requestedAmount != null) {
                                    "Requested Amount: R ${
                                        String.format(
                                            "%,.2f",
                                            loan.requestedAmount
                                        )
                                    }"
                                } else {
                                    "Requested Amount: R --"
                                }

                            loanReason.text =
                                "Reason for Loan: ${
                                    loan.reasonForLoan
                                        ?: loan.reasonsForLoan
                                        ?: "--"
                                }"

                            // -------------------------
                            // REVIEW INFORMATION
                            // -------------------------

                            loanNote.text =
                                if (!loan.reviewNote.isNullOrEmpty()) {
                                    "Review Note: ${loan.reviewNote}"
                                } else {
                                    "Review Note: --"
                                }

                            // -------------------------
                            // APPROVE / VERIFY BUTTON
                            // -------------------------

                            if (
                                loan.status.equals(
                                    "Verified",
                                    ignoreCase = true
                                ) ||
                                loan.status.equals(
                                    "Approved",
                                    ignoreCase = true
                                )
                            ) {

                                verifyButton.isEnabled = false
                                verifyButton.text = "Loan Approved"

                            } else {

                                verifyButton.isEnabled = true
                                verifyButton.text = "Approve Loan"
                            }

                            verifyButton.setOnClickListener {
                                verifyLoanDetails()
                            }

                        } else {

                            Toast.makeText(
                                this@StaffLoanDetailsActivity,
                                "Loan application could not be found",
                                Toast.LENGTH_SHORT
                            ).show()
                        }

                    } else {

                        val error =
                            response.errorBody()?.string()

                        android.util.Log.e(
                            "STAFF_DETAILS",
                            "Code: ${response.code()}, Error: $error"
                        )

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Could not load the loan application (${response.code()})",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }

            } catch (e: Exception) {

                android.util.Log.e(
                    "STAFF_DETAILS",
                    "Error loading application",
                    e
                )

                withContext(Dispatchers.Main) {

                    Toast.makeText(
                        this@StaffLoanDetailsActivity,
                        "Error loading application",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }

    private fun verifyLoanDetails() {

        val id = loanId ?: return

        verifyButton.isEnabled = false

        CoroutineScope(Dispatchers.IO).launch {

            try {

                val request =
                    UpdateStatusRequest(
                        status = "Approved",
                        comments = "Staff approved the loan application."
                    )

                val response =
                    RetrofitClient.apiService.updateLoanStatus(
                        id = id,
                        request = request
                    )

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        applicationStatus.text =
                            "Status: Approved"

                        verifyButton.text =
                            "Loan Approved"

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Loan approved successfully",
                            Toast.LENGTH_SHORT
                        ).show()

                    } else {

                        verifyButton.isEnabled = true
                        verifyButton.text = "Approve Loan"

                        val error =
                            response.errorBody()?.string()

                        android.util.Log.e(
                            "STAFF_VERIFY",
                            "Code: ${response.code()}, Error: $error"
                        )

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Could not verify the loan details",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }

            } catch (e: Exception) {

                android.util.Log.e(
                    "STAFF_VERIFY",
                    "Error verifying loan",
                    e
                )

                withContext(Dispatchers.Main) {

                    verifyButton.isEnabled = true
                    verifyButton.text = "Approve Loan"

                    Toast.makeText(
                        this@StaffLoanDetailsActivity,
                        "Error approving the loan",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }
}