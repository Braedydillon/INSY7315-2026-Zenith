
package com.example.prototype_2

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.inputmethod.EditorInfo
import android.widget.EditText
import android.widget.ImageButton
import android.widget.TextView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.chip.Chip
import com.google.android.material.chip.ChipGroup
import com.google.android.material.floatingactionbutton.FloatingActionButton
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale
import java.util.UUID

class Chatbot_Page : AppCompatActivity() {

    private lateinit var recyclerView: RecyclerView
    private lateinit var edtChatMessage: EditText
    private lateinit var btnSendMessage: FloatingActionButton
    private lateinit var btnBack: ImageButton
    private lateinit var chatAdapter: ChatAdapter

    private val messageList = mutableListOf<ChatMessage>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_chatbot_page)

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
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

        initViews()
        setupRecyclerView()
        setupListeners()

        // Welcome message
        addBotMessage(
            "Hello! 👋 Welcome to the Bridge & Anchor Assistant.\n\n" +
                    "I can help you with loan information, required documents, " +
                    "the application process, branch locations and contacting our team.",
            listOf(
                ChatOption("💰 Loan Information") {
                    sendUserQuery("How much can I borrow?")
                },
                ChatOption("📄 Required Documents") {
                    sendUserQuery("What documents do I need?")
                },
                ChatOption("📝 How to Apply") {
                    sendUserQuery("How do I apply?")
                }
            )
        )
    }

    private fun initViews() {
        recyclerView = findViewById(R.id.chatRecyclerView)
        edtChatMessage = findViewById(R.id.edtChatMessage)
        btnSendMessage = findViewById(R.id.btnSendMessage)
        btnBack = findViewById(R.id.btnBack)
    }

    private fun setupRecyclerView() {

        chatAdapter = ChatAdapter(messageList)

        recyclerView.layoutManager = LinearLayoutManager(this).apply {
            stackFromEnd = true
        }

        recyclerView.adapter = chatAdapter
    }

    private fun setupListeners() {

        btnBack.setOnClickListener {
            finish()
        }

        btnSendMessage.setOnClickListener {
            handleUserMessageSubmit()
        }

        edtChatMessage.setOnEditorActionListener { _, actionId, _ ->

            if (actionId == EditorInfo.IME_ACTION_SEND) {
                handleUserMessageSubmit()
                true
            } else {
                false
            }
        }

        findViewById<Chip>(R.id.chipLoanInfo)?.setOnClickListener {
            sendUserQuery("How much can I borrow?")
        }

        findViewById<Chip>(R.id.chipDocs)?.setOnClickListener {
            sendUserQuery("What documents do I need?")
        }

        findViewById<Chip>(R.id.chipApply)?.setOnClickListener {
            sendUserQuery("How do I apply?")
        }

        findViewById<Chip>(R.id.chipBranches)?.setOnClickListener {
            sendUserQuery("Where are the branches?")
        }

        findViewById<Chip>(R.id.chipContact)?.setOnClickListener {
            sendUserQuery("How can I contact Bridge and Anchor?")
        }
    }

    private fun handleUserMessageSubmit() {

        val text = edtChatMessage.text.toString().trim()

        if (text.isNotEmpty()) {

            edtChatMessage.setText("")

            sendUserQuery(text)
        }
    }

    private fun sendUserQuery(userQuery: String) {

        addUserMessage(userQuery)

        generateBotResponse(userQuery)
    }

    private fun addUserMessage(text: String) {

        val userMsg = ChatMessage(
            id = UUID.randomUUID().toString(),
            sender = Sender.USER,
            text = text,
            timestamp = getCurrentTime()
        )

        messageList.add(userMsg)

        chatAdapter.notifyItemInserted(messageList.size - 1)

        recyclerView.smoothScrollToPosition(messageList.size - 1)
    }

    private fun addBotMessage(
        text: String,
        options: List<ChatOption> = emptyList()
    ) {

        val botMsg = ChatMessage(
            id = UUID.randomUUID().toString(),
            sender = Sender.BOT,
            text = text,
            timestamp = getCurrentTime(),
            options = options
        )

        messageList.add(botMsg)

        chatAdapter.notifyItemInserted(messageList.size - 1)

        recyclerView.smoothScrollToPosition(messageList.size - 1)
    }

    private fun generateBotResponse(query: String) {

        val lower = query
            .lowercase(Locale.ROOT)
            .trim()

        when {

            // GREETING
            lower == "hi" ||
                    lower == "hello" ||
                    lower == "hey" ||
                    lower.contains("good morning") ||
                    lower.contains("good afternoon") -> {

                addBotMessage(
                    "Hello! 👋 How can I help you today?\n\n" +
                            "You can ask me about loan amounts, repayment terms, " +
                            "required documents, applications or branch locations.",
                    listOf(
                        ChatOption("💰 Loan Information") {
                            sendUserQuery("How much can I borrow?")
                        },
                        ChatOption("📄 Required Documents") {
                            sendUserQuery("What documents do I need?")
                        }
                    )
                )
            }

            // LOAN AMOUNTS & REPAYMENT
            lower.contains("how much can i borrow") ||
                    lower.contains("loan amount") ||
                    lower.contains("borrow") ||
                    lower.contains("repayment") ||
                    lower.contains("repay") ||
                    lower.contains("repayment period") ||
                    lower.contains("how long can i repay") ||
                    lower.contains("loan terms") -> {

                addBotMessage(
                    "💰 Loan Amounts & Repayment Terms:\n\n" +
                            "• Loan amounts range from R300 to R7,000.\n" +
                            "• Repayment periods range from 1 to 6 months.\n\n" +
                            "ℹ️ Please note: I am an automated support assistant. " +
                            "I cannot make affordability, credit or loan-approval decisions. " +
                            "Actual applications are assessed through Bridge & Anchor's formal process.",
                    listOf(
                        ChatOption("📄 Required Documents") {
                            sendUserQuery("What documents do I need?")
                        },
                        ChatOption("📝 How to Apply") {
                            sendUserQuery("How do I apply?")
                        }
                    )
                )
            }

            // DOCUMENTS / REQUIREMENTS
            lower.contains("what documents") ||
                    lower.contains("required documents") ||
                    lower.contains("documents do i need") ||
                    lower.contains("loan requirements") ||
                    lower.contains("requirements") ||
                    lower.contains("payslip") ||
                    lower.contains("bank statement") ||
                    lower.contains("proof of address") -> {

                addBotMessage(
                    "📄 Required Documents:\n\n" +
                            "To apply for a loan with Bridge & Anchor, " +
                            "the following documents are listed as requirements:\n\n" +
                            "1. 🆔 South African ID\n" +
                            "2. 🏠 Proof of address\n" +
                            "3. 💵 Payslip\n" +
                            "4. 🏦 3 months bank statements\n\n" +
                            "It is a good idea to have these documents ready when applying.",
                    listOf(
                        ChatOption("📝 How to Apply") {
                            sendUserQuery("How do I apply?")
                        },
                        ChatOption("🏢 Branch Locations") {
                            sendUserQuery("Where are the branches?")
                        }
                    )
                )
            }

            // APPLICATION
            lower.contains("how do i apply") ||
                    lower.contains("apply for a loan") ||
                    lower.contains("loan application") ||
                    lower.contains("application process") ||
                    lower.contains("application") -> {

                addBotMessage(
                    "📝 Loan Application:\n\n" +
                            "To apply, prepare the required documents and " +
                            "follow the Bridge & Anchor application process.\n\n" +
                            "You can also visit a Bridge & Anchor branch or " +
                            "contact the team for assistance with your application.\n\n" +
                            "Would you like to view the branch information or contact the team?",
                    listOf(
                        ChatOption("📞 Contact Staff") {
                            openContactUsPage()
                        },
                        ChatOption("🏢 View Branches") {
                            sendUserQuery("Where are the branches?")
                        }
                    )
                )
            }

            // AFTER APPLICATION / STATUS
            lower.contains("what happens after") ||
                    lower.contains("after i apply") ||
                    lower.contains("after applying") ||
                    lower.contains("application status") ||
                    lower.contains("check my application") -> {

                addBotMessage(
                    "⏳ After You Apply:\n\n" +
                            "Your application will go through Bridge & Anchor's " +
                            "formal assessment process.\n\n" +
                            "If you need an update on an existing application, " +
                            "please contact Bridge & Anchor staff directly.",
                    listOf(
                        ChatOption("📞 Contact Staff") {
                            openContactUsPage()
                        },
                        ChatOption("🏢 Branch Information") {
                            sendUserQuery("Where are the branches?")
                        }
                    )
                )
            }

            // BRANCHES
            lower.contains("where are the branches") ||
                    lower.contains("branch location") ||
                    lower.contains("branch locations") ||
                    lower.contains("branch address") ||
                    lower.contains("where is the branch") -> {

                showBranchInformation()
            }

            // CONTACT
            lower.contains("contact") ||
                    lower.contains("phone number") ||
                    lower.contains("telephone") ||
                    lower.contains("email") ||
                    lower.contains("call") ||
                    lower.contains("speak to someone") ||
                    lower.contains("human support") -> {

                showBranchInformation()
            }

            // FALLBACK
            else -> {

                addBotMessage(
                    "I'm sorry, I couldn't quite understand your question. 😕\n\n" +
                            "I can help with:\n\n" +
                            "• Loan amounts\n" +
                            "• Repayment periods\n" +
                            "• Required documents\n" +
                            "• How to apply\n" +
                            "• What happens after applying\n" +
                            "• Branch locations\n" +
                            "• Contact information\n\n" +
                            "If you need further assistance, you can contact the Bridge & Anchor team.",
                    listOf(
                        ChatOption("📞 Contact Staff") {
                            openContactUsPage()
                        },
                        ChatOption("🏢 Branch Information") {
                            showBranchInformation()
                        }
                    )
                )
            }
        }
    }

    private fun showBranchInformation() {

        addBotMessage(
            "🏢 Bridge & Anchor Branch Information:\n\n" +

                    "📍 Branch 1 – Church Street\n" +
                    "Shop 2, 200 Church Street, Pietermaritzburg, 3201\n" +
                    "📞 Tel: 033 394 3154\n" +
                    "📱 Cell: 076 678 9661\n" +
                    "✉️ Email: Bridgeanchor@gmail.com\n\n" +

                    "📍 Branch 2 – Boshoff Street\n" +
                    "161 Boshoff Street, Pietermaritzburg, 3201\n" +
                    "📞 Tel: 033 345 8021\n" +
                    "📱 Cell: 062 769 2868\n" +
                    "✉️ Email: Bridgeanchor3@gmail.com",

            listOf(
                ChatOption("📞 Call Church Street") {
                    dialNumber("0333943154")
                },
                ChatOption("📞 Call Boshoff Street") {
                    dialNumber("0333458021")
                },
                ChatOption("📱 Contact Page") {
                    openContactUsPage()
                }
            )
        )
    }

    private fun openContactUsPage() {

        try {

            val intent = Intent(
                this,
                Contact_Us_Page::class.java
            )

            startActivity(intent)

        } catch (e: Exception) {

            Toast.makeText(
                this,
                "Unable to open the contact page.",
                Toast.LENGTH_SHORT
            ).show()
        }
    }

    private fun dialNumber(phoneNumber: String) {

        try {

            val intent = Intent(
                Intent.ACTION_DIAL,
                Uri.parse("tel:$phoneNumber")
            )

            startActivity(intent)

        } catch (e: Exception) {

            Toast.makeText(
                this,
                "Unable to open the phone dialer.",
                Toast.LENGTH_SHORT
            ).show()
        }
    }

    private fun getCurrentTime(): String {

        val sdf = SimpleDateFormat(
            "hh:mm a",
            Locale.getDefault()
        )

        return sdf.format(Date())
    }

    // --------------------------------------------------
    // DATA MODELS
    // --------------------------------------------------

    enum class Sender {
        USER,
        BOT
    }

    data class ChatOption(
        val label: String,
        val action: () -> Unit
    )

    data class ChatMessage(
        val id: String,
        val sender: Sender,
        val text: String,
        val timestamp: String,
        val options: List<ChatOption> = emptyList()
    )

    // --------------------------------------------------
    // RECYCLERVIEW ADAPTER
    // --------------------------------------------------

    private inner class ChatAdapter(
        private val items: List<ChatMessage>
    ) : RecyclerView.Adapter<ChatAdapter.ChatViewHolder>() {

        inner class ChatViewHolder(
            itemView: View
        ) : RecyclerView.ViewHolder(itemView) {

            val layoutBotMessage: View =
                itemView.findViewById(R.id.layoutBotMessage)

            val txtBotText: TextView =
                itemView.findViewById(R.id.txtBotText)

            val txtBotTime: TextView =
                itemView.findViewById(R.id.txtBotTime)

            val layoutActionButtons: ChipGroup =
                itemView.findViewById(R.id.layoutActionButtons)

            val layoutUserMessage: View =
                itemView.findViewById(R.id.layoutUserMessage)

            val txtUserText: TextView =
                itemView.findViewById(R.id.txtUserText)

            val txtUserTime: TextView =
                itemView.findViewById(R.id.txtUserTime)
        }

        override fun onCreateViewHolder(
            parent: ViewGroup,
            viewType: Int
        ): ChatViewHolder {

            val view = LayoutInflater
                .from(parent.context)
                .inflate(
                    R.layout.activity_item_chat_message,
                    parent,
                    false
                )

            return ChatViewHolder(view)
        }

        override fun onBindViewHolder(
            holder: ChatViewHolder,
            position: Int
        ) {

            val message = items[position]

            if (message.sender == Sender.BOT) {

                holder.layoutBotMessage.visibility = View.VISIBLE
                holder.layoutUserMessage.visibility = View.GONE

                holder.txtBotText.text = message.text
                holder.txtBotTime.text = message.timestamp

                holder.layoutActionButtons.removeAllViews()

                if (message.options.isNotEmpty()) {

                    holder.layoutActionButtons.visibility =
                        View.VISIBLE

                    for (option in message.options) {

                        val chip = Chip(holder.itemView.context).apply {
                            text = option.label
                            textSize = 12f
                            setChipBackgroundColorResource(android.R.color.white)
                            chipStrokeWidth = 2f
                            chipStrokeColor = android.content.res.ColorStateList.valueOf(
                                android.graphics.Color.parseColor("#1769E0")
                            )
                            setTextColor(android.graphics.Color.parseColor("#1769E0"))
                            setOnClickListener {
                                option.action.invoke()
                            }
                        }

                        holder.layoutActionButtons.addView(chip)
                    }

                } else {

                    holder.layoutActionButtons.visibility =
                        View.GONE
                }

            } else {

                holder.layoutUserMessage.visibility =
                    View.VISIBLE

                holder.layoutBotMessage.visibility =
                    View.GONE

                holder.txtUserText.text =
                    message.text

                holder.txtUserTime.text =
                    message.timestamp
            }
        }

        override fun getItemCount(): Int =
            items.size
    }
}