package com.example.prototype_2

import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.CheckBox
import android.widget.EditText
import android.widget.RadioButton
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.prototype_2.API.ClientDetailsRequest
import com.example.prototype_2.API.RelativeDetailsRequest
import com.example.prototype_2.API.SpouseDetailsRequest
import com.example.prototype_2.API.SubmitLoanRequest
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import androidx.activity.enableEdgeToEdge
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.lifecycle.lifecycleScope
import com.example.prototype_2.API.RetrofitClient
import kotlinx.coroutines.launch

class LoanApplicationActivity : AppCompatActivity() {

    private lateinit var etFullName: EditText
    private lateinit var etIdNumber: EditText
    private lateinit var etHomeTelephone: EditText
    private lateinit var etCellNumber: EditText

    private lateinit var rbMarried: RadioButton
    private lateinit var rbUnmarried: RadioButton

    private lateinit var etMaritalCommunity: EditText
    private lateinit var cbDivorcedYes: CheckBox
    private lateinit var cbDivorcedNo: CheckBox
    private lateinit var etDivorceYear: EditText
    private lateinit var etDivorceCommunity: EditText

    private lateinit var etPhysicalAddress: EditText
    private lateinit var etPostalAddress: EditText
    private lateinit var etParentsAddress: EditText
    private lateinit var etResidenceYears: EditText
    private lateinit var etResidenceMonths: EditText

    private lateinit var etCompanyName: EditText
    private lateinit var etWorkTelephone: EditText
    private lateinit var etOccupation: EditText
    private lateinit var etWorkAddress: EditText

    private lateinit var etBankName: EditText
    private lateinit var etAccountType: EditText
    private lateinit var etAccountNumber: EditText
    private lateinit var etBranchName: EditText
    private lateinit var etAccountName: EditText
    private lateinit var etBranchCode: EditText

    private lateinit var etSpouseName: EditText
    private lateinit var etSpouseIdNumber: EditText
    private lateinit var etSpouseTelephone: EditText
    private lateinit var etSpouseEmployerName: EditText
    private lateinit var etSpouseEmployerAddress: EditText
    private lateinit var etSpouseEmployerTelephone: EditText

    private lateinit var etRelative1Name: EditText
    private lateinit var etRelative1Relationship: EditText
    private lateinit var etRelative1Telephone: EditText
    private lateinit var etRelative1Address: EditText

    private lateinit var etRelative2Name: EditText
    private lateinit var etRelative2Relationship: EditText
    private lateinit var etRelative2Telephone: EditText
    private lateinit var etRelative2Address: EditText

    private lateinit var etLoanAmount: EditText
    private lateinit var cbBusiness: CheckBox
    private lateinit var cbEducation: CheckBox
    private lateinit var cbConsumption: CheckBox
    private lateinit var cbHousing: CheckBox
    private lateinit var cbFurniture: CheckBox
    private lateinit var cbOther: CheckBox
    private lateinit var etOtherPurpose: EditText

    private lateinit var etApplicantSignature: EditText
    private lateinit var etApplicationDate: EditText

