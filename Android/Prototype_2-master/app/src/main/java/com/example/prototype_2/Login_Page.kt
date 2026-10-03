
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

                val response = withContext(Dispatchers.IO) {
                    RetrofitClient.apiService.login(
                        LoginRequest(
                            email = email,
                            password = password
                        )
                    )
                }

                if (!response.isSuccessful || response.body() == null) {

                    val error = withContext(Dispatchers.IO) {
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

                val authResponse = response.body()!!

                Log.d(
                    "LOGIN_API",
                    "Login response: $authResponse"
                )

                // Get the authentication token.
                // The backend may return it as idToken, token or accessToken.
                val token = authResponse.idToken
                    ?: authResponse.token
                    ?: authResponse.accessToken

                if (token.isNullOrEmpty()) {

                    Toast.makeText(
                        this@Login_Page,
                        "Login successful but no authentication token was returned.",
                        Toast.LENGTH_LONG
                    ).show()

                    return@launch
                }

                // Store token for API requests
                RetrofitClient.authToken = token

                // Get the user's role
                val role = (
                        authResponse.role
                            ?: authResponse.user?.role
                            ?: ""
                        ).lowercase()

                Log.d(
                    "LOGIN_ROLE",
                    "User role: $role"
                )

                // Save login information
                val prefs = getSharedPreferences(
                    "AppPrefs",
                    MODE_PRIVATE
                )

                prefs.edit()
                    .putString("AUTH_TOKEN", token)
                    .putString(
                        "REFRESH_TOKEN",
                        authResponse.refreshToken ?: ""
                    )
                    .putString(
                        "UID",
                        authResponse.user?.id ?: ""
                    )
                    .putString(
                        "EMAIL",
                        authResponse.user?.email ?: email
                    )
                    .putString(
                        "ROLE",
                        role
                    )
                    .apply()

                Toast.makeText(
                    this@Login_Page,
                    "Login successful!",
                    Toast.LENGTH_SHORT
                ).show()

                // Staff and admin users go to the staff dashboard.
                // Clients go to the normal client home page.
                if (role == "staff" || role == "admin") {

                    val intent = Intent(
                        this@Login_Page,
                        Staff_Page::class.java
                    )

                    startActivity(intent)

                } else {

                    val intent = Intent(
                        this@Login_Page,
                        MainActivity::class.java
                    )

                    startActivity(intent)
                }

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

