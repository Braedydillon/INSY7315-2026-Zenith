# Fix Missing Loan Application Fields (PascalCase vs camelCase Serialization Mismatch)

## Problem Statement
Users report that only the Application ID and requested amount are showing, while client name, status, submission date, reason, and client details are missing or showing `--`.
Investigation reveals that the backend API (ASP.NET Core) returns JSON fields in PascalCase (`Id`, `ApplicantName`, `Status`, `SubmissionDate`, `ReasonForLoan`, `ClientDetails`, `Note`), whereas `LoanDto` was annotated with camelCase `@SerializedName` (`id`, `applicantName`, `status`, etc.). Consequently, Gson failed to parse these fields from the response.

## Proposed Changes

### API Models Layer

#### [MODIFY] [ApiModels.kt](file:///C:/Users/kelle/AndroidStudioProjects/INSY7315-2026-Zenith/Android/Prototype_2-master/app/src/main/java/com/example/prototype_2/API/ApiModels.kt)
- Update `LoanDto` to use `@SerializedName` with PascalCase primary names and camelCase alternates (e.g., `@SerializedName(value = "Id", alternate = ["id"])`, `@SerializedName(value = "ApplicantName", alternate = ["applicantName"])`, `@SerializedName(value = "Status", alternate = ["status"])`, etc.).

## Verification Plan

### Automated Tests
- Build project using gradle (`app:assembleDebug`) to verify compilation.

### Manual Verification
- Deploy and run the app.
- Check Staff Dashboard and Staff Loan Details to verify that client name, ID number, status, submission date, reason, and additional notes display correctly without showing `--`.
