package com.example.prototype_2

import android.os.Bundle
import android.view.View
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

class TrackApplicationActivity : AppCompatActivity() {

    private lateinit var rvApplications: RecyclerView
    private lateinit var cardEmptyState: View
    private lateinit var adapter: LoanApplicationAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_track_application)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(
                systemBars.left,
                systemBars.top,
                systemBars.right,
                systemBars.bottom
            )
            insets
        }

        rvApplications = findViewById(R.id.rvApplications)
        cardEmptyState = findViewById(R.id.cardEmptyState)

        adapter = LoanApplicationAdapter(emptyList())
        rvApplications.adapter = adapter

        loadApplications()
    }

    private fun loadApplications() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val response = RetrofitClient.apiService.getMyLoans()

                withContext(Dispatchers.Main) {
                    if (response.isSuccessful) {
                        val applications = response.body() ?: emptyList()

                        if (applications.isEmpty()) {
                            showEmptyState()
                        } else {
                            showApplications(applications)
                        }
                    } else {
                        showEmptyState()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    showEmptyState()
                }
            }
        }
    }

    private fun showEmptyState() {
        cardEmptyState.visibility = View.VISIBLE
        rvApplications.visibility = View.GONE
    }

    private fun showApplications(applications: List<LoanDto>) {
        cardEmptyState.visibility = View.GONE
        rvApplications.visibility = View.VISIBLE
        adapter.updateApplications(applications)
    }
}