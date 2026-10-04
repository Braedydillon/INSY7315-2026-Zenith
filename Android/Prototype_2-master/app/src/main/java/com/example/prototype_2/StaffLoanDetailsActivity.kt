package com.example.prototype_2

import android.annotation.SuppressLint
import android.content.res.ColorStateList
import android.graphics.Color
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

    // Stores current loan details
    private var currentLoanAmount: Double? = null
    private var currentStatus: String? = null

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

        // -------------------------
        // FIND VIEWS
        // -------------------------

        applicationId =
            findViewById(R.id.ApplicationId)

        applicationStatus =
            findViewById(R.id.ApplicationStatus)

        submissionDate =
            findViewById(R.id.SubmissionDate)

        clientName =
            findViewById(R.id.ClientName)

        clientDetails =
            findViewById(R.id.ClientDetails)

        loanAmount =
            findViewById(R.id.LoanAmount)

        loanReason =
            findViewById(R.id.LoanReason)

        loanNote =
            findViewById(R.id.LoanNote)

        verifyButton =
            findViewById(R.id.btnApproveLoan)

        // -------------------------
        // BACK BUTTON
        // -------------------------

        findViewById<View>(R.id.btnBack).setOnClickListener {
            finish()
        }

        // -------------------------
        // GET LOAN ID
        // -------------------------

        loanId =
            intent.getStringExtra("LOAN_ID")

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

        // -------------------------
        // LOAD LOAN
        // -------------------------

        loadLoanDetails()
    }

    private fun getAuthToken(): String? {

        var token = RetrofitClient.authToken

        if (token.isNullOrEmpty()) {

            val prefsApp =
                getSharedPreferences("AppPrefs", MODE_PRIVATE)

            token =
                prefsApp.getString("AUTH_TOKEN", null)
        }

        if (token.isNullOrEmpty()) {

            val prefsAuth =
                getSharedPreferences("Auth", MODE_PRIVATE)

            token =
                prefsAuth.getString("AUTH_TOKEN", null)
                    ?: prefsAuth.getString("token", null)
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

                val rawToken =
                    getAuthToken()

                if (rawToken.isNullOrEmpty()) {

                    withContext(Dispatchers.Main) {

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Please log in again",
                            Toast.LENGTH_SHORT
                        ).show()
                    }

                    return@launch
                }

                val bearerToken =
                    if (rawToken.startsWith("Bearer ", ignoreCase = true)) {
                        rawToken
                    } else {
                        "Bearer $rawToken"
                    }

                val response =
                    RetrofitClient.apiService.getLoanById(
                        token = bearerToken,
                        id = loanId!!
                    )

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        val loan =
                            response.body()

                        if (loan != null) {

                            // -------------------------
                            // SAVE LOAN DETAILS
                            // -------------------------

                            currentLoanAmount =
                                loan.requestedAmount

                            currentStatus =
                                loan.status ?: "Pending"

                            val amount =
                                currentLoanAmount ?: 0.0

                            val status =
                                currentStatus ?: "Pending"

                            // -------------------------
                            // APPLICATION INFORMATION
                            // -------------------------

                            applicationId.text =
                                "Application ID: ${
                                    loan.applicationId ?: "--"
                                }"

                            applicationStatus.text =
                                "Status: $status"

                            submissionDate.text =
                                "Submission Date: ${
                                    loan.applicationDate ?: "--"
                                }"

                            // -------------------------
                            // CLIENT INFORMATION
                            // -------------------------

                            val name =
                                loan.clientDetails
                                    ?.fullNameAndSurname
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
                                            Locale("en", "ZA"),
                                            "%,.2f",
                                            loan.requestedAmount
                                        )
                                    }"

                                } else {

                                    "Requested Amount: R --"
                                }

                            // -------------------------
                            // LOAN REASON
                            // -------------------------

                            loanReason.text =
                                "Reason for Loan: ${
                                    loan.reasonForLoan
                                        ?: loan.reasonsForLoan?.joinToString(", ")
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
                            // BUTTON STATE & ACTIONS
                            // -------------------------

                            updateButtonForState(
                                amount = amount,
                                status = status
                            )

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

    // -------------------------
    // UPDATE BUTTON STATE
    // -------------------------

    private fun updateButtonForState(
        amount: Double,
        status: String
    ) {

        if (status.equals("Approved", ignoreCase = true)) {

            verifyButton.text =
                "Loan Approved"

            verifyButton.isEnabled =
                false

            verifyButton.backgroundTintList =
                ColorStateList.valueOf(Color.parseColor("#059669"))

        } else if (status.equals("Verified", ignoreCase = true)) {

            if (amount < 7000) {

                // Step 2 for < R7,000: Already verified, now ready to Approve
                verifyButton.text =
                    "Approve Loan"

                verifyButton.isEnabled =
                    true

                verifyButton.backgroundTintList =
                    ColorStateList.valueOf(Color.parseColor("#059669"))

                verifyButton.setOnClickListener {
                    approveLoan()
                }

            } else {

                // Step 2 for >= R7,000: Already verified for Manager
                verifyButton.text =
                    "Loan Verified (Awaiting Manager Approval)"

                verifyButton.isEnabled =
                    false

                verifyButton.backgroundTintList =
                    ColorStateList.valueOf(Color.parseColor("#6B7280"))
            }

        } else {

            // Step 1: Pending / Submitted - Verify details
            verifyButton.text =
                "Verify Details"

            verifyButton.isEnabled =
                true

            verifyButton.backgroundTintList =
                ColorStateList.valueOf(Color.parseColor("#2563EB"))

            verifyButton.setOnClickListener {
                verifyLoanDetails()
            }
        }
    }

    // -------------------------
    // STEP 1: VERIFY LOAN
    // -------------------------

    private fun verifyLoanDetails() {

        val amount =
            currentLoanAmount ?: 0.0

        currentStatus = "Verified"

        applicationStatus.text =
            "Status: Verified"

        Toast.makeText(
            this@StaffLoanDetailsActivity,
            "Loan details verified successfully",
            Toast.LENGTH_SHORT
        ).show()

        updateButtonForState(
            amount = amount,
            status = "Verified"
        )
    }

    // -------------------------
    // STEP 2: APPROVE LOAN
    // -------------------------

    private fun approveLoan() {

        val id =
            loanId ?: return

        val amount =
            currentLoanAmount ?: 0.0

        // Security check: Staff can only approve loans < R7,000
        if (amount >= 7000) {

            Toast.makeText(
                this,
                "Action not permitted for this loan amount.",
                Toast.LENGTH_SHORT
            ).show()

            return
        }

        val rawToken =
            getAuthToken()

        if (rawToken.isNullOrEmpty()) {

            Toast.makeText(
                this,
                "Please log in again",
                Toast.LENGTH_SHORT
            ).show()

            return
        }

        val bearerToken =
            if (rawToken.startsWith("Bearer ", ignoreCase = true)) {
                rawToken
            } else {
                "Bearer $rawToken"
            }

        verifyButton.isEnabled = false

        CoroutineScope(Dispatchers.IO).launch {

            try {

                val request =
                    UpdateStatusRequest(
                        status = "Approved",
                        comments = "Loan approved (< R7,000)."
                    )

                val response =
                    RetrofitClient.apiService.updateLoanStatus(
                        token = bearerToken,
                        id = id,
                        request = request
                    )

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        currentStatus = "Approved"

                        applicationStatus.text =
                            "Status: Approved"

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            "Loan approved successfully",
                            Toast.LENGTH_SHORT
                        ).show()

                        updateButtonForState(
                            amount = amount,
                            status = "Approved"
                        )

                    } else {

                        verifyButton.isEnabled =
                            true

                        val error =
                            response.errorBody()?.string()

                        android.util.Log.e(
                            "STAFF_APPROVE",
                            "Code: ${response.code()}, Error: $error"
                        )

                        val errorMsg =
                            if (response.code() == 401 || response.code() == 403) {
                                "Session expired. Please log in again."
                            } else if (!error.isNullOrEmpty()) {
                                "Could not approve loan (${response.code()}): $error"
                            } else {
                                "Could not approve loan (${response.code()})"
                            }

                        Toast.makeText(
                            this@StaffLoanDetailsActivity,
                            errorMsg,
                            Toast.LENGTH_LONG
                        ).show()
                    }
                }

            } catch (e: Exception) {

                android.util.Log.e(
                    "STAFF_APPROVE",
                    "Error approving loan",
                    e
                )

                withContext(Dispatchers.Main) {

                    verifyButton.isEnabled =
                        true

                    Toast.makeText(
                        this@StaffLoanDetailsActivity,
                        "Error approving the loan: ${e.localizedMessage}",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }
}