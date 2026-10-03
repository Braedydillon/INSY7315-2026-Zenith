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
import androidx.recyclerview.widget.RecyclerView
import com.example.prototype_2.API.LoanDto
import com.example.prototype_2.API.RetrofitClient
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class Staff_Page : AppCompatActivity() {

    private lateinit var totalApplicationsText: TextView
    private lateinit var pendingApplicationsText: TextView
    private lateinit var approvedApplicationsText: TextView
    private lateinit var rvStaffLoans: RecyclerView
    private lateinit var btnLogoutStaff: Button

    private lateinit var adapter: StaffLoanAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_staff_page)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        totalApplicationsText = findViewById(R.id.TotalApplications)
        pendingApplicationsText = findViewById(R.id.PendingApplications)
        approvedApplicationsText = findViewById(R.id.ApprovedApplications)
        rvStaffLoans = findViewById(R.id.rvStaffLoans)
        btnLogoutStaff = findViewById(R.id.btnLogoutStaff)

        adapter = StaffLoanAdapter(emptyList())
        rvStaffLoans.adapter = adapter

        btnLogoutStaff.setOnClickListener {
            val prefsApp = getSharedPreferences("AppPrefs", MODE_PRIVATE)
            prefsApp.edit().clear().apply()
            RetrofitClient.authToken = null

            val intent = Intent(this, Login_Page::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            startActivity(intent)
            finish()
        }

        loadLoans()
    }

    override fun onResume() {
        super.onResume()
        loadLoans()
    }

    private fun loadLoans() {
        CoroutineScope(Dispatchers.IO).launch {
            try {

                RetrofitClient.loadToken(this@Staff_Page)
                
                val response = RetrofitClient.apiService.getAllLoans()

                withContext(Dispatchers.Main) {
                    if (response.isSuccessful) {
                        val loans = response.body() ?: emptyList()

                        val total = loans.size
                        val pending = loans.count { it.status.isNullOrEmpty() || it.status.equals("Pending", ignoreCase = true) }
                        val verifiedOrApproved = loans.count { 
                            it.status.equals("Verified", ignoreCase = true) || 
                            it.status.equals("Approved", ignoreCase = true) 
                        }

                        totalApplicationsText.text = total.toString()
                        pendingApplicationsText.text = pending.toString()
                        approvedApplicationsText.text = verifiedOrApproved.toString()

                        adapter = StaffLoanAdapter(loans)
                        rvStaffLoans.adapter = adapter

                    } else {
                        Toast.makeText(
                            this@Staff_Page,
                            "Failed to load loan applications",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(
                        this@Staff_Page,
                        "Error loading loan applications",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }
}
