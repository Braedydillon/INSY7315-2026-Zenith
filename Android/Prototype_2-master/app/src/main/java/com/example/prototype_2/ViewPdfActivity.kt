package com.example.prototype_2

import android.os.Bundle
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar

class ViewPdfActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_view_pdf_mock)

        val toolbar = findViewById<Toolbar>(R.id.pdfToolbar)
        setSupportActionBar(toolbar)
        supportActionBar?.setDisplayHomeAsUpEnabled(true)
        toolbar.setNavigationOnClickListener {
            onBackPressedDispatcher.onBackPressed()
        }

        val applicantName = intent.getStringExtra("APPLICANT_NAME") ?: "N/A"
        findViewById<TextView>(R.id.pdfApplicantName).text = "Full Name: $applicantName"
    }
}
