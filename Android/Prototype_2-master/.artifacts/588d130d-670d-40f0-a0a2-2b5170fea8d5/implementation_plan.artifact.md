# Fix Issues on Track Application Page

Fix malformed XML, layout constraint issues, and implement backend loan tracking logic in `TrackApplicationActivity`.

## User Review Required

> [!NOTE]
> This plan addresses XML syntax errors in `activity_track_application.xml`, fixes empty state constraints, and adds API integration (`getMyLoans`) to display submitted loan applications or fallback to an empty state card.

## Open Questions

- None. The API endpoint `/api/LoansApi/mine` and `LoanDto` model are already available in `ApiService.kt` and `ApiModels.kt`.

## Proposed Changes

### UI & Layout

#### [MODIFY] [activity_track_application.xml](file:///C:/Users/kelle/AndroidStudioProjects/INSY7315-2026-Zenith/Android/Prototype_2-master/app/src/main/res/layout/activity_track_application.xml)
- Remove malformed CDATA / stray XML attributes block.
- Add correct constraints for `cardEmptyState` below `trackHeaderBg`.
- Ensure `rvApplications` is properly positioned and constrained below header.

### Activity & Logic

#### [MODIFY] [TrackApplicationActivity.kt](file:///C:/Users/kelle/AndroidStudioProjects/INSY7315-2026-Zenith/Android/Prototype_2-master/app/src/main/java/com/example/prototype_2/TrackApplicationActivity.kt)
- Implement `RecyclerView` adapter for `LoanDto` items using `item_loan_application.xml`.
- Fetch user's loan applications from `RetrofitClient.apiService.getMyLoans(...)` on startup.
- Toggle visibility between `cardEmptyState` and `rvApplications` based on whether loans are returned.
- Handle network errors and loading states gracefully.

## Verification Plan

### Automated Tests
- Run Gradle build to ensure project compiles without errors (`app:assembleDebug`).

### Manual Verification
- Deploy app to emulator/device, log in, navigate to Track Applications page, and verify whether list or empty state displays correctly.
