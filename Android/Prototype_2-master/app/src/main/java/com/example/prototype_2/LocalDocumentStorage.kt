
package com.example.prototype_2

import android.content.Context
import android.net.Uri
import android.util.Log
import java.io.File

object LocalDocumentStorage {

    private const val TAG = "LocalDocumentStorage"

    fun saveDocument(
        context: Context,
        loanId: String,
        documentType: String,
        sourceUri: Uri
    ): String {
        require(loanId.isNotBlank()) {
            "Loan ID cannot be empty"
        }

        require(
            documentType in listOf(
                "id",
                "proof_of_address",
                "payslip"
            )
        ) {
            "Invalid document type: $documentType"
        }

        val documentsDirectory = File(
            context.filesDir,
            "loan_documents/$loanId"
        )

        if (!documentsDirectory.exists() &&
            !documentsDirectory.mkdirs()
        ) {
            throw IllegalStateException(
                "Could not create document folder"
            )
        }

        val destinationFile = File(
            documentsDirectory,
            "$documentType.document"
        )

        val inputStream = context.contentResolver
            .openInputStream(sourceUri)
            ?: throw IllegalStateException(
                "Could not open selected document"
            )

        inputStream.use { input ->
            destinationFile.outputStream().use { output ->
                input.copyTo(output)
            }
        }

        if (!destinationFile.exists() ||
            destinationFile.length() == 0L
        ) {
            throw IllegalStateException(
                "Document was not saved correctly"
            )
        }

        Log.d(
            TAG,
            "Document saved: ${destinationFile.absolutePath}"
        )

        return destinationFile.absolutePath
    }

    fun getDocument(
        context: Context,
        loanId: String,
        documentType: String
    ): File? {
        if (loanId.isBlank()) {
            Log.e(TAG, "Cannot find document: loan ID is empty")
            return null
        }

        val file = File(
            context.filesDir,
            "loan_documents/$loanId/$documentType.document"
        )

        Log.d(TAG, "Looking for document: ${file.absolutePath}")

        return if (file.exists() && file.length() > 0L) {
            Log.d(TAG, "Document found: ${file.name}")
            file
        } else {
            Log.e(TAG, "Document not found: ${file.absolutePath}")
            null
        }
    }
}
