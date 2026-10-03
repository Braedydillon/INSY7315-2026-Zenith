package com.example.prototype_2.API

import android.content.Context
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object RetrofitClient {
    private const val BASE_URL = "https://apiinsy7315-latest.onrender.com/"

    var authToken: String? = null

    private val loggingInterceptor = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }

    private val okHttpClient: OkHttpClient by lazy {
        OkHttpClient.Builder()
            .addInterceptor(loggingInterceptor)
            .addInterceptor { chain ->
                val original = chain.request()
                val requestBuilder = original.newBuilder()

                val token = authToken
                if (!token.isNullOrEmpty() && original.header("Authorization") == null) {
                    val bearerHeader = if (token.startsWith("Bearer ", ignoreCase = true)) {
                        token
                    } else {
                        "Bearer $token"
                    }
                    requestBuilder.header("Authorization", bearerHeader)
                }

                chain.proceed(requestBuilder.build())
            }
            .build()
    }

    val apiService: ApiService by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ApiService::class.java)
    }

    fun loadToken(context: Context) {

        val prefs =
            context.getSharedPreferences(
                "AppPrefs",
                Context.MODE_PRIVATE
            )

        authToken =
            prefs.getString(
                "AUTH_TOKEN",
                null
            )
    }
}
