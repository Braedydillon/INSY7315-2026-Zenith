package com.example.prototype_2

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.Button
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.google.android.material.floatingactionbutton.FloatingActionButton

class MainActivity : AppCompatActivity() {

    private lateinit var btnPolicy: Button
    private lateinit var btnCall : Button
    private lateinit var btnRegister: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_main)

        findViewById<Button>(R.id.btnPolicy)?.let { btn ->
            btnPolicy = btn
            btnPolicy.setOnClickListener {
                val intent = Intent(this, Policy_Page::class.java)
                startActivity(intent)
            }
        }

        findViewById<FloatingActionButton>(R.id.btnWhatsapp)?.setOnClickListener {
            val intent = Intent(this, Staff_Page::class.java)
            startActivity(intent)
        }

        findViewById<FloatingActionButton>(R.id.btnChatbot)?.setOnClickListener {
            val intent = Intent(this, Chatbot_Page::class.java)
            startActivity(intent)
        }

        findViewById<Button>(R.id.btnCall)?.let { btn ->
            btnCall = btn
            btnCall.setOnClickListener {
                val intent = Intent(this, Contact_Us_Page::class.java)
                startActivity(intent)
            }
        }

        btnRegister = findViewById(R.id.btnRegister)

        btnRegister.setOnClickListener {
            val intent = Intent(this, Register_Page::class.java)
            startActivity(intent)
        }

        findViewById<Button>(R.id.btnLogout)?.setOnClickListener {
            val intent = Intent(this, Login_Page::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            startActivity(intent)
            finish()
        }

        findViewById<View>(R.id.cardLoan)?.setOnClickListener {
            val intent = Intent(this, LoanApplicationActivity::class.java)
            startActivity(intent)
        }

        findViewById<View>(R.id.cardTrack)?.setOnClickListener {
            val intent = Intent(this, TrackApplicationActivity::class.java)
            startActivity(intent)
        }


        findViewById<View>(R.id.main)?.let { mainView ->
            ViewCompat.setOnApplyWindowInsetsListener(mainView) { v, insets ->
                val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
                v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
                insets
            }
        }
    }
}