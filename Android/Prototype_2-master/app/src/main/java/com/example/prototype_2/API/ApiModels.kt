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

// ==========================================
// Authentication Requests
// ==========================================

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
    @SerializedName("token")
    val token: String? = null,

    @SerializedName("idToken")
    val idToken: String? = null,

    @SerializedName("accessToken")
    val accessToken: String? = null,

    @SerializedName("refreshToken")
    val refreshToken: String? = null,

    @SerializedName("expiration")
    val expiration: String? = null,

    @SerializedName("role")
    val role: String? = null,

    @SerializedName("user")
    val user: UserDto? = null
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

// ==========================================
// Client Details Response Model
// ==========================================

data class ClientDetailsDto(

    @SerializedName("fullNameAndSurname")
    val fullNameAndSurname: String? = null,

    @SerializedName("idNumber")
    val idNumber: String? = null,

    @SerializedName("cellNo")
    val cellNo: String? = null,

    @SerializedName("homeTelNo")
    val homeTelNo: String? = null,

    @SerializedName("marriedOrUnmarried")
    val marriedOrUnmarried: String? = null,

    @SerializedName("marriageCommunity")
    val marriageCommunity: String? = null,

    @SerializedName("previouslyDivorced")
    val previouslyDivorced: String? = null,

    @SerializedName("divorceYear")
    val divorceYear: String? = null,

    @SerializedName("divorceCommunity")
    val divorceCommunity: String? = null,

    @SerializedName("currentPhysicalAddress")
    val currentPhysicalAddress: String? = null,

    @SerializedName("postalAddress")
    val postalAddress: String? = null,

    @SerializedName("parentsAddress")
    val parentsAddress: String? = null,

    @SerializedName("residenceYears")
    val residenceYears: Int? = null,

    @SerializedName("residenceMonths")
    val residenceMonths: Int? = null,

    @SerializedName("companyName")
    val companyName: String? = null,

    @SerializedName("occupation")
    val occupation: String? = null,

    @SerializedName("workTelephone")
    val workTelephone: String? = null,

    @SerializedName("workAddress")
    val workAddress: String? = null
)

// ==========================================
// Loan Response Model
// ==========================================

data class LoanDto(

    @SerializedName("applicationId")
    val applicationId: String? = null,

    @SerializedName("userId")
    val userId: String? = null,

    @SerializedName("userEmail")
    val userEmail: String? = null,

    @SerializedName("status")
    var status: String? = null,

    @SerializedName("applicationDate")
    val applicationDate: String? = null,

    @SerializedName("requestedAmount")
    val requestedAmount: Double? = null,

    @SerializedName("reasonForLoan")
    val reasonForLoan: String? = null,

    @SerializedName("clientDetails")
    val clientDetails: ClientDetailsDto? = null,

    @SerializedName("homeTelNo")
    val homeTelNo: String? = null,

    @SerializedName("marriedOrUnmarried")
    val marriedOrUnmarried: String? = null,

    @SerializedName("marriageCommunity")
    val marriageCommunity: String? = null,

    @SerializedName("previouslyDivorced")
    val previouslyDivorced: String? = null,

    @SerializedName("divorceYear")
    val divorceYear: String? = null,

    @SerializedName("divorceCommunity")
    val divorceCommunity: String? = null,

    @SerializedName("currentPhysicalAddress")
    val currentPhysicalAddress: String? = null,

    @SerializedName("postalAddress")
    val postalAddress: String? = null,

    @SerializedName("parentsAddress")
    val parentsAddress: String? = null,

    @SerializedName("residenceYears")
    val residenceYears: Int? = null,

    @SerializedName("residenceMonths")
    val residenceMonths: Int? = null,

    @SerializedName("companyName")
    val companyName: String? = null,

    @SerializedName("workTelephone")
    val workTelephone: String? = null,

    @SerializedName("occupation")
    val occupation: String? = null,

    @SerializedName("workAddress")
    val workAddress: String? = null,

    @SerializedName("bankName")
    val bankName: String? = null,

    @SerializedName("accountType")
    val accountType: String? = null,

    @SerializedName("accountNumber")
    val accountNumber: String? = null,

    @SerializedName("branchName")
    val branchName: String? = null,

    @SerializedName("accountName")
    val accountName: String? = null,

    @SerializedName("branchCode")
    val branchCode: String? = null,

    @SerializedName("spouseNameAndSurname")
    val spouseNameAndSurname: String? = null,

    @SerializedName("spouseIdNumber")
    val spouseIdNumber: String? = null,

    @SerializedName("spouseTelNumber")
    val spouseTelNumber: String? = null,

    @SerializedName("spouseEmployerName")
    val spouseEmployerName: String? = null,

    @SerializedName("spouseEmployerAddress")
    val spouseEmployerAddress: String? = null,

    @SerializedName("spouseEmployerTelNumber")
    val spouseEmployerTelNumber: String? = null,

    @SerializedName("relative1Name")
    val relative1Name: String? = null,

    @SerializedName("relative1Relationship")
    val relative1Relationship: String? = null,

    @SerializedName("relative1TelNumber")
    val relative1TelNumber: String? = null,

    @SerializedName("relative1Address")
    val relative1Address: String? = null,

    @SerializedName("relative2Name")
    val relative2Name: String? = null,

    @SerializedName("relative2Relationship")
    val relative2Relationship: String? = null,

    @SerializedName("relative2TelNumber")
    val relative2TelNumber: String? = null,

    @SerializedName("relative2Address")
    val relative2Address: String? = null,

    @SerializedName("reasonsForLoan")
    val reasonsForLoan: List<String>? = null,

    @SerializedName("otherReason")
    val otherReason: String? = null,

    @SerializedName("applicantSignature")
    val applicantSignature: String? = null,

    @SerializedName("applicantFormDate")
    val applicantFormDate: String? = null,

    @SerializedName("reviewedBy")
    val reviewedBy: String? = null,

    @SerializedName("reviewNote")
    val reviewNote: String? = null
)