package com.example.prototype_2

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.prototype_2.API.LoanDto

class ManagerLoanAdapter(
    private val applications: MutableList<LoanDto>,
    private val listener: ManagerActionListener
) : RecyclerView.Adapter<ManagerLoanAdapter.LoanViewHolder>() {

    interface ManagerActionListener {

        fun onApprove(loan: LoanDto)

        fun onReject(loan: LoanDto)
    }

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

        val approveButton: Button =
            itemView.findViewById(R.id.btnApprove)

        val rejectButton: Button =
            itemView.findViewById(R.id.btnReject)
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
            "Application: ${loan.applicationId ?: loan.applicationId ?: "--"}"

        holder.applicantName.text =
            loan.clientDetails?.fullNameAndSurname ?: "Unknown Client"

        val amount =
            loan.requestedAmount ?: loan.requestedAmount

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

        val status = loan.status ?: ""

        if (
            status.equals("Approved", true) ||
            status.equals("Rejected", true)
        ) {

            holder.approveButton.isEnabled = false
            holder.rejectButton.isEnabled = false

            if (status.equals("Approved", true)) {
                holder.approveButton.text = "Approved"
            } else {
                holder.rejectButton.text = "Rejected"
            }

        } else {

            holder.approveButton.isEnabled = true
            holder.rejectButton.isEnabled = true

            holder.approveButton.text = "Approve"
            holder.rejectButton.text = "Reject"
        }

        holder.approveButton.setOnClickListener {
            listener.onApprove(loan)
        }

        holder.rejectButton.setOnClickListener {
            listener.onReject(loan)
        }
    }

    override fun getItemCount(): Int {
        return applications.size
    }
}