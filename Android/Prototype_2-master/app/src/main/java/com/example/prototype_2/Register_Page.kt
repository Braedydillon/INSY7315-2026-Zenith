package com.example.prototype_2

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.CheckBox
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.lifecycle.lifecycleScope
import com.example.prototype_2.API.RegisterRequest
import com.example.prototype_2.API.RetrofitClient
import com.google.android.material.textfield.TextInputEditText
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class Register_Page : AppCompatActivity() {

    private lateinit var btnSubmitRegistration : Button
    private lateinit var txtLoginBack : TextView
    private lateinit var checkTerms : CheckBox
    private lateinit var edtFullName : TextInputEditText
    private lateinit var edtEmail : TextInputEditText
    private lateinit var edtPhone : TextInputEditText
    private lateinit var edtIdNumber : TextInputEditText
    private lateinit var edtPassword : TextInputEditText
    private lateinit var edtConfirmPassword : TextInputEditText

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_register_page)

        btnSubmitRegistration = findViewById(R.id.btnSubmitRegistration)
        txtLoginBack = findViewById(R.id.txtLoginBack)
        checkTerms = findViewById(R.id.checkTerms)
        edtFullName = findViewById(R.id.edtFullName)
        edtEmail = findViewById(R.id.edtEmail)
        edtPhone = findViewById(R.id.edtPhone)
        edtIdNumber = findViewById(R.id.edtIdNumber)
        edtPassword = findViewById(R.id.edtPassword)
        edtConfirmPassword = findViewById(R.id.edtConfirmPassword)


        btnSubmitRegistration.setOnClickListener {
            if (checkTerms.isChecked) {
                val fullName = edtFullName.text?.toString()?.trim().orEmpty()
                val email = edtEmail.text?.toString()?.trim().orEmpty()
                val password = edtPassword.text?.toString()?.trim().orEmpty()
                val confirmPassword = edtConfirmPassword.text?.toString()?.trim().orEmpty()
                val idnumber = edtIdNumber.text?.toString()?.trim().orEmpty()
                val phone = edtPhone.text?.toString()?.trim().orEmpty()

                if (fullName.isEmpty() || email.isEmpty() || password.isEmpty()) {
                    Toast.makeText(this, "Please fill in all required fields", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }

                if (password != confirmPassword) {
                    Toast.makeText(this, "Passwords do not match", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }

                if (!isPasswordValid(password)) {
                    Toast.makeText(
                        this,
                        "Password must be at least 6 characters and include an uppercase letter, lowercase letter, digit, and special character (e.g. Password123!)",
                        Toast.LENGTH_LONG
                    ).show()
                    return@setOnClickListener
                }
                if (idnumber.isEmpty()) {
                    Toast.makeText(this, "Please enter your ID number", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }
                if (idnumber.length != 13) {
                    Toast.makeText(this, "Please enter a valid ID number", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }
                if (phone.isEmpty()) {
                    Toast.makeText(this, "Please enter your phone number", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }
                if (phone.length != 10) {
                    Toast.makeText(this, "Please enter a valid phone number", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }
                if  (!email.contains("@")) {
                    Toast.makeText(this, "Please enter a valid email address", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }





                lifecycleScope.launch {
                    try {
                        val request = RegisterRequest(
                            fullName = fullName,
                            email = email,
                            password = password,
                            idNumber = edtIdNumber.text?.toString()?.trim().orEmpty(),
                            cellNo = edtPhone.text?.toString()?.trim().orEmpty(),

                        )

                        val response = withContext(Dispatchers.IO) {
                            RetrofitClient.apiService.register(request)
                        }

                        if (response.isSuccessful) {
                            val authResponse = response.body()
                            val token = authResponse?.idToken ?: authResponse?.accessToken

                            if (token != null) {
                                RetrofitClient.authToken = token
                                val prefs = getSharedPreferences("AppPrefs", MODE_PRIVATE)
                                prefs.edit().putString("AUTH_TOKEN", token).apply()
                            }

                            Toast.makeText(this@Register_Page, "Registration successful!", Toast.LENGTH_SHORT).show()
                            startActivity(Intent(this@Register_Page, MainActivity::class.java))
                            finish()
                        } else {
                            val code = response.code()
                            val errorBodyStr = withContext(Dispatchers.IO) {
                                response.errorBody()?.string()
                            }
                            Log.e("Register_API", "Code: $code, ErrorBody: $errorBodyStr")

                            val displayMsg = when {
                                !errorBodyStr.isNullOrEmpty() -> errorBodyStr
                                else -> "HTTP $code ${response.message()}"
                            }

                            Toast.makeText(this@Register_Page, "Failed ($code): $displayMsg", Toast.LENGTH_LONG).show()
                        }
                    } catch (e: Exception) {
                        Log.e("Register_API", "Exception: ", e)
                        Toast.makeText(this@Register_Page, "Error: ${e.localizedMessage}", Toast.LENGTH_LONG).show()
                    } finally {
                        btnSubmitRegistration.isEnabled = true
                    }
                }
            } else {
                checkTerms.error = "Please accept the terms and conditions"
            }
        }

        txtLoginBack.setOnClickListener {
            val intent = Intent(this, Login_Page::class.java)
            startActivity(intent)
            finish()
        }

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
    }

    private fun isPasswordValid(password: String): Boolean {
        if (password.length < 6) return false
        val hasUpper = password.any { it.isUpperCase() }
        val hasLower = password.any { it.isLowerCase() }
        val hasDigit = password.any { it.isDigit() }
        val hasSpecial = password.any { !it.isLetterOrDigit() }
        return hasUpper && hasLower && hasDigit && hasSpecial
    }
}
