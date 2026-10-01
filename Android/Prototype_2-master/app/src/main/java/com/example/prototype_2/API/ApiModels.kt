package com.example.prototype_2.API

import com.google.gson.annotations.SerializedName

// ==========================================
// Request Schemas
// ==========================================

data class ClientDetailsRequest(
    @SerializedName("firstName") val firstName: String? = null,
    @SerializedName("lastName") val lastName: String? = null,
    @SerializedName("email") val email: String? = null,
    @SerializedName("phoneNumber") val phoneNumber: String? = null,
    @SerializedName("idNumber") val idNumber: String? = null,
    @SerializedName("address") val address: String? = null,
    @SerializedName("employmentStatus") val employmentStatus: String? = null,
    @SerializedName("monthlyIncome") val monthlyIncome: Double? = null
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

data class SubmitLoanRequest(
    @SerializedName("amount") val amount: Double? = null,
    @SerializedName("termMonths") val termMonths: Int? = null,
    @SerializedName("purpose") val purpose: String? = null,
    @SerializedName("clientDetails") val clientDetails: ClientDetailsRequest? = null
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
    @SerializedName("amount") val amount: Double? = null,
    @SerializedName("termMonths") val termMonths: Int? = null,
    @SerializedName("purpose") val purpose: String? = null,
    @SerializedName("status") val status: String? = null,
    @SerializedName("submissionDate") val submissionDate: String? = null,
    @SerializedName("clientDetails") val clientDetails: ClientDetailsRequest? = null,
    @SerializedName("comments") val comments: String? = null
)
