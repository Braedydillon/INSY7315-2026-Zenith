package com.example.prototype_2

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.prototype_2.API.LoanDto
import com.example.prototype_2.API.RetrofitClient
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class Manager_Page : AppCompatActivity() {

    private lateinit var totalApplicationsText: TextView
    private lateinit var pendingApplicationsText: TextView
    private lateinit var approvedApplicationsText: TextView
    private lateinit var rejectedApplicationsText: TextView

    private lateinit var rvManagerLoans: RecyclerView
    private lateinit var btnLogoutManager: Button

    private lateinit var adapter: ManagerLoanAdapter

    private var applications = mutableListOf<LoanDto>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_manager_page)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        totalApplicationsText = findViewById(R.id.TotalApplications)
        pendingApplicationsText = findViewById(R.id.PendingApplications)
        approvedApplicationsText = findViewById(R.id.ApprovedApplications)
        rejectedApplicationsText = findViewById(R.id.RejectedApplications)

        rvManagerLoans = findViewById(R.id.rvManagerLoans)
        btnLogoutManager = findViewById(R.id.btnLogoutManager)

        adapter = ManagerLoanAdapter(applications)
        rvManagerLoans.layoutManager = LinearLayoutManager(this)
        rvManagerLoans.adapter = adapter

        btnLogoutManager.setOnClickListener {
            logout()
        }

        loadApplications()
    }

    override fun onResume() {
        super.onResume()
        loadApplications()
    }

    private fun getAuthToken(): String? {
        var token = RetrofitClient.authToken

        if (token.isNullOrEmpty()) {
            val prefsApp = getSharedPreferences("AppPrefs", MODE_PRIVATE)
            token = prefsApp.getString("AUTH_TOKEN", null)
        }

        if (token.isNullOrEmpty()) {
            val prefsAuth = getSharedPreferences("Auth", MODE_PRIVATE)
            token = prefsAuth.getString("AUTH_TOKEN", null) ?: prefsAuth.getString("token", null)
        }

        if (!token.isNullOrEmpty()) {
            RetrofitClient.authToken = token
        }

        return token
    }

    private fun loadApplications() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val rawToken = getAuthToken()

                if (rawToken.isNullOrEmpty()) {
                    withContext(Dispatchers.Main) {
                        Toast.makeText(
                            this@Manager_Page,
                            "Please log in again",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                    return@launch
                }

                val bearerToken = if (rawToken.startsWith("Bearer ", ignoreCase = true)) {
                    rawToken
                } else {
                    "Bearer $rawToken"
                }

                val response = RetrofitClient.apiService.getAllLoans(token = bearerToken)

                withContext(Dispatchers.Main) {
                    if (response.isSuccessful) {
                        val loans = response.body() ?: emptyList()

                        applications.clear()
                        applications.addAll(loans)

                        adapter = ManagerLoanAdapter(applications)
                        rvManagerLoans.adapter = adapter

                        updateStatistics()
                    } else {
                        Toast.makeText(
                            this@Manager_Page,
                            "Failed to load applications: ${response.code()}",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(
                        this@Manager_Page,
                        "Error loading applications",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }

    private fun updateStatistics() {
        val total = applications.size

        val pending = applications.count {
            it.status.isNullOrEmpty() ||
                    it.status.equals("Submitted", true) ||
                    it.status.equals("Pending", true) ||
                    it.status.equals("Verified", true)
        }

        val approved = applications.count {
            it.status.equals("Approved", true)
        }

        val rejected = applications.count {
            it.status.equals("Rejected", true)
        }

        totalApplicationsText.text = total.toString()
        pendingApplicationsText.text = pending.toString()
        approvedApplicationsText.text = approved.toString()
        rejectedApplicationsText.text = rejected.toString()
    }

    private fun logout() {
        val prefs = getSharedPreferences("Auth", MODE_PRIVATE)
        prefs.edit().clear().apply()

        val appPrefs = getSharedPreferences("AppPrefs", MODE_PRIVATE)
        appPrefs.edit().clear().apply()

        RetrofitClient.authToken = null

        val intent = Intent(this, Login_Page::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
        finish()
    }
}