    private lateinit var btnSubmitApplication: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_loan_application)

        RetrofitClient.loadToken(this)

        initialiseViews()

        btnSubmitApplication.setOnClickListener {
            submitApplication()
        }

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(android.R.id.content)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
    }

    private fun initialiseViews() {

        etFullName = findViewById(R.id.etFullName)
        etIdNumber = findViewById(R.id.etIdNumber)
        etHomeTelephone = findViewById(R.id.etHomeTelephone)
        etCellNumber = findViewById(R.id.etCellNumber)

        rbMarried = findViewById(R.id.rbMarried)
        rbUnmarried = findViewById(R.id.rbUnmarried)

        etMaritalCommunity = findViewById(R.id.etMaritalCommunity)
        cbDivorcedYes = findViewById(R.id.cbDivorcedYes)
        cbDivorcedNo = findViewById(R.id.cbDivorcedNo)
        etDivorceYear = findViewById(R.id.etDivorceYear)
        etDivorceCommunity = findViewById(R.id.etDivorceCommunity)

        etPhysicalAddress = findViewById(R.id.etPhysicalAddress)
        etPostalAddress = findViewById(R.id.etPostalAddress)
        etParentsAddress = findViewById(R.id.etParentsAddress)
        etResidenceYears = findViewById(R.id.etResidenceYears)
        etResidenceMonths = findViewById(R.id.etResidenceMonths)

        etCompanyName = findViewById(R.id.etCompanyName)
        etWorkTelephone = findViewById(R.id.etWorkTelephone)
        etOccupation = findViewById(R.id.etOccupation)
        etWorkAddress = findViewById(R.id.etWorkAddress)

        etBankName = findViewById(R.id.etBankName)
        etAccountType = findViewById(R.id.etAccountType)
        etAccountNumber = findViewById(R.id.etAccountNumber)
        etBranchName = findViewById(R.id.etBranchName)
        etAccountName = findViewById(R.id.etAccountName)
        etBranchCode = findViewById(R.id.etBranchCode)

        etSpouseName = findViewById(R.id.etSpouseName)
        etSpouseIdNumber = findViewById(R.id.etSpouseIdNumber)
        etSpouseTelephone = findViewById(R.id.etSpouseTelephone)
        etSpouseEmployerName = findViewById(R.id.etSpouseEmployerName)
        etSpouseEmployerAddress = findViewById(R.id.etSpouseEmployerAddress)
        etSpouseEmployerTelephone = findViewById(R.id.etSpouseEmployerTelephone)

        etRelative1Name = findViewById(R.id.etRelative1Name)
        etRelative1Relationship = findViewById(R.id.etRelative1Relationship)
        etRelative1Telephone = findViewById(R.id.etRelative1Telephone)
        etRelative1Address = findViewById(R.id.etRelative1Address)

        etRelative2Name = findViewById(R.id.etRelative2Name)
        etRelative2Relationship = findViewById(R.id.etRelative2Relationship)
        etRelative2Telephone = findViewById(R.id.etRelative2Telephone)
        etRelative2Address = findViewById(R.id.etRelative2Address)

        etLoanAmount = findViewById(R.id.etLoanAmount)

        cbBusiness = findViewById(R.id.cbBusiness)
        cbEducation = findViewById(R.id.cbEducation)
        cbConsumption = findViewById(R.id.cbConsumption)
        cbHousing = findViewById(R.id.cbHousing)
        cbFurniture = findViewById(R.id.cbFurniture)
        cbOther = findViewById(R.id.cbOther)

        etOtherPurpose = findViewById(R.id.etOtherPurpose)

        etApplicantSignature = findViewById(R.id.etApplicantSignature)
        etApplicationDate = findViewById(R.id.etApplicationDate)

        btnSubmitApplication = findViewById(R.id.btnSubmitApplication)
    }


    private fun getCurrentDateTime(): String {
        val formatter = java.text.SimpleDateFormat(
            "yyyy-MM-dd'T'HH:mm:ss",
            java.util.Locale.getDefault()
        )

        return formatter.format(java.util.Date())
    }


    private fun submitApplication() {

        if (!validateForm()) {
            return
        }

        val loanAmount = etLoanAmount.text.toString().toDoubleOrNull()

        if (loanAmount == null) {
            Toast.makeText(
                this,
                "Please enter a valid loan amount.",
                Toast.LENGTH_SHORT
            ).show()
            return
        }

        val purpose = getLoanPurpose()

        val divorced = cbDivorcedYes.isChecked

        val divorceYear = etDivorceYear.text.toString().toIntOrNull()

        val residenceYears =
            etResidenceYears.text.toString().toIntOrNull()

        val residenceMonths =
            etResidenceMonths.text.toString().toIntOrNull()

        val clientDetails = ClientDetailsRequest(
            fullNameAndSurname = etFullName.text.toString().trim(),
            idNumber = etIdNumber.text.toString().trim(),
            cellNo = etCellNumber.text.toString().trim(),
            homeTelNo = etHomeTelephone.text.toString().trim(),
            marriedOrUnmarried = "",
            marriageCommunity = etMaritalCommunity.text.toString().trim(),
            previouslyDivorced = if (divorced) "Yes" else "No",
            divorceYear = divorceYear?.toString() ?: "",
            divorceCommunity = etDivorceCommunity.text.toString().trim(),
            currentPhysicalAddress = etPhysicalAddress.text.toString().trim(),
            postalAddress = etPostalAddress.text.toString().trim(),
            parentsAddress = etParentsAddress.text.toString().trim(),
            residenceYears = residenceYears,
            residenceMonths = residenceMonths,
            companyName = etCompanyName.text.toString().trim(),
            occupation = etOccupation.text.toString().trim(),
            workTelephone = etWorkTelephone.text.toString().trim(),
            workAddress = etWorkAddress.text.toString().trim()
        )

        val spouseDetails = SpouseDetailsRequest(
            name = etSpouseName.text.toString().trim(),
            idNumber = etSpouseIdNumber.text.toString().trim(),
            telephone = etSpouseTelephone.text.toString().trim(),
            employerName = etSpouseEmployerName.text.toString().trim(),
            employerAddress = etSpouseEmployerAddress.text.toString().trim(),
            employerTelephone = etSpouseEmployerTelephone.text.toString().trim()
        )

        val relative1 = RelativeDetailsRequest(
            name = etRelative1Name.text.toString().trim(),
            relationship = etRelative1Relationship.text.toString().trim(),
            telephone = etRelative1Telephone.text.toString().trim(),
            address = etRelative1Address.text.toString().trim()
        )

        val relative2 = RelativeDetailsRequest(
            name = etRelative2Name.text.toString().trim(),
            relationship = etRelative2Relationship.text.toString().trim(),
            telephone = etRelative2Telephone.text.toString().trim(),
            address = etRelative2Address.text.toString().trim()
        )

        val request = SubmitLoanRequest(
            requestedAmount = loanAmount,
            reasonForLoan = purpose,
            otherReason = etOtherPurpose.text.toString().trim(),

            clientDetails = clientDetails,

            bankName = etBankName.text.toString().trim(),
            accountType = etAccountType.text.toString().trim(),
            accountNumber = etAccountNumber.text.toString().trim(),
            branchName = etBranchName.text.toString().trim(),
            accountName = etAccountName.text.toString().trim(),
            branchCode = etBranchCode.text.toString().trim(),

            spouseNameAndSurname = etSpouseName.text.toString().trim(),
            spouseIdNumber = etSpouseIdNumber.text.toString().trim(),
            spouseTelNumber = etSpouseTelephone.text.toString().trim(),
            spouseEmployerName = etSpouseEmployerName.text.toString().trim(),
            spouseEmployerAddress = etSpouseEmployerAddress.text.toString().trim(),
            spouseEmployerTelNumber = etSpouseEmployerTelephone.text.toString().trim(),

            relative1Name = etRelative1Name.text.toString().trim(),
            relative1Relationship = etRelative1Relationship.text.toString().trim(),
            relative1TelNumber = etRelative1Telephone.text.toString().trim(),
            relative1Address = etRelative1Address.text.toString().trim(),

            relative2Name = etRelative2Name.text.toString().trim(),
            relative2Relationship = etRelative2Relationship.text.toString().trim(),
            relative2TelNumber = etRelative2Telephone.text.toString().trim(),
            relative2Address = etRelative2Address.text.toString().trim(),

            applicantSignature = etApplicantSignature.text.toString().trim(),
            applicationDate = etApplicationDate.text.toString().trim(),
            applicantFormDate = getCurrentDateTime()
        )

        sendApplicationToApi(request)
    }

    private fun sendApplicationToApi(request: SubmitLoanRequest) {

        btnSubmitApplication.isEnabled = false
        btnSubmitApplication.text = "Submitting..."

        lifecycleScope.launch {

            try {

                val prefs = getSharedPreferences("AppPrefs", MODE_PRIVATE)

                var token = RetrofitClient.authToken

                if (token.isNullOrEmpty()) {
                    token = prefs.getString("AUTH_TOKEN", null)
                }

                if (token.isNullOrEmpty()) {

                    Toast.makeText(
                        this@LoanApplicationActivity,
                        "Please log in again before submitting.",
                        Toast.LENGTH_LONG
                    ).show()

                    return@launch
                }

                RetrofitClient.authToken = token

                Log.d(
                    "LOAN_AUTH",
                    "Token found: ${token.take(20)}..."
                )

                val response = RetrofitClient.apiService.submitLoan(
                    request = request
                )

                if (response.isSuccessful) {

                    Toast.makeText(
                        this@LoanApplicationActivity,
                        "Application submitted successfully.",
                        Toast.LENGTH_LONG
                    ).show()

                    finish()

                } else {

                    val errorBody = response.errorBody()?.string()

                    Log.e(
                        "LOAN_API",
                        "Code: ${response.code()}, Error: $errorBody"
                    )

                    Toast.makeText(
                        this@LoanApplicationActivity,
                        "Submission failed: ${response.code()}",
                        Toast.LENGTH_LONG
                    ).show()
                }

            } catch (e: Exception) {

                Log.e("LOAN_API", "Exception", e)

                Toast.makeText(
                    this@LoanApplicationActivity,
                    "Network error: ${e.localizedMessage}",
                    Toast.LENGTH_LONG
                ).show()

            } finally {

                btnSubmitApplication.isEnabled = true
                btnSubmitApplication.text = "Submit Application"
            }
        }
    }

    private fun validateForm(): Boolean {

        if (etFullName.text.toString().trim().isEmpty()) {
            etFullName.error = "Enter your full name"
            etFullName.requestFocus()
            return false
        }

        if (etIdNumber.text.toString().trim().isEmpty()) {
            etIdNumber.error = "Enter your ID number"
            etIdNumber.requestFocus()
            return false
        }

        if (etCellNumber.text.toString().trim().isEmpty()) {
            etCellNumber.error = "Enter your cell number"
            etCellNumber.requestFocus()
            return false
        }

        if (etPhysicalAddress.text.toString().trim().isEmpty()) {
            etPhysicalAddress.error = "Enter your physical address"
            etPhysicalAddress.requestFocus()
            return false
        }

        if (etLoanAmount.text.toString().trim().isEmpty()) {
            etLoanAmount.error = "Enter the loan amount"
            etLoanAmount.requestFocus()
            return false
        }

        if (getLoanPurpose().isEmpty()) {
            Toast.makeText(
                this,
                "Please select a reason for the loan.",
                Toast.LENGTH_SHORT
            ).show()
            return false
        }

        if (etApplicantSignature.text.toString().trim().isEmpty()) {
            etApplicantSignature.error = "Enter your name as your signature"
            etApplicantSignature.requestFocus()
            return false
        }

        if (etApplicationDate.text.toString().trim().isEmpty()) {
            etApplicationDate.error = "Enter the application date"
            etApplicationDate.requestFocus()
            return false
        }

        return true
    }

    private fun getFirstName(): String {

        val fullName = etFullName.text.toString().trim()
        val parts = fullName.split(" ")

        return if (parts.isNotEmpty()) {
            parts[0]
        } else {
            ""
        }
    }

    private fun getLastName(): String {

        val fullName = etFullName.text.toString().trim()
        val parts = fullName.split(" ")

        return if (parts.size > 1) {
            parts.drop(1).joinToString(" ")
        } else {
            ""
        }
    }

    private fun getMaritalStatus(): String {

        return when {
            rbMarried.isChecked -> "Married"
            rbUnmarried.isChecked -> "Unmarried"
            else -> ""
        }
    }

    private fun getLoanPurpose(): String {

        val purposes = mutableListOf<String>()

        if (cbBusiness.isChecked) {
            purposes.add("Business")
        }

        if (cbEducation.isChecked) {
            purposes.add("Education")
        }

        if (cbConsumption.isChecked) {
            purposes.add("Consumption")
        }

        if (cbHousing.isChecked) {
            purposes.add("Housing")
        }

        if (cbFurniture.isChecked) {
            purposes.add("Furniture")
        }

        if (cbOther.isChecked) {
            purposes.add("Other")
        }

        return purposes.joinToString(", ")
    }
}




