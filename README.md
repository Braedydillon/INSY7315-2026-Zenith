# Bridge & Anchor Loans

A short-term loan management platform for **Bridge & Anchor Loans**, built as a university project for **INSY7315**. It has two client applications that share one REST API:

| App | Stack | Audience |
|---|---|---|
| **Android app** (`/Android`) | Kotlin, Retrofit, Material Components | Clients, staff and managers on mobile |
| **Web app** (`/INSY7315_Prototype`) | ASP.NET Core MVC (.NET 8), Razor, Bootstrap | Clients, staff and managers in the browser |

Clients apply for loans of **R300 – R7,000** over **1 – 6 months**. Staff verify applications, and anything at or above **R7,000** is escalated to a manager for the final decision.

---

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
  - [Web app](#web-app)
  - [Android app](#android-app)
- [API](#api)
- [Roles](#roles)

---

## Features

### Clients
- Register and log in
- Submit a full loan application (personal, employment, marital status, spouse and divorce details, loan purpose)
- Track the status of submitted applications
- Read the company policy and NCR compliance information
- Contact page and WhatsApp shortcut *(Android)*
- Built-in FAQ chatbot covering loan amounts, required documents, how to apply, branch locations and contact details *(Android)*
- Downloadable PDF finance application form *(Web)*

### Staff
- Dashboard with counts for pending, sent to manager, approved and declined applications
- Review each application's client details
- Verification checklist (ID, address, employment, bank) that must be fully ticked before a positive decision
- Approve or decline small loans directly
- Verify and forward large loans to the manager

### Managers
- Dashboard with application statistics
- Review verified applications and record a verification result with notes
- Approve or reject large loans
- View all applications

---

The threshold lives in `LoanRules.ManagerThreshold` in the web project. The web app re-reads the loan from the API on every decision, so the amount and status are never trusted from the submitted form.

---

## Tech Stack

**Web (`INSY7315_Prototype`)**
- ASP.NET Core MVC on .NET 8
- Cookie authentication with role-based authorisation
- In-memory session (30 minute idle timeout)
- `HttpClientFactory` for API calls
- Bootstrap 5 and custom CSS

**Android (`Android`)**
- Kotlin, minSdk 24, targetSdk 37
- Retrofit 2.11.0, Gson and OkHttp logging interceptor
- Material Components, ConstraintLayout and RecyclerView
- Bearer token stored in `SharedPreferences` and attached by an OkHttp interceptor

**Backend**
- Hosted REST API at `https://apiinsy7315-latest.onrender.com/` (not part of this repository)

---

## Getting Started


### Android app

**Prerequisites:** Android Studio (recent version with Android Gradle Plugin 9.x support), JDK 11+, and an emulator or device running Android 7.0 (API 24) or higher.

1. Open the `Android` folder in Android Studio.
2. Let Gradle sync finish.
3. Select an emulator or connected device.
4. Click **Run**.

The app needs an internet connection because it calls the hosted API.

> **Note:** The API is hosted on Render. If the service has been idle, the first request (login, for example) may take a little longer while it wakes up.

## API

Both clients use the same base URL, set in `RetrofitClient.kt` (Android) and `Program.cs` (Web):

```
https://apiinsy7315-latest.onrender.com/
```

Endpoints used by the apps:

| Method | Endpoint | Purpose |
|---|---|---|
| `POST` | `/api/Auth/register` | Register a new client |
| `POST` | `/api/Auth/login` | Log in, returns token and role |
| `POST` | `/api/Auth/refresh` | Refresh a session token |
| `GET` | `/api/Me` | Current user profile |
| `POST` | `/api/LoansApi` | Submit a loan application |
| `GET` | `/api/LoansApi` | All applications (staff / manager) |
| `GET` | `/api/LoansApi/mine` | The signed-in client's applications |
| `GET` | `/api/LoansApi/pending` | Pending applications |
| `GET` | `/api/LoansApi/{id}` | A single application |
| `PUT` | `/api/LoansApi/{id}/status` | Update application status |
| `GET` | `/api/Admin/users` | List users (admin) |
| `POST` | `/api/Admin/set-role` | Assign a role (admin) |
| `GET` | `/health`, `/health/firestore` | Health checks |

Authenticated requests send `Authorization: Bearer <token>`.

---

## Roles

| Role | Web landing page | Android landing page |
|---|---|---|
| `client` | Client dashboard | `MainActivity` |
| `staff` | Staff dashboard | `Staff_Page` |
| `admin` (manager) | Manager dashboard | `Manager_Page` |

The role is returned by the API at login and used to route the user and restrict access to controllers and screens.
