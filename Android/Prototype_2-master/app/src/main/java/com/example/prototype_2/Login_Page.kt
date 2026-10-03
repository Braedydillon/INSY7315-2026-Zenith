package com.example.prototype_2

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.lifecycle.lifecycleScope
import com.example.prototype_2.API.LoginRequest
import com.example.prototype_2.API.RetrofitClient
import com.google.android.material.floatingactionbutton.FloatingActionButton
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class Login_Page : AppCompatActivity() {

    private lateinit var edtEmail: EditText
    private lateinit var edtPassword: EditText
    private lateinit var btnLogin: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_login_page)

        edtEmail = findViewById(R.id.edtLoginEmail)
        edtPassword = findViewById(R.id.edtLoginPassword)
        btnLogin = findViewById(R.id.btnLogin)

        btnLogin.setOnClickListener {
            performLogin()
        }

        findViewById<TextView>(R.id.txtCreateAccount).setOnClickListener {
            val intent = Intent(this, Register_Page::class.java)
            startActivity(intent)
        }

        findViewById<Button>(R.id.btnStaffProto).setOnClickListener {
            val intent = Intent(this, Staff_Page::class.java)
            startActivity(intent)
        }

        val btnHome = findViewById<FloatingActionButton>(R.id.btnHome)

        btnHome.setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            startActivity(intent)
        }

        ViewCompat.setOnApplyWindowInsetsListener(
            findViewById(R.id.main)
        ) { v, insets ->

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
    }

    private fun performLogin() {

        val email = edtEmail.text.toString().trim()
        val password = edtPassword.text.toString().trim()

        if (email.isEmpty() || password.isEmpty()) {

            Toast.makeText(
                this,
                "Please fill in all fields",
                Toast.LENGTH_SHORT
            ).show()

            return
        }

        btnLogin.isEnabled = false

        lifecycleScope.launch {

            try {

                // Login
                val response = withContext(Dispatchers.IO) {
                    RetrofitClient.apiService.login(
                        LoginRequest(
                            email = email,
                            password = password
                        )
                    )
                }

<<<<<<< HEAD
                if (response.isSuccessful && response.body() != null) {
                    val authResponse = response.body()
                    
                    // Note: Your backend returns 'idToken' in JSON instead of 'token' in the Login endpoint
                    // so we make sure we grab the token properly. If the backend actually maps to 'idToken',
                    // we'll rely on the manual API models.
                    
                    val role = authResponse?.role?.lowercase() ?: authResponse?.user?.role?.lowercase()
                    
                    if(role == "admin") {
                        val intent = Intent(this@Login_Page, Staff_Page::class.java)
                        startActivity(intent)
                        finish()
                        return@launch
                    } else {
                        // Treat everything else as a client for now
                        val token = authResponse?.token ?: authResponse?.accessToken ?: authResponse?.idToken

                        if (token != null) {
                            RetrofitClient.authToken = token
                            val prefs = getSharedPreferences("AppPrefs", MODE_PRIVATE)
                            prefs.edit().putString("AUTH_TOKEN", token).apply()
                        }

                        Toast.makeText(this@Login_Page, "Login successful!", Toast.LENGTH_SHORT).show()

                        val intent = Intent(this@Login_Page, MainActivity::class.java)
                        startActivity(intent)
                        finish()
                    }
                } else {
                    val code = response.code()
                    val errorMsg = withContext(Dispatchers.IO) {
=======
                if (!response.isSuccessful || response.body() == null) {

                    val error = withContext(Dispatchers.IO) {
>>>>>>> fea7102 (Fixed the application page)
                        response.errorBody()?.string()
                    }

                    Log.e(
                        "LOGIN_API",
                        "Login failed: ${response.code()} - $error"
                    )

                    Toast.makeText(
                        this@Login_Page,
                        "Login failed: ${response.code()}",
                        Toast.LENGTH_LONG
                    ).show()

                    return@launch
                }

                val authResponse = response.body()

                Log.d(
                    "LOGIN_API",
                    "Login response: $authResponse"
                )

                // Get Firebase ID token
                val idToken = authResponse?.idToken

                if (idToken.isNullOrEmpty()) {

                    Toast.makeText(
                        this@Login_Page,
                        "Login successful but no ID token was returned.",
                        Toast.LENGTH_LONG
                    ).show()

                    return@launch
                }

                // Store token in RetrofitClient
                RetrofitClient.authToken = idToken

                // Save login information
                val prefs = getSharedPreferences(
                    "AppPrefs",
                    MODE_PRIVATE
                )

                prefs.edit()
                    .putString("AUTH_TOKEN", idToken)
                    .putString(
                        "REFRESH_TOKEN",
                        authResponse.refreshToken
                    )
                    .putString(
                        "UID",
                        authResponse.uid
                    )
                    .putString(
                        "EMAIL",
                        authResponse.email
                    )
                    .putString(
                        "ROLE",
                        authResponse.role
                    )
                    .apply()

                Log.d(
                    "LOGIN_AUTH",
                    "ID token saved: ${idToken.take(20)}..."
                )

                Toast.makeText(
                    this@Login_Page,
                    "Login successful!",
                    Toast.LENGTH_SHORT
                ).show()

                // Open MainActivity
                val intent = Intent(
                    this@Login_Page,
                    MainActivity::class.java
                )

                startActivity(intent)
                finish()

            } catch (e: Exception) {

                Log.e(
                    "LOGIN_API",
                    "Exception:",
                    e
                )

                Toast.makeText(
                    this@Login_Page,
                    "Network error: ${e.localizedMessage}",
                    Toast.LENGTH_LONG
                ).show()

            } finally {

                btnLogin.isEnabled = true
            }
        }
    }
}