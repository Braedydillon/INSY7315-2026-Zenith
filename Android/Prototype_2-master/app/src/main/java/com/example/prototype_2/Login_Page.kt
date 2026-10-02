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

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
    }

    private fun performLogin() {
        val email = edtEmail.text.toString().trim()
        val password = edtPassword.text.toString().trim()

        if (email.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, "Please fill in all fields", Toast.LENGTH_SHORT).show()
            return
        }

        btnLogin.isEnabled = false

        lifecycleScope.launch {
            try {
                val response = withContext(Dispatchers.IO) {
                    RetrofitClient.apiService.login(LoginRequest(email, password))
                }

                if (response.isSuccessful && response.body() != null) {
                    val authResponse = response.body()
                    val token = authResponse?.token ?: authResponse?.accessToken

                    RetrofitClient.authToken = token

                    val prefs = getSharedPreferences("AppPrefs", MODE_PRIVATE)
                    prefs.edit().putString("AUTH_TOKEN", token).apply()

                    Toast.makeText(this@Login_Page, "Login successful!", Toast.LENGTH_SHORT).show()

                    val intent = Intent(this@Login_Page, MainActivity::class.java)
                    startActivity(intent)
                    finish()
                } else {
                    val code = response.code()
                    val errorMsg = withContext(Dispatchers.IO) {
                        response.errorBody()?.string()
                    }
                    Log.e("Login_API", "Code: $code, ErrorBody: $errorMsg")
                    
                    val displayMsg = when {
                        !errorMsg.isNullOrEmpty() -> errorMsg
                        else -> "HTTP $code ${response.message()}"
                    }
                    Toast.makeText(this@Login_Page, "Login failed: $displayMsg", Toast.LENGTH_LONG).show()
                }
            } catch (e: Exception) {
                Log.e("Login_API", "Exception: ", e)
                Toast.makeText(this@Login_Page, "Network error: ${e.localizedMessage}", Toast.LENGTH_LONG).show()
            } finally {
                btnLogin.isEnabled = true
            }
        }
    }
}
