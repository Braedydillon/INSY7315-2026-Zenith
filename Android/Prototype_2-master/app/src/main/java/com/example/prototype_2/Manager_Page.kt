package com.example.prototype_2

import android.annotation.SuppressLint
import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.Button
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.prototype_2.API.RetrofitClient
import com.example.prototype_2.API.UpdateStatusRequest
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

    private var applications = mutableListOf<com.example.prototype_2.API.LoanDto>()

    @SuppressLint("MissingInflatedId")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_manager_page)

        totalApplicationsText = findViewById(R.id.TotalApplications)
        pendingApplicationsText = findViewById(R.id.PendingApplications)
        approvedApplicationsText = findViewById(R.id.ApprovedApplications)
        rejectedApplicationsText = findViewById(R.id.RejectedApplications)

        rvManagerLoans = findViewById(R.id.rvManagerLoans)
        btnLogoutManager = findViewById(R.id.btnLogoutManager)

        adapter = ManagerLoanAdapter(
            applications,
            object : ManagerLoanAdapter.ManagerActionListener {

                override fun onApprove(loan: com.example.prototype_2.API.LoanDto) {
                    updateLoanStatus(loan, "Approved")
                }

                override fun onReject(loan: com.example.prototype_2.API.LoanDto) {
                    updateLoanStatus(loan, "Rejected")
                }
            }
        )

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

    private fun loadApplications() {

        CoroutineScope(Dispatchers.IO).launch {

            try {

                var token = RetrofitClient.authToken

                if (token.isNullOrEmpty()) {

                    val prefs = getSharedPreferences(
                        "Auth",
                        MODE_PRIVATE
                    )

                    token = prefs.getString(
                        "AUTH_TOKEN",
                        null
                    )

                    if (token.isNullOrEmpty()) {
                        token = prefs.getString(
                            "token",
                            null
                        )
                    }
                }

                if (token.isNullOrEmpty()) {

                    withContext(Dispatchers.Main) {
                        Toast.makeText(
                            this@Manager_Page,
                            "Please log in again",
                            Toast.LENGTH_SHORT
                        ).show()
                    }

                    return@launch
                }

                RetrofitClient.authToken = token

                val response = RetrofitClient.apiService.getAllLoans()

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        val loans = response.body()

                        if (loans != null) {

                            applications.clear()
                            applications.addAll(loans)

                            adapter.notifyDataSetChanged()

                            updateStatistics()

                        } else {

                            Toast.makeText(
                                this@Manager_Page,
                                "No applications found",
                                Toast.LENGTH_SHORT
                            ).show()
                        }

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

    private fun updateLoanStatus(
        loan: com.example.prototype_2.API.LoanDto,
        newStatus: String
    ) {

        val id = loan.applicationId ?: loan.applicationId

        if (id.isNullOrEmpty()) {

            Toast.makeText(
                this,
                "Application ID is missing",
                Toast.LENGTH_SHORT
            ).show()

            return
        }

        CoroutineScope(Dispatchers.IO).launch {

            try {

                var token = RetrofitClient.authToken

                if (token.isNullOrEmpty()) {

                    val prefs = getSharedPreferences(
                        "Auth",
                        MODE_PRIVATE
                    )

                    token = prefs.getString(
                        "AUTH_TOKEN",
                        null
                    )

                    if (token.isNullOrEmpty()) {
                        token = prefs.getString(
                            "token",
                            null
                        )
                    }
                }

                if (token.isNullOrEmpty()) {

                    withContext(Dispatchers.Main) {

                        Toast.makeText(
                            this@Manager_Page,
                            "Please log in again",
                            Toast.LENGTH_SHORT
                        ).show()
                    }

                    return@launch
                }

                RetrofitClient.authToken = token

                val request = UpdateStatusRequest(
                    status = newStatus,
                    comments = if (newStatus == "Approved") {
                        "Application approved by manager."
                    } else {
                        "Application rejected by manager."
                    }
                )

                val response =
                    RetrofitClient.apiService.updateLoanStatus(
                        id = id,
                        request = request
                    )

                withContext(Dispatchers.Main) {

                    if (response.isSuccessful) {

                        loan.status = newStatus

                        adapter.notifyDataSetChanged()

                        updateStatistics()

                        Toast.makeText(
                            this@Manager_Page,
                            "Application $newStatus",
                            Toast.LENGTH_SHORT
                        ).show()

                    } else {

                        Toast.makeText(
                            this@Manager_Page,
                            "Could not update application: ${response.code()}",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }

            } catch (e: Exception) {

                withContext(Dispatchers.Main) {

                    Toast.makeText(
                        this@Manager_Page,
                        "Error updating application",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }

    private fun logout() {

        val prefs = getSharedPreferences(
            "Auth",
            MODE_PRIVATE
        )

        prefs.edit().clear().apply()

        val appPrefs = getSharedPreferences(
            "AppPrefs",
            MODE_PRIVATE
        )

        appPrefs.edit().clear().apply()

        RetrofitClient.authToken = null

        val intent = Intent(
            this,
            Login_Page::class.java
        )

        intent.flags =
            Intent.FLAG_ACTIVITY_NEW_TASK or
                    Intent.FLAG_ACTIVITY_CLEAR_TASK

        startActivity(intent)

        finish()
    }
}