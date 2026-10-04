package com.example.prototype_2

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.prototype_2.API.LoanDto
import java.text.NumberFormat
import java.util.Locale

class LoanApplicationAdapter(
    private var applications: List<LoanDto>
) : RecyclerView.Adapter<LoanApplicationAdapter.LoanViewHolder>() {

    class LoanViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

        val applicantName: TextView =
            itemView.findViewById(R.id.ApplicantName)

        val appId: TextView =
            itemView.findViewById(R.id.AppId)

        val date: TextView =
            itemView.findViewById(R.id.Date)

        val amount: TextView =
            itemView.findViewById(R.id.Amount)

        val status: TextView =
            itemView.findViewById(R.id.Status)
    }

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): LoanViewHolder {

        val view = LayoutInflater.from(parent.context)
            .inflate(
                R.layout.activity_item_loan_application,
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

        // Applicant name
        holder.applicantName.text =
            "Applicant Name: ${application.clientDetails?.fullNameAndSurname ?: "--"}"

        // Application ID
        holder.appId.text =
            "Application ID: ${application.applicationId ?: "--"}"

        // Submitted date
        holder.date.text =
            "Submitted Date: ${application.applicationDate ?: "--"}"

        // Loan amount
        val loanAmount = application.requestedAmount

        holder.amount.text =
            if (loanAmount != null) {
                val formatter =
                    NumberFormat.getCurrencyInstance(Locale("en", "ZA"))

                "Amount: ${formatter.format(loanAmount)}"
            } else {
                "Amount: R --"
            }

        // Status
        holder.status.text =
            application.status ?: "Pending"
    }

    override fun getItemCount(): Int {
        return applications.size
    }

    fun updateApplications(
        newApplications: List<LoanDto>
    ) {
        applications = newApplications
        notifyDataSetChanged()
    }
}