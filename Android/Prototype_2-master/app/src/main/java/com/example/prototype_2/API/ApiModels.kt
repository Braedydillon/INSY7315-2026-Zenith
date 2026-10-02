package com.example.prototype_2.API

import com.google.gson.annotations.SerializedName

// ==========================================
// Request Schemas
// ==========================================

data class ClientDetailsRequest(
    @SerializedName("firstName") val firstName: String? = null,
    @SerializedName("lastName") val lastName: String? = null,
    @SerializedName("email") val email: String? = null,
    @SerializedName("idNumber") val idNumber: String? = null,
    @SerializedName("homeTelephone") val homeTelephone: String? = null,
    @SerializedName("cellNumber") val cellNumber: String? = null,
    @SerializedName("maritalStatus") val maritalStatus: String? = null,
    @SerializedName("maritalCommunity") val maritalCommunity: String? = null,
    @SerializedName("previouslyDivorced") val previouslyDivorced: Boolean? = null,
    @SerializedName("divorceYear") val divorceYear: Int? = null,
    @SerializedName("divorceCommunity") val divorceCommunity: String? = null,
    @SerializedName("physicalAddress") val physicalAddress: String? = null,
    @SerializedName("postalAddress") val postalAddress: String? = null,
    @SerializedName("parentsAddress") val parentsAddress: String? = null,
    @SerializedName("residenceYears") val residenceYears: Int? = null,
    @SerializedName("residenceMonths") val residenceMonths: Int? = null,
    @SerializedName("companyName") val companyName: String? = null,
    @SerializedName("workTelephone") val workTelephone: String? = null,
    @SerializedName("occupation") val occupation: String? = null,
    @SerializedName("workAddress") val workAddress: String? = null,
    @SerializedName("employmentStatus") val employmentStatus: String? = null,
    @SerializedName("monthlyIncome") val monthlyIncome: Double? = null,
    @SerializedName("bankName") val bankName: String? = null,
    @SerializedName("accountType") val accountType: String? = null,
    @SerializedName("accountNumber") val accountNumber: String? = null,
    @SerializedName("branchName") val branchName: String? = null,
    @SerializedName("accountName") val accountName: String? = null,
    @SerializedName("branchCode") val branchCode: String? = null
)

data class SpouseDetailsRequest(
    @SerializedName("name") val name: String? = null,
    @SerializedName("idNumber") val idNumber: String? = null,
    @SerializedName("telephone") val telephone: String? = null,
    @SerializedName("employerName") val employerName: String? = null,
    @SerializedName("employerAddress") val employerAddress: String? = null,
    @SerializedName("employerTelephone") val employerTelephone: String? = null
)

data class RelativeDetailsRequest(
    @SerializedName("name") val name: String? = null,
    @SerializedName("relationship") val relationship: String? = null,
    @SerializedName("telephone") val telephone: String? = null,
    @SerializedName("address") val address: String? = null
)

data class SubmitLoanRequest(
    @SerializedName("amount") val amount: Double? = null,
    @SerializedName("termMonths") val termMonths: Int? = null,
    @SerializedName("purpose") val purpose: String? = null,
    @SerializedName("otherPurpose") val otherPurpose: String? = null,
    @SerializedName("clientDetails") val clientDetails: ClientDetailsRequest? = null,
    @SerializedName("spouseDetails") val spouseDetails: SpouseDetailsRequest? = null,
    @SerializedName("relative1") val relative1: RelativeDetailsRequest? = null,
    @SerializedName("relative2") val relative2: RelativeDetailsRequest? = null,
    @SerializedName("applicantSignature") val applicantSignature: String? = null,
    @SerializedName("applicationDate") val applicationDate: String? = null
)

data class LoginRequest(
    @SerializedName("email") val email: String? = null,
    @SerializedName("password") val password: String? = null
)

data class RefreshRequest(
    @SerializedName("accessToken") val accessToken: String? = null,
    @SerializedName("refreshToken") val refreshToken: String? = null
)

data class RegisterRequest(
    @SerializedName("fullName") val fullName: String? = null,
    @SerializedName("email") val email: String? = null,
    @SerializedName("password") val password: String? = null,
    @SerializedName("role") val role: String? = null
)

data class RoleAssignmentRequest(
    @SerializedName("userId") val userId: String? = null,
    @SerializedName("email") val email: String? = null,
    @SerializedName("role") val role: String? = null
)

data class UpdateStatusRequest(
    @SerializedName("status") val status: String? = null,
    @SerializedName("comments") val comments: String? = null
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
