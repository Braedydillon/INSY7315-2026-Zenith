package com.example.prototype_2.API

import okhttp3.ResponseBody
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.Header
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

interface ApiService {

    // --- Admin ---

    @POST("api/Admin/set-role")
    suspend fun setRole(
        @Header("Authorization") token: String? = null,
        @Body request: RoleAssignmentRequest
    ): Response<ResponseBody>

    @GET("api/Admin/users")
    suspend fun getUsers(
        @Header("Authorization") token: String? = null
    ): Response<List<UserDto>>

    // --- Health ---

    @GET("health")
    suspend fun getHealth(): Response<ResponseBody>

    @GET("health/firestore")
    suspend fun getHealthFirestore(): Response<ResponseBody>

    // --- Auth ---

    @POST("api/Auth/register")
    suspend fun register(
        @Body request: RegisterRequest
    ): Response<AuthResponse>

    @POST("api/Auth/login")
    suspend fun login(
        @Body request: LoginRequest
    ): Response<AuthResponse>

    @POST("api/Auth/refresh")
    suspend fun refresh(
        @Body request: RefreshRequest
    ): Response<AuthResponse>

    // --- LoansApi ---

    @POST("api/LoansApi")
    suspend fun submitLoan(
        @Header("Authorization") token: String? = null,
        @Body request: SubmitLoanRequest
    ): Response<LoanDto>

    @GET("api/LoansApi")
    suspend fun getAllLoans(
        @Header("Authorization") token: String? = null
    ): Response<List<LoanDto>>

    @GET("api/LoansApi/mine")
    suspend fun getMyLoans(
        @Header("Authorization") token: String? = null
    ): Response<List<LoanDto>>

    @GET("api/LoansApi/pending")
    suspend fun getPendingLoans(
        @Header("Authorization") token: String? = null
    ): Response<List<LoanDto>>

    @GET("api/LoansApi/{id}")
    suspend fun getLoanById(
        @Header("Authorization") token: String? = null,
        @Path("id") id: String
    ): Response<LoanDto>

    @PUT("api/LoansApi/{id}/status")
    suspend fun updateLoanStatus(
        @Header("Authorization") token: String? = null,
        @Path("id") id: String,
        @Body request: UpdateStatusRequest
    ): Response<ResponseBody>

    // --- Me ---

    @GET("api/Me")
    suspend fun getMe(
        @Header("Authorization") token: String? = null
    ): Response<UserDto>
}
