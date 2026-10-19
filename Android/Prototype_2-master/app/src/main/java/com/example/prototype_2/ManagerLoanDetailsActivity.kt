package com.example.prototype_2

import android.annotation.SuppressLint
import android.os.Bundle
import android.view.View
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
import java.util.Locale

class ManagerLoanDetailsActivity : AppCompatActivity() {

    private lateinit var applicationId: TextView
    private lateinit var applicationStatus: TextView
    private lateinit var submissionDate: TextView

    private lateinit var clientName: TextView
    private lateinit var clientDetails: TextView

    private lateinit var loanAmount: TextView
    private lateinit var loanReason: TextView
    private lateinit var loanNote: TextView

    private lateinit var btnApprove: Button
    private lateinit var btnReject: Button

    private var loanId: String? = null

    private var currentLoanAmount: Double? = null
    private var currentStatus: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_manager_loan_details)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        // FIND VIEWS
        applicationId = findViewById(R.id.ApplicationId)
        applicationStatus = findViewById(R.id.ApplicationStatus)
        submissionDate = findViewById(R.id.SubmissionDate)

        clientName = findViewById(R.id.ClientName)
        clientDetails = findViewById(R.id.ClientDetails)

        loanAmount = findViewById(R.id.LoanAmount)
        loanReason = findViewById(R.id.LoanReason)
        loanNote = findViewById(R.id.LoanNote)

        btnApprove = findViewById(R.id.btnApprove)
        btnReject = findViewById(R.id.btnReject)

        findViewById<View>(R.id.btnBack).setOnClickListener {
            finish()
        }

        loanId = intent.getStringExtra("LOAN_ID")

        if (loanId.isNullOrEmpty()) {
            Toast.makeText(this, "Loan application could not be found", Toast.LENGTH_SHORT).show()
            finish()
            return
        }

        btnApprove.setOnClickListener {
            updateLoanStatus("Approved")
        }

        btnReject.setOnClickListener {
            updateLoanStatus("Rejected")
        }

        loadLoanDetails()
    }

    private fun getAuthToken(): String? {
        var token = RetrofitClient.authToken

        if (token.isNullOrEmpty()) {
            val prefsApp = getSharedPreferences("AppPrefs", MODE_PRIVATE)
            token = prefsApp.getString("AUTH_TOKEN", null)
        }

        if (token.isNullOrEmpty()) {
            val prefsAuth = getSharedPreferences("Auth", MODE_PRIVATE)
            token = prefsAuth.getString("AUTH_TOKEN", null) ?: prefsAuth.getString("token", null)
        }

        if (!token.isNullOrEmpty()) {
            RetrofitClient.authToken = token
        }

        return token
    }

    @SuppressLint("SetTextI18n")
    private fun loadLoanDetails() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val rawToken = getAuthToken()

                if (rawToken.isNullOrEmpty()) {
                    withContext(Dispatchers.Main) {
                        Toast.makeText(this@ManagerLoanDetailsActivity, "Please log in again", Toast.LENGTH_SHORT).show()
                    }
                    return@launch
                }

                val bearerToken = if (rawToken.startsWith("Bearer ", ignoreCase = true)) {
                    rawToken
                } else {
                    "Bearer $rawToken"
                }

                val response = RetrofitClient.apiService.getLoanById(
                    token = bearerToken,
                    id = loanId!!
                )

                withContext(Dispatchers.Main) {
                    if (response.isSuccessful) {
                        val loan = response.body()

                        if (loan != null) {
                            currentLoanAmount = loan.requestedAmount
                            currentStatus = loan.status ?: "Pending"

                            val status = currentStatus ?: "Pending"

                            applicationId.text = "Application ID: ${loan.applicationId ?: "--"}"
                            applicationStatus.text = "Status: $status"
                            submissionDate.text = "Submission Date: ${loan.applicationDate ?: "--"}"

                            val name = loan.clientDetails?.fullNameAndSurname ?: "--"
                            clientName.text = "Client Name: $name"

                            clientDetails.text = """
                                ID Number: ${loan.clientDetails?.idNumber ?: "--"}
                                
                                Cell Number: ${loan.clientDetails?.cellNo ?: "--"}
                                
                                Home Telephone: ${loan.homeTelNo ?: "--"}
                                
                                Marital Status: ${loan.marriedOrUnmarried ?: "--"}
                                
                                Current Address: ${loan.currentPhysicalAddress ?: "--"}
                                
                                Postal Address: ${loan.postalAddress ?: "--"}
                                
                                Parents Address: ${loan.parentsAddress ?: "--"}
                                
                                Residence: ${loan.residenceYears ?: "--"} years ${loan.residenceMonths ?: "--"} months
                                
                                Company: ${loan.companyName ?: "--"}
                                
                                Occupation: ${loan.occupation ?: "--"}
                                
                                Work Telephone: ${loan.workTelephone ?: "--"}
                                
                                Work Address: ${loan.workAddress ?: "--"}
                                
                                Bank: ${loan.bankName ?: "--"}
                                
                                Account Type: ${loan.accountType ?: "--"}
                                
                                Account Number: ${loan.accountNumber ?: "--"}
                                
                                Account Name: ${loan.accountName ?: "--"}
                                
                                Branch Name: ${loan.branchName ?: "--"}
                                
                                Branch Code: ${loan.branchCode ?: "--"}
                            """.trimIndent()

                            loanAmount.text = if (loan.requestedAmount != null) {
                                "Requested Amount: R ${String.format(Locale("en", "ZA"), "%,.2f", loan.requestedAmount)}"
                            } else {
                                "Requested Amount: R --"
                            }

                            loanReason.text = "Reason for Loan: ${loan.reasonForLoan ?: loan.reasonsForLoan?.joinToString(", ") ?: "--"}"

                            loanNote.text = if (!loan.reviewNote.isNullOrEmpty()) {
                                "Review Note: ${loan.reviewNote}"
                            } else {
                                "Review Note: --"
                            }

                            updateButtonsForStatus(status)

                        } else {
                            Toast.makeText(this@ManagerLoanDetailsActivity, "Loan application could not be found", Toast.LENGTH_SHORT).show()
                        }
                    } else {
                        Toast.makeText(this@ManagerLoanDetailsActivity, "Could not load the loan application (${response.code()})", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@ManagerLoanDetailsActivity, "Error loading application", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    private fun updateButtonsForStatus(status: String) {
        if (status.equals("Approved", ignoreCase = true)) {
            btnApprove.text = "Approved"
            btnApprove.isEnabled = false
            btnApprove.visibility = View.VISIBLE
            btnReject.visibility = View.GONE
        } else if (status.equals("Rejected", ignoreCase = true)) {
            btnReject.text = "Rejected"
            btnReject.isEnabled = false
            btnReject.visibility = View.VISIBLE
            btnApprove.visibility = View.GONE
        } else {
            btnApprove.text = "Approve"
            btnApprove.isEnabled = true
            btnApprove.visibility = View.VISIBLE
            btnReject.text = "Reject"
            btnReject.isEnabled = true
            btnReject.visibility = View.VISIBLE
        }
    }

    private fun updateLoanStatus(newStatus: String) {
        val id = loanId ?: return
        val rawToken = getAuthToken()

        if (rawToken.isNullOrEmpty()) {
            Toast.makeText(this, "Please log in again", Toast.LENGTH_SHORT).show()
            return
        }

        val bearerToken = if (rawToken.startsWith("Bearer ", ignoreCase = true)) {
            rawToken
        } else {
            "Bearer $rawToken"
        }

        btnApprove.isEnabled = false
        btnReject.isEnabled = false

        CoroutineScope(Dispatchers.IO).launch {
            try {
                val comments = if (newStatus.equals("Approved", ignoreCase = true)) {
                    "Application approved by manager."
                } else {
                    "Application rejected by manager."
                }

                val request = UpdateStatusRequest(
                    status = newStatus,
                    comments = comments
                )

                val response = RetrofitClient.apiService.updateLoanStatus(
                    token = bearerToken,
                    id = id,
                    request = request
                )

                withContext(Dispatchers.Main) {
                    if (response.isSuccessful) {
                        currentStatus = newStatus
                        applicationStatus.text = "Status: $newStatus"
                        updateButtonsForStatus(newStatus)
                        Toast.makeText(this@ManagerLoanDetailsActivity, "Application $newStatus", Toast.LENGTH_SHORT).show()
                    } else {
                        updateButtonsForStatus(currentStatus ?: "Pending")
                        Toast.makeText(this@ManagerLoanDetailsActivity, "Could not update application (${response.code()})", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    updateButtonsForStatus(currentStatus ?: "Pending")
                    Toast.makeText(this@ManagerLoanDetailsActivity, "Error updating application", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
}