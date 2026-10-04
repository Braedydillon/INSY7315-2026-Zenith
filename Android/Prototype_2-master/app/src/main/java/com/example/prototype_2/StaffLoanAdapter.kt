package com.example.prototype_2

import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.prototype_2.API.LoanDto

class StaffLoanAdapter(
    private val applications: List<LoanDto>
) : RecyclerView.Adapter<StaffLoanAdapter.LoanViewHolder>() {

    class LoanViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

        val applicantName: TextView =
            itemView.findViewById(R.id.ApplicantName)

        val applicationId: TextView =
            itemView.findViewById(R.id.ApplicationId)

        val loanAmount: TextView =
            itemView.findViewById(R.id.LoanAmount)

        val submissionDate: TextView =
            itemView.findViewById(R.id.SubmissionDate)

        val loanStatus: TextView =
            itemView.findViewById(R.id.LoanStatus)

        val viewDetailsButton: Button =
            itemView.findViewById(R.id.btnViewDetails)
    }

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): LoanViewHolder {

        val view = LayoutInflater.from(parent.context)
            .inflate(
                R.layout.activity_item_staff_loan,
                parent,
                false
            )

        return LoanViewHolder(view)
    }

    override fun onBindViewHolder(
        holder: LoanViewHolder,
        position: Int
    ) {

        val application = applications[position]

        // Client name
        holder.applicantName.text =
            application.clientDetails?.fullNameAndSurname
                ?: "Unknown Client"

        // Application ID
        holder.applicationId.text =
            "Application: ${application.applicationId ?: "--"}"

        // Loan amount
        val amount = application.requestedAmount

        holder.loanAmount.text =
            if (amount != null) {
                "Amount: R ${String.format("%,.2f", amount)}"
            } else {
                "Amount: R --"
            }

        // Application date
        holder.submissionDate.text =
            "Submitted: ${application.applicationDate ?: "--"}"

        // Status
        holder.loanStatus.text =
            "Status: ${application.status ?: "Pending"}"

        // View details button
        holder.viewDetailsButton.setOnClickListener {

            val context = holder.itemView.context

            val id = application.applicationId

            if (id.isNullOrEmpty()) {
                android.widget.Toast.makeText(
                    context,
                    "Application ID is missing",
                    android.widget.Toast.LENGTH_LONG
                ).show()

                return@setOnClickListener
            }

            val intent = Intent(
                context,
                StaffLoanDetailsActivity::class.java
            )

            intent.putExtra("LOAN_ID", id)

            context.startActivity(intent)
        }
    }

    override fun getItemCount(): Int {
        return applications.size
    }
}