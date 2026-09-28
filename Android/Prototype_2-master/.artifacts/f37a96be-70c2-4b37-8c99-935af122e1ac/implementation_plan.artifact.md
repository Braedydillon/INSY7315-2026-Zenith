# Implementation Plan - Fix MainActivity Crash and XML Warnings

This plan addresses a `FATAL EXCEPTION` in `MainActivity` caused by an incorrect view type casting and resolves hardcoded string warnings in `activity_main.xml`.

## User Review Required

> [!IMPORTANT]
> The crash is caused by `btnChatbot` and `btnWhatsapp` being declared as `Button` in `MainActivity.kt` while they are `FloatingActionButton` in `activity_main.xml`. This results in a `ClassCastException` at runtime.

## Proposed Changes

### [app]

#### [MODIFY] [MainActivity.kt](file:///C:/Users/Noir/AndroidStudioProjects/Prototype_2/app/src/main/java/com/example/prototype_2/MainActivity.kt)
- Update `btnChatbot` and `btnWhatsapp` variable types from `Button` to `FloatingActionButton`.
- Ensure proper imports are added.

#### [MODIFY] [strings.xml](file:///C:/Users/Noir/AndroidStudioProjects/Prototype_2/app/src/main/res/values/strings.xml)
- Add all hardcoded strings from `activity_main.xml` as string resources.

#### [MODIFY] [activity_main.xml](file:///C:/Users/Noir/AndroidStudioProjects/Prototype_2/app/src/main/res/layout/activity_main.xml)
- Replace hardcoded text and content descriptions with references to the new string resources in `strings.xml`.

## Verification Plan

### Automated Tests
- Run the application to ensure it no longer crashes on startup.
- Verify that all buttons and floating action buttons trigger the correct intents.

### Manual Verification
- Deploy the app to the emulator/device.
- Check that the UI looks as expected and all text is correctly displayed from resources.
