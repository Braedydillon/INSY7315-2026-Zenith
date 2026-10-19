package com.example.prototype_2

import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import android.widget.Toast
import androidx.recyclerview.widget.RecyclerView
import com.example.prototype_2.API.LoanDto

class ManagerLoanAdapter(
    private val applications: List<LoanDto>
) : RecyclerView.Adapter<ManagerLoanAdapter.LoanViewHolder>() {

    class LoanViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

        val applicationId: TextView =
            itemView.findViewById(R.id.ApplicationId)

        val applicantName: TextView =
            itemView.findViewById(R.id.ApplicantName)

        val loanAmount: TextView =
            itemView.findViewById(R.id.LoanAmount)

        val loanReason: TextView =
            itemView.findViewById(R.id.LoanReason)

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
                R.layout.activity_item_manager_loan,
                parent,
                false
            )

        return LoanViewHolder(view)
    }

    override fun onBindViewHolder(
        holder: LoanViewHolder,
        position: Int
    ) {

        val loan = applications[position]

        holder.applicationId.text =
            "Application: ${loan.applicationId ?: "--"}"

        holder.applicantName.text =
            loan.clientDetails?.fullNameAndSurname ?: "Unknown Client"

        val amount = loan.requestedAmount

        holder.loanAmount.text =
            if (amount != null) {
                "Requested Amount: R ${String.format("%,.2f", amount)}"
            } else {
                "Requested Amount: R --"
            }

        holder.loanReason.text =
            "Reason: ${loan.reasonForLoan ?: "--"}"

        holder.submissionDate.text =
            "Submitted: ${loan.applicationDate ?: "--"}"

        holder.loanStatus.text =
            "Status: ${loan.status ?: "Pending"}"

        holder.viewDetailsButton.setOnClickListener {
            val context = holder.itemView.context
            val id = loan.applicationId

            if (id.isNullOrEmpty()) {
                Toast.makeText(
                    context,
                    "Application ID is missing",
                    Toast.LENGTH_LONG
                ).show()
                return@setOnClickListener
            }

            val intent = Intent(
                context,
                ManagerLoanDetailsActivity::class.java
            )
            intent.putExtra("LOAN_ID", id)
            context.startActivity(intent)
        }
    }

    override fun getItemCount(): Int {
        return applications.size
    }
}