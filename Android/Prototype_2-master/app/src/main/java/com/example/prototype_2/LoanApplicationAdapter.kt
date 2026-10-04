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
        val txtAppId: TextView = itemView.findViewById(R.id.txtAppId)
        val txtDate: TextView = itemView.findViewById(R.id.txtDate)
        val txtAmount: TextView = itemView.findViewById(R.id.txtAmount)
        val txtStatus: TextView = itemView.findViewById(R.id.txtStatus)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): LoanViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.activity_item_loan_application, parent, false)

        return LoanViewHolder(view)
    }

    override fun onBindViewHolder(holder: LoanViewHolder, position: Int) {
        val application = applications[position]

        holder.txtAppId.text = "Application: ${application.applicationId?: "--"}"
        holder.txtDate.text = "Submitted: ${application.applicationDate ?: "--"}"

        val loanAmount = application.requestedAmount ?: application.requestedAmount

        holder.txtAmount.text = if (loanAmount != null) {
            val formatter = NumberFormat.getCurrencyInstance(Locale("en", "ZA"))
            "Amount: ${formatter.format(loanAmount)}"
        } else {
            "Amount: R --"
        }

        holder.txtStatus.text = application.status ?: "Pending"
    }

    override fun getItemCount(): Int {
        return applications.size
    }

    fun updateApplications(newApplications: List<LoanDto>) {
        applications = newApplications
        notifyDataSetChanged()
    }
}