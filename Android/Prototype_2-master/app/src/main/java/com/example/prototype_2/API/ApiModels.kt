package com.example.prototype_2.API

import com.google.gson.annotations.SerializedName

// ==========================================
// Request Schemas (Strict matching swagger.json)
// ==========================================

data class ClientDetailsRequest(
    @SerializedName("fullNameAndSurname") val fullNameAndSurname: String,
    @SerializedName("idNumber") val idNumber: String,
    @SerializedName("cellNo") val cellNo: String
)

data class LoginRequest(
    @SerializedName("email") val email: String,
    @SerializedName("password") val password: String
)

data class RefreshRequest(
    @SerializedName("refreshToken") val refreshToken: String
)

data class RegisterRequest(
    @SerializedName("email") val email: String,
    @SerializedName("fullName") val fullName: String,
    @SerializedName("password") val password: String
)

data class RoleAssignmentRequest(
    @SerializedName("email") val email: String,
    @SerializedName("role") val role: String
)

data class SubmitLoanRequest(
    @SerializedName("reasonForLoan") val reasonForLoan: String,
    @SerializedName("clientDetails") val clientDetails: ClientDetailsRequest,
    @SerializedName("requestedAmount") val requestedAmount: Double? = null,
    @SerializedName("homeTelNo") val homeTelNo: String? = null,
    @SerializedName("marriedOrUnmarried") val marriedOrUnmarried: String? = null,
    @SerializedName("marriageCommunity") val marriageCommunity: String? = null,
    @SerializedName("previouslyDivorced") val previouslyDivorced: String? = null,
    @SerializedName("divorceYear") val divorceYear: String? = null,
    @SerializedName("divorceCommunity") val divorceCommunity: String? = null,
    @SerializedName("currentPhysicalAddress") val currentPhysicalAddress: String? = null,
    @SerializedName("postalAddress") val postalAddress: String? = null,
    @SerializedName("parentsAddress") val parentsAddress: String? = null,
    @SerializedName("residenceYears") val residenceYears: Int? = null,
    @SerializedName("residenceMonths") val residenceMonths: Int? = null,
    @SerializedName("companyName") val companyName: String? = null,
    @SerializedName("workTelephone") val workTelephone: String? = null,
    @SerializedName("occupation") val occupation: String? = null,
    @SerializedName("workAddress") val workAddress: String? = null,
    @SerializedName("bankName") val bankName: String? = null,
    @SerializedName("accountType") val accountType: String? = null,
    @SerializedName("accountNumber") val accountNumber: String? = null,
    @SerializedName("branchName") val branchName: String? = null,
    @SerializedName("accountName") val accountName: String? = null,
    @SerializedName("branchCode") val branchCode: String? = null,
    @SerializedName("spouseNameAndSurname") val spouseNameAndSurname: String? = null,
    @SerializedName("spouseIdNumber") val spouseIdNumber: String? = null,
    @SerializedName("spouseTelNumber") val spouseTelNumber: String? = null,
    @SerializedName("spouseEmployerName") val spouseEmployerName: String? = null,
    @SerializedName("spouseEmployerAddress") val spouseEmployerAddress: String? = null,
    @SerializedName("spouseEmployerTelNumber") val spouseEmployerTelNumber: String? = null,
    @SerializedName("relative1Name") val relative1Name: String? = null,
    @SerializedName("relative1Relationship") val relative1Relationship: String? = null,
    @SerializedName("relative1TelNumber") val relative1TelNumber: String? = null,
    @SerializedName("relative1Address") val relative1Address: String? = null,
    @SerializedName("relative2Name") val relative2Name: String? = null,
    @SerializedName("relative2Relationship") val relative2Relationship: String? = null,
    @SerializedName("relative2TelNumber") val relative2TelNumber: String? = null,
    @SerializedName("relative2Address") val relative2Address: String? = null,
    @SerializedName("reasonsForLoan") val reasonsForLoan: List<String>? = null,
    @SerializedName("otherReason") val otherReason: String? = null,
    @SerializedName("applicantSignature") val applicantSignature: String? = null,
    @SerializedName("applicantFormDate") val applicantFormDate: String? = null
)

data class UpdateStatusRequest(
    @SerializedName("status") val status: String,
    @SerializedName("note") val note: String? = null
)

// ==========================================
// Response Models
// ==========================================

data class AuthResponse(
    @SerializedName("token") val token: String? = null,
    @SerializedName("accessToken") val accessToken: String? = null,
    @SerializedName("refreshToken") val refreshToken: String? = null,
    @SerializedName("expiration") val expiration: String? = null,
    @SerializedName("user") val user: UserDto? = null
)

data class UserDto(
    @SerializedName("id") val id: String? = null,
    @SerializedName("fullName") val fullName: String? = null,
    @SerializedName("email") val email: String? = null,
    @SerializedName("role") val role: String? = null,
    @SerializedName("createdAt") val createdAt: String? = null
)

data class LoanDto(
    @SerializedName("id") val id: String? = null,
    @SerializedName("applicantName") val applicantName: String? = null,
    @SerializedName("requestedAmount") val requestedAmount: Double? = null,
    @SerializedName("amount") val amount: Double? = null,
    @SerializedName("reasonForLoan") val reasonForLoan: String? = null,
    @SerializedName("status") val status: String? = null,
    @SerializedName("submissionDate") val submissionDate: String? = null,
    @SerializedName("clientDetails") val clientDetails: ClientDetailsRequest? = null,
    @SerializedName("note") val note: String? = null
)
