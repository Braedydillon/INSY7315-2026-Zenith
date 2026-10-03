package com.example.prototype_2.API

import com.google.gson.annotations.SerializedName

// ==========================================
// Request Schemas
// ==========================================

data class ClientDetailsRequest(
    @SerializedName("FullNameAndSurname")
    val fullNameAndSurname: String? = null,

    @SerializedName("IdNumber")
    val idNumber: String? = null,

    @SerializedName("CellNo")
    val cellNo: String? = null,

    @SerializedName("HomeTelNo")
    val homeTelNo: String? = null,

    @SerializedName("MarriedOrUnmarried")
    val marriedOrUnmarried: String? = null,

    @SerializedName("MarriageCommunity")
    val marriageCommunity: String? = null,

    @SerializedName("PreviouslyDivorced")
    val previouslyDivorced: String? = null,

    @SerializedName("DivorceYear")
    val divorceYear: String? = null,

    @SerializedName("DivorceCommunity")
    val divorceCommunity: String? = null,

    @SerializedName("CurrentPhysicalAddress")
    val currentPhysicalAddress: String? = null,

    @SerializedName("PostalAddress")
    val postalAddress: String? = null,

    @SerializedName("ParentsAddress")
    val parentsAddress: String? = null,

    @SerializedName("ResidenceYears")
    val residenceYears: Int? = null,

    @SerializedName("ResidenceMonths")
    val residenceMonths: Int? = null,

    @SerializedName("CompanyName")
    val companyName: String? = null,

    @SerializedName("Occupation")
    val occupation: String? = null,

    @SerializedName("WorkTelephone")
    val workTelephone: String? = null,

    @SerializedName("WorkAddress")
    val workAddress: String? = null
)
data class SpouseDetailsRequest(
    @SerializedName("Name")
    val name: String? = null,

    @SerializedName("IdNumber")
    val idNumber: String? = null,

    @SerializedName("Telephone")
    val telephone: String? = null,

    @SerializedName("EmployerName")
    val employerName: String? = null,

    @SerializedName("EmployerAddress")
    val employerAddress: String? = null,

    @SerializedName("EmployerTelephone")
    val employerTelephone: String? = null
)

data class RelativeDetailsRequest(
    @SerializedName("Name")
    val name: String? = null,

    @SerializedName("Relationship")
    val relationship: String? = null,

    @SerializedName("Telephone")
    val telephone: String? = null,

    @SerializedName("Address")
    val address: String? = null
)

data class SubmitLoanRequest(
    @SerializedName("RequestedAmount")
    val requestedAmount: Double? = null,

    @SerializedName("ReasonForLoan")
    val reasonForLoan: String? = null,

    @SerializedName("OtherReason")
    val otherReason: String? = null,

    @SerializedName("ClientDetails")
    val clientDetails: ClientDetailsRequest? = null,

    // Bank Details
    @SerializedName("BankName")
    val bankName: String? = null,

    @SerializedName("AccountType")
    val accountType: String? = null,

    @SerializedName("AccountNumber")
    val accountNumber: String? = null,

    @SerializedName("BranchName")
    val branchName: String? = null,

    @SerializedName("AccountName")
    val accountName: String? = null,

    @SerializedName("BranchCode")
    val branchCode: String? = null,

    // Spouse Details
    @SerializedName("SpouseNameAndSurname")
    val spouseNameAndSurname: String? = null,

    @SerializedName("SpouseIdNumber")
    val spouseIdNumber: String? = null,

    @SerializedName("SpouseTelNumber")
    val spouseTelNumber: String? = null,

    @SerializedName("SpouseEmployerName")
    val spouseEmployerName: String? = null,

    @SerializedName("SpouseEmployerAddress")
    val spouseEmployerAddress: String? = null,

    @SerializedName("SpouseEmployerTelNumber")
    val spouseEmployerTelNumber: String? = null,

    // Relative 1
    @SerializedName("Relative1Name")
    val relative1Name: String? = null,

    @SerializedName("Relative1Relationship")
    val relative1Relationship: String? = null,

    @SerializedName("Relative1TelNumber")
    val relative1TelNumber: String? = null,

    @SerializedName("Relative1Address")
    val relative1Address: String? = null,

    // Relative 2
    @SerializedName("Relative2Name")
    val relative2Name: String? = null,

    @SerializedName("Relative2Relationship")
    val relative2Relationship: String? = null,

    @SerializedName("Relative2TelNumber")
    val relative2TelNumber: String? = null,

    @SerializedName("Relative2Address")
    val relative2Address: String? = null,

    // Application Details
    @SerializedName("ApplicantSignature")
    val applicantSignature: String? = null,

    @SerializedName("ApplicationDate")
    val applicationDate: String? = null,

    @SerializedName("ApplicantFormDate")
    val applicantFormDate: String? = null
)

data class LoginRequest(
    @SerializedName("email")
    val email: String? = null,

    @SerializedName("password")
    val password: String? = null
)

data class RefreshRequest(
    @SerializedName("accessToken")
    val accessToken: String? = null,

    @SerializedName("refreshToken")
    val refreshToken: String? = null
)

data class RegisterRequest(
    @SerializedName("fullName")
    val fullName: String? = null,

    @SerializedName("email")
    val email: String? = null,

    @SerializedName("password")
    val password: String? = null,

    @SerializedName("role")
    val role: String? = null,

    @SerializedName("idNumber")
    val idNumber: String? = null,

    @SerializedName("cellNo")
    val cellNo: String? = null
)

data class RoleAssignmentRequest(
    @SerializedName("userId")
    val userId: String? = null,

    @SerializedName("email")
    val email: String? = null,

    @SerializedName("role")
    val role: String? = null
)

data class UpdateStatusRequest(
    @SerializedName("status")
    val status: String? = null,

    @SerializedName("comments")
    val comments: String? = null
)

// ==========================================
// Response Models
// ==========================================

data class AuthResponse(

    @SerializedName("token") val token: String? = null,
    @SerializedName("idToken") val idToken: String? = null,
    @SerializedName("accessToken") val accessToken: String? = null,
    @SerializedName("refreshToken") val refreshToken: String? = null,
    @SerializedName("expiration") val expiration: String? = null,
    @SerializedName("role") val role: String? = null,
    @SerializedName("user") val user: UserDto? = null



)

data class UserDto(
    @SerializedName("id")
    val id: String? = null,

    @SerializedName("fullName")
    val fullName: String? = null,

    @SerializedName("email")
    val email: String? = null,

    @SerializedName("role")
    val role: String? = null,

    @SerializedName("createdAt")
    val createdAt: String? = null
)

data class LoanDto(
    @SerializedName("id")
    val id: String? = null,
    @SerializedName("applicationId")
    val applicationId: String? = null,

    @SerializedName("applicantName")
    val applicantName: String? = null,


    @SerializedName("requestedAmount")
    val requestedAmount: Double? = null,

    @SerializedName("amount")
    val amount: Double? = null,

    @SerializedName("reasonForLoan")
    val reasonForLoan: String? = null,

    @SerializedName("status")
    val status: String? = null,

    @SerializedName("submissionDate")
    val submissionDate: String? = null,

    @SerializedName("clientDetails")
    val clientDetails: ClientDetailsRequest? = null,

    @SerializedName("note")
    val note: String? = null
)