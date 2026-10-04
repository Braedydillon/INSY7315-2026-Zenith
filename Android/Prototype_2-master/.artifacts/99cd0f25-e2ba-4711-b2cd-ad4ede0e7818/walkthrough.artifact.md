# Walkthrough - Fixing Staff Dashboard and Loan Details Display Issues

## Changes Made

### API Models Layer
#### [MODIFY] [ApiModels.kt](file:///C:/Users/kelle/AndroidStudioProjects/INSY7315-2026-Zenith/Android/Prototype_2-master/app/src/main/java/com/example/prototype_2/API/ApiModels.kt)
- Updated `LoanDto` and `ClientDetailsRequest` with comprehensive `@SerializedName(value = ..., alternate = [...])` mappings covering PascalCase, camelCase, lowercase, and snake_case JSON variants.
- Added flat top-level fallback properties (`topLevelFullName`, `topLevelIdNumber`, `topLevelCellNo`, `topLevelOccupation`, `topLevelPhysicalAddress`) in `LoanDto` to handle backends that return client details flattened at the root level of the loan object.

### Staff Dashboard & Adapters
#### [MODIFY] [StaffLoanAdapter.kt](file:///C:/Users/kelle/AndroidStudioProjects/INSY7315-2026-Zenith/Android/Prototype_2-master/app/src/main/java/com/example/prototype_2/StaffLoanAdapter.kt)
- Updated applicant name resolution to check `applicantName ?: clientDetails?.fullNameAndSurname ?: topLevelFullName ?: "Unknown Client"`.

### Staff Loan Details Activity
#### [MODIFY] [StaffLoanDetailsActivity.kt](file:///C:/Users/kelle/AndroidStudioProjects/INSY7315-2026-Zenith/Android/Prototype_2-master/app/src/main/java/com/example/prototype_2/StaffLoanDetailsActivity.kt)
- Updated client name and client details binding to check nested `clientDetails` as well as top-level flat fallbacks (`topLevelFullName`, `topLevelIdNumber`, `topLevelCellNo`, `topLevelOccupation`, `topLevelPhysicalAddress`), preventing `--` when data is present in any JSON format.

---

## Verification Results

### Automated Tests
- Executed Gradle build (`app:assembleDebug`) successfully with zero errors.
  ```
  BUILD SUCCESSFUL
  ```
