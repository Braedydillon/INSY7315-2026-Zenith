# Bridge & Anchor Loans

A loan management platform for **Bridge & Anchor Loans**. It has both applications, Android and MVC that share one REST API:

---

## Website Link
https://bridgeanchor-latest.onrender.com/
https://apiinsy7315-latest.onrender.com/index.html

### Clients
- Register and log in
- Submit a full loan application (personal, employment, marital status, spouse and divorce details, loan purpose)
- Track the status of submitted applications
- Read the company policy and NCR compliance information
- Contact page and WhatsApp shortcut
- Built-in FAQ chatbot covering loan amounts, required documents, how to apply, branch locations and contact details in Android

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

## Android app

1. Open the `Android` folder in Android Studio.
2. Let Gradle sync finish.
3. Select an emulator or connected device.
4. Click **Run**.

The app needs an internet connection because it calls the hosted API.

## API

Both clients use the same base URL, set in `RetrofitClient.kt` (Android) and `Program.cs` (Web):

```

Endpoints used by the apps:

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/Auth/register | Register a new client |
| POST | /api/Auth/login | Log in, returns token and role |
| POST | /api/Auth/refresh | Refresh a session token |
| GET | /api/Me | Current user profile |
| POST | /api/LoansApi | Submit a loan application |
| GET | /api/LoansApi | All applications (staff / manager) |
| GET | /api/LoansApi/mine | The signed-in client's applications |
| GET | /api/LoansApi/pending | Pending applications |
| GET | /api/LoansApi/{id} | A single application |
| PUT | /api/LoansApi/{id}/status | Update application status |
| GET | /api/Admin/users | List users (admin) |
| POST | /api/Admin/set-role | Assign a role (admin) |

Authenticated requests send Authorization: Bearer <token>.

---

## Roles

| Role | Web landing page | Android landing page |
|---|---|---|
| client | Client dashboard | MainActivity |
| staff | Staff dashboard | Staff_Page |
| admin (manager) | Manager dashboard | Manager_Page |

The role is returned by the API at login and used to route the user and restrict access to controllers and screens.

## Login Details

### Android
Client : Kelz@gmail.com 
Password : kelly1108@

Staff : pg1@gnail.com
Password : Password@89 

Admin: pg@gmail.com
Password : Password@89 

### MVC
Client: red@gmail.com
Password: redblue

Staff: idk@gmail.com
Password: zxcvbnm

Admin: admin@bridgeandanchor.com
Password: AdminPassword123!
