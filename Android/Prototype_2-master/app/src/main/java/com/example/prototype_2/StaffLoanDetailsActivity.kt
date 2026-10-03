package com.example.prototype_2

import android.os.Bundle
import android.widget.Button
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

        verifyButton = findViewById(R.id.btnApproveLoan) // button ID in XML layout activity_staff_loan_details.xml
        verifyButton.text = "Verify Details"

        loanId = intent.getStringExtra("LOAN_ID")

        findViewById<Button>(R.id.btnBack).setOnClickListener {
            finish()
        }

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

                val token = getSharedPreferences("Auth", MODE_PRIVATE)
                    .getString("token", null)

                val response = RetrofitClient.apiService.getLoanById(
                    token = if (token != null) {
                        "Bearer $token"
                    } else {
                        null
                    },
                    id = loanId!!
                )

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        val loan = response.body()

                        if (loan != null) {

                            applicationId.text =
                                "Application ID: ${loan.id ?: "--"}"

                            applicationStatus.text =
                                "Status: ${loan.status ?: "Pending"}"

                            submissionDate.text =
                                "Submission Date: ${loan.submissionDate ?: "--"}"

                            clientName.text =
                                "Client Name: ${loan.applicantName ?: "--"}"

                            clientDetails.text =
                                if (loan.clientDetails != null) {
                                    "Client Details: ${loan.clientDetails}"
                                } else {
                                    "Client Details: --"
                                }

                            val amount =
                                loan.requestedAmount ?: loan.amount

                            loanAmount.text =
                                if (amount != null) {
                                    "Requested Amount: R ${
                                        String.format("%,.2f", amount)
                                    }"
                                } else {
                                    "Requested Amount: R --"
                                }

                            loanReason.text =
                                "Reason for Loan: ${
                                    loan.reasonForLoan ?: "--"
                                }"

                            loanNote.text =
                                "Additional Notes: ${
                                    loan.note ?: "--"
                                }"

                            if (loan.status.equals(
                                    "Verified",
                                    ignoreCase = true
                                ) || loan.status.equals(
                                    "Approved",
                                    ignoreCase = true
                                )
                            ) {

                                verifyButton.isEnabled = false
                                verifyButton.text = "Details Verified"

                            } else {

                                verifyButton.isEnabled = true
                                verifyButton.text = "Verify Details"
                            }

                            verifyButton.setOnClickListener {
                                verifyLoanDetails()
                            }
                        }

                    } else {

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Could not load the loan application",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }

            } catch (e: Exception) {

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

                val token = getSharedPreferences("Auth", MODE_PRIVATE)
                    .getString("token", null)

                val request = UpdateStatusRequest(
                    status = "Verified",
                    comments = "Staff verified that all details are correct."
                )

                val response =
                    RetrofitClient.apiService.updateLoanStatus(
                        token = if (token != null) {
                            "Bearer $token"
                        } else {
                            null
                        },
                        id = id,
                        request = request
                    )

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        applicationStatus.text =
                            "Status: Verified"

                        verifyButton.text =
                            "Details Verified"

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Loan details verified successfully",
                            Toast.LENGTH_SHORT
                        ).show()

                    } else {

                        verifyButton.isEnabled = true
                        verifyButton.text = "Verify Details"

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Could not verify the loan details",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }

            } catch (e: Exception) {

                withContext(Dispatchers.Main) {

                    verifyButton.isEnabled = true
                    verifyButton.text = "Verify Details"

                    Toast.makeText(
                        this@StaffLoanDetailsActivity,
                        "Error verifying the loan details",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }
}
