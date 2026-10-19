
package com.example.prototype_2

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.provider.OpenableColumns
import android.util.Log
import android.widget.Button
import android.widget.CheckBox
import android.widget.EditText
import android.widget.RadioButton
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.activity.result.contract.ActivityResultContracts
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.lifecycle.lifecycleScope
import com.example.prototype_2.API.ClientDetailsRequest
import com.example.prototype_2.API.RelativeDetailsRequest
import com.example.prototype_2.API.RetrofitClient
import com.example.prototype_2.API.SpouseDetailsRequest
import com.example.prototype_2.API.SubmitLoanRequest
import kotlinx.coroutines.launch
import java.security.MessageDigest
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

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

    private lateinit var btnUploadId: Button
    private lateinit var btnUploadProofOfAddress: Button
    private lateinit var btnUploadPayslip: Button

    private lateinit var tvIdFile: TextView
    private lateinit var tvAddressFile: TextView
    private lateinit var tvPayslipFile: TextView

    private var idDocumentUri: Uri? = null
    private var addressDocumentUri: Uri? = null
    private var payslipDocumentUri: Uri? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_loan_application)

        RetrofitClient.loadToken(this)
        initialiseViews()

        btnSubmitApplication.setOnClickListener {
            submitApplication()
        }

        ViewCompat.setOnApplyWindowInsetsListener(
            findViewById(android.R.id.content)
        ) { view, insets ->
            val systemBars =
                insets.getInsets(WindowInsetsCompat.Type.systemBars())

            view.setPadding(
                systemBars.left,
                systemBars.top,
                systemBars.right,
                systemBars.bottom
            )

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
        etSpouseEmployerTelephone =
            findViewById(R.id.etSpouseEmployerTelephone)

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

        btnUploadId = findViewById(R.id.btnUploadId)
        btnUploadProofOfAddress =
            findViewById(R.id.btnUploadProofOfAddress)
        btnUploadPayslip = findViewById(R.id.btnUploadPayslip)

        tvIdFile = findViewById(R.id.tvIdFile)
        tvAddressFile = findViewById(R.id.tvAddressFile)
        tvPayslipFile = findViewById(R.id.tvPayslipFile)

        btnUploadId.setOnClickListener {
            idDocumentPicker.launch(arrayOf("application/pdf", "image/*"))
        }

        btnUploadProofOfAddress.setOnClickListener {
            addressDocumentPicker.launch(
                arrayOf("application/pdf", "image/*")
            )
        }

        btnUploadPayslip.setOnClickListener {
            payslipDocumentPicker.launch(
                arrayOf("application/pdf", "image/*")
            )
        }
    }

    // Opens the system file picker and remembers the selected ID document.
    private val idDocumentPicker =
        registerForActivityResult(ActivityResultContracts.OpenDocument()) { uri ->
            if (uri != null) {
                idDocumentUri = uri
                tvIdFile.text = getFileName(uri)
            }
        }

    // Opens the system file picker for proof of address.
    private val addressDocumentPicker =
        registerForActivityResult(ActivityResultContracts.OpenDocument()) { uri ->
            if (uri != null) {
                addressDocumentUri = uri
                tvAddressFile.text = getFileName(uri)
            }
        }

    // Opens the system file picker for the payslip.
    private val payslipDocumentPicker =
        registerForActivityResult(ActivityResultContracts.OpenDocument()) { uri ->
            if (uri != null) {
                payslipDocumentUri = uri
                tvPayslipFile.text = getFileName(uri)
            }
        }

    private fun getFileName(uri: Uri): String {
        contentResolver.query(
            uri,
            arrayOf(OpenableColumns.DISPLAY_NAME),
            null,
            null,
            null
        )?.use { cursor ->
            if (cursor.moveToFirst()) {
                val index =
                    cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME)

                if (index >= 0) {
                    return cursor.getString(index)
                }
            }
        }

        return "Document selected"
    }

    private fun getCurrentDateTime(): String {
        val formatter = SimpleDateFormat(
            "yyyy-MM-dd'T'HH:mm:ss",
            Locale.getDefault()
        )

        return formatter.format(Date())
    }

    private fun submitApplication() {
        if (!validateForm()) {
            return
        }

        val loanAmount =
            etLoanAmount.text.toString().toDoubleOrNull()

        if (loanAmount == null || loanAmount <= 0) {
            etLoanAmount.error = "Enter a valid loan amount"
            etLoanAmount.requestFocus()
            return
        }

        val purpose = getLoanPurpose()
        val divorced = cbDivorcedYes.isChecked
        val divorceYear = etDivorceYear.text.toString().toIntOrNull()
        val residenceYears = etResidenceYears.text.toString().toIntOrNull()
        val residenceMonths = etResidenceMonths.text.toString().toIntOrNull()

        val clientDetails = ClientDetailsRequest(
            fullNameAndSurname = etFullName.text.toString().trim(),
            idNumber = etIdNumber.text.toString().trim(),
            cellNo = etCellNumber.text.toString().trim(),
            homeTelNo = etHomeTelephone.text.toString().trim(),
            marriedOrUnmarried = getMaritalStatus(),
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

        val request = SubmitLoanRequest(
            requestedAmount = loanAmount,
            reasonForLoan = purpose,
            otherReason = etOtherPurpose.text.toString().trim(),
            clientDetails = clientDetails,

            homeTelNo = etHomeTelephone.text.toString().trim(),
            marriedOrUnmarried = getMaritalStatus(),
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
            workAddress = etWorkAddress.text.toString().trim(),

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
            spouseEmployerTelNumber =
                etSpouseEmployerTelephone.text.toString().trim(),

            relative1Name = etRelative1Name.text.toString().trim(),
            relative1Relationship =
                etRelative1Relationship.text.toString().trim(),
            relative1TelNumber = etRelative1Telephone.text.toString().trim(),
            relative1Address = etRelative1Address.text.toString().trim(),

            relative2Name = etRelative2Name.text.toString().trim(),
            relative2Relationship =
                etRelative2Relationship.text.toString().trim(),
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

                // Do not log the authentication token.
                val response = RetrofitClient.apiService.submitLoan(
                    request = request
                )

                if (response.isSuccessful) {
                    /*
                     * Use a consistent local key on the application page
                     * and staff page. This does not change the API.
                     */
                    val applicationKey =
                        createApplicationKey(etIdNumber.text.toString())

                    val documentsSaved =
                        saveSelectedDocuments(applicationKey)

                    if (documentsSaved) {
                        Toast.makeText(
                            this@LoanApplicationActivity,
                            "Application submitted. Selected documents saved locally.",
                            Toast.LENGTH_LONG
                        ).show()
                    } else {
                        Toast.makeText(
                            this@LoanApplicationActivity,
                            "Application submitted, but one or more documents could not be saved.",
                            Toast.LENGTH_LONG
                        ).show()
                    }

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
                Log.e("LOAN_API", "Exception while submitting application", e)

                Toast.makeText(
                    this@LoanApplicationActivity,
                    "Network or document error: ${e.localizedMessage}",
                    Toast.LENGTH_LONG
                ).show()
            } finally {
                btnSubmitApplication.isEnabled = true
                btnSubmitApplication.text = "Submit Application"
            }
        }
    }

    /*
     * The same ID number produces the same local folder key.
     * The staff page must use this exact algorithm too.
     */
    private fun createApplicationKey(idNumber: String): String {
        val normalisedId = idNumber.trim()
        val bytes = MessageDigest.getInstance("SHA-256")
            .digest(normalisedId.toByteArray(Charsets.UTF_8))

        return bytes.joinToString("") { byte ->
            "%02x".format(byte.toInt() and 0xff)
        }
    }

    private fun saveSelectedDocuments(applicationKey: String): Boolean {
        return try {
            idDocumentUri?.let { uri ->
                LocalDocumentStorage.saveDocument(
                    this,
                    applicationKey,
                    "id",
                    uri
                )
            }

            addressDocumentUri?.let { uri ->
                LocalDocumentStorage.saveDocument(
                    this,
                    applicationKey,
                    "proof_of_address",
                    uri
                )
            }

            payslipDocumentUri?.let { uri ->
                LocalDocumentStorage.saveDocument(
                    this,
                    applicationKey,
                    "payslip",
                    uri
                )
            }

            true
        } catch (e: Exception) {
            Log.e(
                "LOAN_DOCUMENT",
                "Failed to save one or more documents",
                e
            )
            false
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

    private fun getMaritalStatus(): String {
        return when {
            rbMarried.isChecked -> "Married"
            rbUnmarried.isChecked -> "Unmarried"
            else -> ""
        }
    }

    private fun getLoanPurpose(): String {
        val purposes = mutableListOf<String>()

        if (cbBusiness.isChecked) purposes.add("Business")
        if (cbEducation.isChecked) purposes.add("Education")
        if (cbConsumption.isChecked) purposes.add("Consumption")
        if (cbHousing.isChecked) purposes.add("Housing")
        if (cbFurniture.isChecked) purposes.add("Furniture")
        if (cbOther.isChecked) purposes.add("Other")

        return purposes.joinToString(", ")
    }
}
