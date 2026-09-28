package com.example.prototype_2

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat

class Staff_Page : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_staff_page)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        // Handle PDF View clicks for mock data
        findViewById<Button>(R.id.btnViewPdf1).setOnClickListener {
            openPdfViewer("John Doe")
        }
        findViewById<Button>(R.id.btnViewPdf2).setOnClickListener {
            openPdfViewer("Sarah Smith")
        }
        findViewById<Button>(R.id.btnViewPdf3).setOnClickListener {
            openPdfViewer("Michael Brown")
        }

        findViewById<Button>(R.id.btnLogoutStaff).setOnClickListener {
            val intent = Intent(this, Login_Page::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            startActivity(intent)
            finish()
        }
    }

    private fun openPdfViewer(applicantName: String) {
        val intent = Intent(this, ViewPdfActivity::class.java)
        intent.putExtra("APPLICANT_NAME", applicantName)
        startActivity(intent)
    }
}