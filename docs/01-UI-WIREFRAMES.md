# Pet Clinic - UI Wireframes & Screen Mockups

> Production-ready wireframes for all major screens. Each wireframe shows
> desktop (1024px+), tablet (768px), and mobile (375px) layouts.

> Implementation delta (March 2026): `MyPetsPage` and `VetTodayAppointments` are now implemented,
> and vet inventory screens include incoming/delivered reorder sections in addition to these base wireframes.

---

## 1. LOGIN PAGE

### Desktop (Split-Screen Layout)
```
┌─────────────────────────────────────────────────────────────────────┐
│                                                                     │
│  ┌─────────────────────────┐  ┌──────────────────────────────────┐  │
│  │                         │  │                                  │  │
│  │   ▓▓▓  PET CLINIC      │  │      Sign in to your account     │
│  │                         │  │                                  │  │
│  │   Gradient Background   │  │  ┌──────────────────────────┐   │  │
│  │   (Cyan-500 → Teal-600) │  │  │  📧 Email address        │   │  │
│  │                         │  │  └──────────────────────────┘   │  │
│  │   🩺 Trusted by 500+   │  │                                  │  │
│  │      veterinary clinics │  │  ┌──────────────────────────┐   │  │
│  │                         │  │  │  🔒 Password         👁  │   │  │
│  │   📊 Manage patients,   │  │  └──────────────────────────┘   │  │
│  │      appointments &     │  │                                  │  │
│  │      records in one     │  │  ☐ Remember me   Forgot pass?   │  │
│  │      place              │  │                                  │  │
│  │                         │  │  ┌──────────────────────────┐   │  │
│  │   🔒 Secure, HIPAA-     │  │  │       SIGN IN            │   │  │
│  │      compliant data     │  │  └──────────────────────────┘   │  │
│  │      handling           │  │                                  │  │
│  │                         │  │  ─── or ───                      │  │
│  │                         │  │                                  │  │
│  │                         │  │  Don't have an account?          │  │
│  │                         │  │  → Create account                │  │
│  │                         │  │                                  │  │
│  └─────────────────────────┘  └──────────────────────────────────┘  │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Mobile (Stacked Layout)
```
┌──────────────────────┐
│                      │
│    🩺 PET CLINIC     │
│    ───────────────   │
│    Professional      │
│    Veterinary Care   │
│                      │
│  Sign in to your     │
│  account             │
│                      │
│  ┌────────────────┐  │
│  │ Email address  │  │
│  └────────────────┘  │
│                      │
│  ┌────────────────┐  │
│  │ Password    👁 │  │
│  └────────────────┘  │
│                      │
│  ┌────────────────┐  │
│  │    SIGN IN     │  │
│  └────────────────┘  │
│                      │
│  Don't have an       │
│  account? Register   │
│                      │
└──────────────────────┘
```

### Visual Specifications
```
Brand Panel (Left):
  Background: linear-gradient(135deg, #0ea5e9, #0d9488)
  Text: White, font-weight 600
  Icons: 24px, white, outline style
  Feature items: 16px with 24px spacing

Form Panel (Right):
  Background: White (#ffffff)
  Max-width: 420px centered
  Padding: 48px

Title: "Sign in to your account"
  Font: 28px, weight 600, slate-900
  Margin-bottom: 32px

Input Fields:
  Height: 48px
  Border: 1px solid slate-300
  Border-radius: 8px
  Padding: 12px 16px
  Font: 16px
  Icon: left-aligned, 20px, slate-400
  Focus: border cyan-500, ring 3px cyan-100

Submit Button:
  Height: 48px
  Background: cyan-500
  Hover: cyan-600
  Text: White, 16px, weight 600
  Border-radius: 8px
  Full width

Error State:
  Background: red-50
  Border: 1px solid red-300
  Border-radius: 8px
  Padding: 12px 16px
  Icon: XCircle, red-500, 20px
  Text: red-700, 14px
```

---

## 2. REGISTER PAGE

### Desktop
```
┌─────────────────────────────────────────────────────────────────────┐
│                                                                     │
│  ┌─────────────────────────┐  ┌──────────────────────────────────┐  │
│  │                         │  │                                  │  │
│  │   ▓▓▓  PET CLINIC      │  │      Create your account         │  │
│  │                         │  │                                  │  │
│  │   Gradient Background   │  │  ┌─────────────┐ ┌───────────┐  │  │
│  │   (same as login)       │  │  │ First name  │ │ Last name │  │  │
│  │                         │  │  └─────────────┘ └───────────┘  │  │
│  │                         │  │                                  │  │
│  │                         │  │  ┌──────────────────────────┐   │  │
│  │                         │  │  │  📧 Email address        │   │  │
│  │                         │  │  └──────────────────────────┘   │  │
│  │                         │  │                                  │  │
│  │                         │  │  ┌──────────────────────────┐   │  │
│  │                         │  │  │  🔒 Password         👁  │   │  │
│  │                         │  │  └──────────────────────────┘   │  │
│  │                         │  │                                  │  │
│  │                         │  │  ┌──────────────────────────┐   │  │
│  │                         │  │  │  🔒 Confirm password  👁  │   │  │
│  │                         │  │  └──────────────────────────┘   │  │
│  │                         │  │                                  │  │
│  │                         │  │  ┌──────────────────────────┐   │  │
│  │                         │  │  │     CREATE ACCOUNT       │   │  │
│  │                         │  │  └──────────────────────────┘   │  │
│  │                         │  │                                  │  │
│  │                         │  │  Already have an account?        │  │
│  │                         │  │  → Sign in                       │  │
│  └─────────────────────────┘  └──────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 3. OWNER DASHBOARD

### Desktop (1024px+)
```
┌──────┬──────────────────────────────────────────────────────────────┐
│      │  🔍 Search pets, appointments...   🌙  🔔(2)  (JD) john@  │
│  S   ├──────────────────────────────────────────────────────────────┤
│  I   │                                                              │
│  D   │  ┌──────────────────────────────────────────────────────┐   │
│  E   │  │  (JD)  Welcome back, John!                           │   │
│  B   │  │        john@example.com                              │   │
│  A   │  │        🐾 3 Pets  📅 2 Upcoming                     │   │
│  R   │  │                              [Book Appt] [Edit Prof] │   │
│      │  └──────────────────────────────────────────────────────┘   │
│  ──  │                                                              │
│  🏠  │  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐        │
│  📅  │  │  🐾           │ │  ✅           │ │  ⏳           │        │
│  🐾  │  │  3            │ │  12           │ │  2            │        │
│  📋  │  │  Total Pets   │ │  Completed   │ │  Pending     │        │
│  📜  │  └──────────────┘ └──────────────┘ └──────────────┘        │
│  🏥  │                                                              │
│  💳  │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  My Pets                               [+ Add Pet]   │   │
│  ──  │  │  ┌──────────┐ ┌──────────┐ ┌──────────┐             │   │
│      │  │  │ 🐕 Buddy │ │ 🐈 Luna  │ │ 🐕 Max   │             │   │
│      │  │  │ Dog      │ │ Cat      │ │ Dog      │             │   │
│      │  │  │ Labrador │ │ Persian  │ │ Golden   │             │   │
│      │  │  │ 3 years  │ │ 2 years  │ │ 5 years  │             │   │
│      │  │  │ [Book]   │ │ [Book]   │ │ [Book]   │             │   │
│      │  │  └──────────┘ └──────────┘ └──────────┘             │   │
│      │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
│      │  ┌──────────────────────────────────────────────────────┐   │
│  ──  │  │  Upcoming Appointments        [History] [+ Book New] │   │
│  🚪  │  │                                                      │   │
│      │  │  📅 Mar 20, 2026  09:00-09:30  Buddy   ● Pending    │   │
│      │  │                                         [View]       │   │
│      │  │  📅 Mar 22, 2026  14:00-14:30  Luna    ● Confirmed  │   │
│      │  │                                         [View]       │   │
│      │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
└──────┴──────────────────────────────────────────────────────────────┘
```

### Tablet (768px)
```
┌──────────────────────────────────────────────────┐
│  ☰   🔍 Search...            🌙  🔔(2)  (JD)   │
├──────────────────────────────────────────────────┤
│                                                   │
│  ┌────────────────────────────────────────────┐  │
│  │ (JD) Welcome back, John!                   │  │
│  │      john@example.com                      │  │
│  │      🐾 3 Pets  📅 2 Upcoming              │  │
│  │      [Book Appointment]  [Edit Profile]    │  │
│  └────────────────────────────────────────────┘  │
│                                                   │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐   │
│  │ 🐾 3       │ │ ✅ 12      │ │ ⏳ 2       │   │
│  │ Total Pets │ │ Completed  │ │ Pending    │   │
│  └────────────┘ └────────────┘ └────────────┘   │
│                                                   │
│  ┌────────────────────────────────────────────┐  │
│  │ My Pets                      [+ Add Pet]   │  │
│  │ ┌──────────────┐ ┌──────────────┐         │  │
│  │ │ 🐕 Buddy     │ │ 🐈 Luna      │         │  │
│  │ │ Dog/Labrador │ │ Cat/Persian  │         │  │
│  │ │ 3 years      │ │ 2 years      │         │  │
│  │ └──────────────┘ └──────────────┘         │  │
│  │ ┌──────────────┐                          │  │
│  │ │ 🐕 Max       │                          │  │
│  │ │ Dog/Golden   │                          │  │
│  │ │ 5 years      │                          │  │
│  │ └──────────────┘                          │  │
│  └────────────────────────────────────────────┘  │
│                                                   │
└──────────────────────────────────────────────────┘
```

### Mobile (375px)
```
┌────────────────────────┐
│ ☰  Pet Clinic    🔔 JD │
├────────────────────────┤
│                        │
│ Welcome back, John!    │
│ 🐾 3 Pets  📅 2 Next   │
│ [Book Appointment    ] │
│                        │
│ ┌────────┐ ┌────────┐ │
│ │ 🐾 3   │ │ ✅ 12  │ │
│ │ Pets   │ │ Done   │ │
│ └────────┘ └────────┘ │
│ ┌──────────────────┐  │
│ │ ⏳ 2 Pending      │  │
│ └──────────────────┘  │
│                        │
│ My Pets   [+ Add Pet]  │
│ ┌──────────────────┐  │
│ │ 🐕 Buddy         │  │
│ │ Dog · Labrador   │  │
│ │ 3 years  [Book]  │  │
│ └──────────────────┘  │
│ ┌──────────────────┐  │
│ │ 🐈 Luna          │  │
│ │ Cat · Persian    │  │
│ │ 2 years  [Book]  │  │
│ └──────────────────┘  │
│                        │
│ Upcoming Appointments  │
│ ┌──────────────────┐  │
│ │ 📅 Mar 20, 2026  │  │
│ │ Buddy · 9:00 AM  │  │
│ │ ● Pending [View] │  │
│ └──────────────────┘  │
│                        │
└────────────────────────┘
```

---

## 4. VET DASHBOARD

### Desktop (1024px+)
```
┌──────┬──────────────────────────────────────────────────────────────┐
│      │  🔍 Search...                     🌙  🔔(2)  (DS) dr.s@   │
│  S   ├──────────────────────────────────────────────────────────────┤
│  I   │                                                              │
│  D   │  ┌──────────────────────────────────────────────────────┐   │
│  E   │  │  (DS)  Good morning, Dr. Smith!                      │   │
│  B   │  │        General Practitioner                          │   │
│  A   │  │        📅 5 Today's Appt.  ✅ 2 Completed            │   │
│  R   │  │                                     [View Patients]  │   │
│      │  └──────────────────────────────────────────────────────┘   │
│  ──  │                                                              │
│  🏠  │  ⚠️ LOW STOCK ALERTS                                        │
│  📅  │  ┌──────────────────────────────────────────────────────┐   │
│  💊  │  │  🧪 Rabies Vaccine       5 doses left   [Reorder]   │   │
│  🐾  │  │  💊 Amoxicillin          2 bottles left  [Reorder]   │   │
│      │  └──────────────────────────────────────────────────────┘   │
│  ──  │                                                              │
│  🚪  │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  Today's Appointments                     [Refresh]  │   │
│      │  │                                                      │   │
│      │  │  TIME      PET          OWNER       STATUS   ACTION  │   │
│      │  │  ─────────────────────────────────────────────────── │   │
│      │  │  09:00 AM  Buddy (🐕)  John Doe    ● Pending [Start]│   │
│      │  │  10:00 AM  Luna (🐈)   Jane Smith  ● Pending [Start]│   │
│      │  │  11:30 AM  Max (🐕)    Bob Lee     ✅ Done   [View] │   │
│      │  │  02:00 PM  Whiskers    Sarah K.    ● Pending [Start]│   │
│      │  │  03:30 PM  Rocky (🐕)  Mike T.     ● Pending [Start]│   │
│      │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
│      │  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐        │
│      │  │  📅 5         │ │  ✅ 2         │ │  ⏳ 3         │        │
│      │  │  Today's Appt │ │  Completed   │ │  Pending     │        │
│      │  └──────────────┘ └──────────────┘ └──────────────┘        │
│      │                                                              │
└──────┴──────────────────────────────────────────────────────────────┘
```

---

## 5. BOOKING WIZARD

### Desktop - Step 1: Select Pet
```
┌──────┬──────────────────────────────────────────────────────────────┐
│  S   │  Book Appointment                                            │
│  I   │                                                              │
│  D   │  ┌──────────────────────────────────────────────────────┐   │
│  E   │  │  Step 1 ─── Step 2 ─── Step 3 ─── Step 4            │   │
│  B   │  │  ● Pet      ○ Date     ○ Time     ○ Confirm          │   │
│  A   │  └──────────────────────────────────────────────────────┘   │
│  R   │                                                              │
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  Select a Pet                                        │   │
│      │  │                                                      │   │
│      │  │  ┌─────────────────┐  ┌─────────────────┐           │   │
│      │  │  │ ✓ SELECTED      │  │                 │           │   │
│      │  │  │  🐕 Buddy       │  │  🐈 Luna        │           │   │
│      │  │  │  Dog · Labrador │  │  Cat · Persian  │           │   │
│      │  │  │  3 years old    │  │  2 years old    │           │   │
│      │  │  │  ═══════════════│  │                 │           │   │
│      │  │  │  cyan border    │  │  gray border    │           │   │
│      │  │  └─────────────────┘  └─────────────────┘           │   │
│      │  │                                                      │   │
│      │  │  ┌─────────────────┐                                │   │
│      │  │  │                 │                                │   │
│      │  │  │  🐕 Max         │                                │   │
│      │  │  │  Dog · Golden   │                                │   │
│      │  │  │  5 years old    │                                │   │
│      │  │  │                 │                                │   │
│      │  │  └─────────────────┘                                │   │
│      │  │                                                      │   │
│      │  │                         [Cancel]  [Next: Select Date]│   │
│      │  └──────────────────────────────────────────────────────┘   │
└──────┴──────────────────────────────────────────────────────────────┘
```

### Desktop - Step 2: Select Date
```
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  Select Date                                         │   │
│      │  │                                                      │   │
│      │  │  ┌──────────────────────────────────────┐            │   │
│      │  │  │  ◀  March 2026  ▶                    │            │   │
│      │  │  │  Mon  Tue  Wed  Thu  Fri  Sat  Sun   │            │   │
│      │  │  │   2    3    4    5    6    7    8     │            │   │
│      │  │  │   9   10   11   12   13   14   15    │            │   │
│      │  │  │  16  [17]  18   19   20   21   22    │            │   │
│      │  │  │  23   24   25   26   27   28   29    │            │   │
│      │  │  │  30   31                              │            │   │
│      │  │  └──────────────────────────────────────┘            │   │
│      │  │                                                      │   │
│      │  │  Selected: Tuesday, March 17, 2026                   │   │
│      │  │                                                      │   │
│      │  │             [◀ Previous]  [Next: Select Time ▶]      │   │
│      │  └──────────────────────────────────────────────────────┘   │
```

### Desktop - Step 3: Select Time
```
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  Select Time Slot                                    │   │
│      │  │                                                      │   │
│      │  │  Morning                                             │   │
│      │  │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐     │   │
│      │  │  │ 8:00 │ │ 8:30 │ │[9:00]│ │ 9:30 │ │10:00 │     │   │
│      │  │  │  AM  │ │  AM  │ │  AM  │ │  AM  │ │  AM  │     │   │
│      │  │  └──────┘ └──────┘ └══════┘ └──────┘ └──────┘     │   │
│      │  │  ┌──────┐ ┌──────┐ ┌──────┐                        │   │
│      │  │  │10:30 │ │11:00 │ │11:30 │         (selected:     │   │
│      │  │  │  AM  │ │  AM  │ │  AM  │          cyan bg)      │   │
│      │  │  └──────┘ └──────┘ └──────┘                        │   │
│      │  │                                                      │   │
│      │  │  Afternoon                                           │   │
│      │  │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐     │   │
│      │  │  │ 1:00 │ │ 1:30 │ │ 2:00 │ │ 2:30 │ │ 3:00 │     │   │
│      │  │  │  PM  │ │  PM  │ │  PM  │ │  PM  │ │  PM  │     │   │
│      │  │  └──────┘ └──────┘ └──────┘ └──────┘ └──────┘     │   │
│      │  │                                                      │   │
│      │  │             [◀ Previous]  [Next: Review ▶]           │   │
│      │  └──────────────────────────────────────────────────────┘   │
```

### Desktop - Step 4: Confirm
```
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  Review & Confirm                                    │   │
│      │  │                                                      │   │
│      │  │  ┌────────────────────────────────────────────────┐  │   │
│      │  │  │  📋 Appointment Summary                        │  │   │
│      │  │  │                                                │  │   │
│      │  │  │  Pet:      🐕 Buddy (Dog, Labrador)            │  │   │
│      │  │  │  Date:     Tuesday, March 17, 2026             │  │   │
│      │  │  │  Time:     9:00 AM - 9:30 AM                   │  │   │
│      │  │  │  Duration: 30 minutes                          │  │   │
│      │  │  └────────────────────────────────────────────────┘  │   │
│      │  │                                                      │   │
│      │  │  Additional Notes (optional)                         │   │
│      │  │  ┌────────────────────────────────────────────────┐  │   │
│      │  │  │ Annual checkup and vaccinations...             │  │   │
│      │  │  │                                                │  │   │
│      │  │  └────────────────────────────────────────────────┘  │   │
│      │  │                                                      │   │
│      │  │             [◀ Previous]  [✓ Confirm Booking]        │   │
│      │  └──────────────────────────────────────────────────────┘   │
```

### Booking Success State
```
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │                                                      │   │
│      │  │              ✅                                       │   │
│      │  │                                                      │   │
│      │  │       Appointment Booked!                            │   │
│      │  │                                                      │   │
│      │  │  Your appointment has been scheduled.                │   │
│      │  │                                                      │   │
│      │  │  Confirmation #: APT-2026-0317-001                   │   │
│      │  │                                                      │   │
│      │  │  🐕 Buddy · March 17, 2026 · 9:00 AM                │   │
│      │  │                                                      │   │
│      │  │      [Back to Dashboard]  [Book Another]             │   │
│      │  │                                                      │   │
│      │  └──────────────────────────────────────────────────────┘   │
```

---

## 6. VISIT DETAILS PAGE

### Desktop (Two-Column Layout)
```
┌──────┬──────────────────────────────────────────────────────────────┐
│  S   │  ← Dashboard / Appointments / Visit #APT-001                │
│  I   │                                                              │
│  D   │  ┌───────────────────────────┬──────────────────────────┐   │
│  E   │  │  VISIT INFORMATION        │  PET INFORMATION         │   │
│  B   │  │                           │                          │   │
│  A   │  │  Status: ● In Progress    │  🐕 Buddy                │   │
│  R   │  │  Date:   Mar 17, 2026     │  Dog · Labrador          │   │
│      │  │  Time:   9:00 - 9:30 AM   │  3 years old             │   │
│      │  │                           │  Owner: John Doe          │   │
│      │  │  ──── Visit Notes ────    │                          │   │
│      │  │  ┌─────────────────────┐  │  ──── Vet Info ────     │   │
│      │  │  │ Annual checkup.     │  │  Dr. Smith                │   │
│      │  │  │ Weight: 32kg.       │  │  General Practitioner    │   │
│      │  │  │ Vaccination due.    │  │  License: VET-001        │   │
│      │  │  │                     │  │                          │   │
│      │  │  └─────────────────────┘  │  ──── Allergies ────    │   │
│      │  │  [Edit Notes]             │  ⚠ Penicillin            │   │
│      │  │                           │                          │   │
│      │  │  ──── Prescriptions ────  │  ──── Timeline ────     │   │
│      │  │                           │  09:00 Checked in       │   │
│      │  │  ┌────────────────────┐   │  09:05 Exam started     │   │
│      │  │  │ Medication  │ Dose │   │  09:15 Treatment done   │   │
│      │  │  │─────────────│──────│   │  09:25 Notes added      │   │
│      │  │  │ Amoxicillin │ 250mg│   │                          │   │
│      │  │  │ Metacam     │ 1.5ml│   │                          │   │
│      │  │  └────────────────────┘   │                          │   │
│      │  │  [+ Add Prescription]     │                          │   │
│      │  │                           │                          │   │
│      │  │  ──── Invoice ────        │                          │   │
│      │  │  Base Fee:     $50.00     │                          │   │
│      │  │  Medications:  $35.00     │                          │   │
│      │  │  ─────────────────────    │                          │   │
│      │  │  Total:        $85.00     │                          │   │
│      │  │                           │                          │   │
│      │  │  [Complete Visit]         │                          │   │
│      │  └───────────────────────────┴──────────────────────────┘   │
└──────┴──────────────────────────────────────────────────────────────┘
```

---

## 7. HEALTH RECORDS PAGE

### Desktop
```
┌──────┬──────────────────────────────────────────────────────────────┐
│  S   │  Health Records                                              │
│  I   │                                                              │
│  D   │  ┌──────────────────────────────────────────────────────┐   │
│  E   │  │  Select Pet:                                         │   │
│  B   │  │  ┌═══════════┐ ┌───────────┐ ┌───────────┐         │   │
│  A   │  │  │ 🐕 Buddy  │ │ 🐈 Luna   │ │ 🐕 Max    │         │   │
│  R   │  │  │ (selected)│ │           │ │           │         │   │
│      │  │  └═══════════┘ └───────────┘ └───────────┘         │   │
│      │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  Vaccinations                        [+ Add Record]  │   │
│      │  │                                                      │   │
│      │  │  VACCINE          DATE        NEXT DUE    STATUS     │   │
│      │  │  ──────────────────────────────────────────────────  │   │
│      │  │  Rabies           Jan 2026    Jan 2027    ● Current  │   │
│      │  │  DHPP             Mar 2025    Mar 2026    ⚠ Due Soon │   │
│      │  │  Bordetella       Sep 2025    Sep 2026    ● Current  │   │
│      │  │  Leptospirosis    Jun 2025    Jun 2026    ● Current  │   │
│      │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
│      │  ┌─────────────────────────┐ ┌─────────────────────────┐   │
│      │  │  Known Allergies         │ │  Medical Notes          │   │
│      │  │                         │ │                         │   │
│      │  │  ⚠ Penicillin           │ │  Weight: 32kg           │   │
│      │  │  ⚠ Chicken (mild)       │ │  Heart murmur: grade 1  │   │
│      │  │                         │ │  Last dental: Jan 2026  │   │
│      │  └─────────────────────────┘ └─────────────────────────┘   │
└──────┴──────────────────────────────────────────────────────────────┘
```

---

## 8. INVOICES PAGE

### Desktop
```
┌──────┬──────────────────────────────────────────────────────────────┐
│  S   │  Invoices                                                    │
│  I   │                                                              │
│  D   │  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐        │
│  E   │  │  💰 $285.00   │ │  ✅ $185.00   │ │  ⚠ $100.00   │        │
│  B   │  │  Total Due    │ │  Paid (Month)│ │  Overdue     │        │
│  A   │  └──────────────┘ └──────────────┘ └──────────────┘        │
│  R   │                                                              │
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  Filter: [All] [Pending] [Paid] [Overdue]            │   │
│      │  │                                                      │   │
│      │  │  INVOICE #    DATE        AMOUNT   STATUS    ACTIONS │   │
│      │  │  ──────────────────────────────────────────────────  │   │
│      │  │  INV-001      Mar 17      $85.00   ● Pending [Pay]  │   │
│      │  │  INV-002      Mar 10      $120.00  ✅ Paid   [📥]   │   │
│      │  │  INV-003      Feb 28      $200.00  🔴 Overdue [Pay] │   │
│      │  │  INV-004      Feb 15      $65.00   ✅ Paid   [📥]   │   │
│      │  │                                                      │   │
│      │  └──────────────────────────────────────────────────────┘   │
└──────┴──────────────────────────────────────────────────────────────┘
```

---

## 9. INVENTORY PAGE (Vet)

### Desktop
```
┌──────┬──────────────────────────────────────────────────────────────┐
│  S   │  Inventory Management                                        │
│  I   │                                                              │
│  D   │  ⚠ LOW STOCK ALERTS (2)                                     │
│  E   │  ┌──────────────────────────────────────────────────────┐   │
│  B   │  │  🧪 Rabies Vaccine  ██░░░░░░░░  5/100  [Reorder]    │   │
│  A   │  │  💊 Amoxicillin     █░░░░░░░░░  2/50   [Reorder]    │   │
│  R   │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  All Inventory          🔍 Search   [+ Add Item]     │   │
│      │  │                                                      │   │
│      │  │  ITEM             CATEGORY    QTY    REORDER  STATUS │   │
│      │  │  ──────────────────────────────────────────────────  │   │
│      │  │  Aspirin          Medication  100    20       ● OK   │   │
│      │  │  Ibuprofen        Medication   50    15       ● OK   │   │
│      │  │  Rabies Vaccine   Vaccine       5    20       🔴 Low │   │
│      │  │  Amoxicillin      Antibiotic    2    10       🔴 Low │   │
│      │  │  Bandages (Lg)    Supplies    200    50       ● OK   │   │
│      │  │  Syringes 5ml     Supplies    150    30       ● OK   │   │
│      │  └──────────────────────────────────────────────────────┘   │
└──────┴──────────────────────────────────────────────────────────────┘
```

---

## 10. PATIENTS PAGE (Vet)

### Desktop
```
┌──────┬──────────────────────────────────────────────────────────────┐
│  S   │  Patients                                                    │
│  I   │                                                              │
│  D   │  ┌──────────────────────────────────────────────────────┐   │
│  E   │  │  🔍 Search by name or owner...                       │   │
│  B   │  │  Species: [All] [🐕 Dogs] [🐈 Cats] [🐦 Birds]      │   │
│  A   │  └──────────────────────────────────────────────────────┘   │
│  R   │                                                              │
│      │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│      │  │ 🐕 Buddy │ │ 🐈 Luna  │ │ 🐕 Max   │ │ 🐦 Tweety│      │
│      │  │ Dog      │ │ Cat      │ │ Dog      │ │ Bird     │      │
│      │  │ Labrador │ │ Persian  │ │ Golden   │ │ Canary   │      │
│      │  │          │ │          │ │          │ │          │      │
│      │  │ Owner:   │ │ Owner:   │ │ Owner:   │ │ Owner:   │      │
│      │  │ John D.  │ │ Jane S.  │ │ Bob L.   │ │ Sarah K. │      │
│      │  │          │ │          │ │          │ │          │      │
│      │  │ Last:    │ │ Last:    │ │ Last:    │ │ Last:    │      │
│      │  │ Mar 10   │ │ Mar 5    │ │ Feb 28   │ │ Jan 15   │      │
│      │  │          │ │          │ │          │ │          │      │
│      │  │ [View]   │ │ [View]   │ │ [View]   │ │ [View]   │      │
│      │  │ [Hist.]  │ │ [Hist.]  │ │ [Hist.]  │ │ [Hist.]  │      │
│      │  └──────────┘ └──────────┘ └──────────┘ └──────────┘      │
│      │                                                              │
└──────┴──────────────────────────────────────────────────────────────┘
```

---

## 11. VISIT HISTORY PAGE (Owner)

### Desktop
```
┌──────┬──────────────────────────────────────────────────────────────┐
│  S   │  Visit History                                               │
│  I   │                                                              │
│  D   │  ┌──────────────────────────────────────────────────────┐   │
│  E   │  │  Filters:                                            │   │
│  B   │  │  Pet: [All Pets ▾]    From: [____]    To: [____]     │   │
│  A   │  │                            [Apply]    [Clear]        │   │
│  R   │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  📅 March 17, 2026                                   │   │
│      │  │  🐕 Buddy · Annual Checkup                           │   │
│      │  │  Dr. Smith · ✅ Completed                             │   │
│      │  │  Diagnosis: Healthy                                  │   │
│      │  │  Treatments: Rabies vaccine, DHPP booster            │   │
│      │  │  Invoice: $85.00 ● Pending                [View ▶]  │   │
│      │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
│      │  ┌──────────────────────────────────────────────────────┐   │
│      │  │  📅 March 5, 2026                                    │   │
│      │  │  🐈 Luna · Dental Cleaning                           │   │
│      │  │  Dr. Johnson · ✅ Completed                           │   │
│      │  │  Diagnosis: Mild tartar buildup                      │   │
│      │  │  Treatments: Dental scaling, fluoride treatment      │   │
│      │  │  Invoice: $120.00 ✅ Paid                  [View ▶]  │   │
│      │  └──────────────────────────────────────────────────────┘   │
│      │                                                              │
└──────┴──────────────────────────────────────────────────────────────┘
```

---

## 12. MODAL PATTERNS

### Add Pet Modal
```
┌──────────────────────────────────────────────┐
│  Add New Pet                            [✕]  │
├──────────────────────────────────────────────┤
│                                              │
│  Pet Name *                                  │
│  ┌──────────────────────────────────────┐   │
│  │ e.g., Buddy                         │   │
│  └──────────────────────────────────────┘   │
│                                              │
│  ┌──────────────────┐ ┌─────────────────┐   │
│  │ Species *     ▾  │ │ Breed *         │   │
│  │ Select species   │ │ e.g., Labrador  │   │
│  └──────────────────┘ └─────────────────┘   │
│                                              │
│  Date of Birth *                             │
│  ┌──────────────────────────────────────┐   │
│  │ YYYY-MM-DD                          │   │
│  └──────────────────────────────────────┘   │
│                                              │
├──────────────────────────────────────────────┤
│                       [Cancel]  [Add Pet]    │
└──────────────────────────────────────────────┘
```

### Delete Confirmation Modal
```
┌──────────────────────────────────────────────┐
│  Delete Pet                             [✕]  │
├──────────────────────────────────────────────┤
│                                              │
│  ⚠  Are you sure you want to delete         │
│     Buddy? This action cannot be undone.     │
│                                              │
├──────────────────────────────────────────────┤
│                       [Cancel]  [Delete]     │
└──────────────────────────────────────────────┘
```

---

## 13. SIDEBAR NAVIGATION

### Owner Sidebar
```
┌────────────────────────┐
│  🩺 Pet Clinic          │
├────────────────────────┤
│  (JD)  John Doe        │
│        Pet Owner       │
├────────────────────────┤
│                        │
│  ■ 🏠 Dashboard        │ ← active (cyan indicator)
│    📅 Book Appointment │
│    🐾 My Pets          │
│    📋 Appointments     │
│    📜 Visit History    │
│    🏥 Health Records   │
│    💳 Invoices         │
│                        │
├────────────────────────┤
│  🚪 Logout             │
└────────────────────────┘
```

### Vet Sidebar
```
┌────────────────────────┐
│  🩺 Pet Clinic          │
├────────────────────────┤
│  (DS)  Dr. Smith       │
│        Veterinarian    │
├────────────────────────┤
│                        │
│  ■ 🏥 Dashboard        │ ← active
│    📅 Today's Schedule │
│    💊 Inventory        │
│    🐾 Patients         │
│                        │
├────────────────────────┤
│  🚪 Logout             │
└────────────────────────┘
```

---

## 14. COMPONENT STATES

### Button States
```
┌──────────────────────────────────────────────────────────┐
│  Primary:                                                 │
│  [  Save Changes  ] → [  Save Changes  ] → [Saving...]   │
│   default(cyan-500)    hover(cyan-600)     loading        │
│                                                           │
│  Secondary:                                               │
│  [  Cancel  ]      → [  Cancel  ]                         │
│   default(outline)    hover(slate-50 bg)                  │
│                                                           │
│  Danger:                                                  │
│  [  Delete  ]      → [  Delete  ]      → [Deleting...]   │
│   default(red-600)    hover(red-700)     loading          │
│                                                           │
│  Disabled:                                                │
│  [  Save Changes  ]                                       │
│   disabled(slate-200, 50% opacity)                        │
└──────────────────────────────────────────────────────────┘
```

### Input States
```
┌──────────────────────────────────────────────────────────┐
│  Default:                                                 │
│  Label                                                    │
│  ┌─────────────────────────────┐                         │
│  │ Placeholder text            │  border: slate-300      │
│  └─────────────────────────────┘                         │
│                                                           │
│  Focused:                                                 │
│  Label                                                    │
│  ┌═════════════════════════════┐                         │
│  │ User input text             │  border: cyan-500       │
│  └═════════════════════════════┘  ring: cyan-100         │
│                                                           │
│  Error:                                                   │
│  Label                                                    │
│  ┌─────────────────────────────┐                         │
│  │ Invalid input               │  border: red-500        │
│  └─────────────────────────────┘  bg: red-50             │
│  ⚠ This field is required               text: red-600   │
│                                                           │
│  Disabled:                                                │
│  Label                                                    │
│  ┌─────────────────────────────┐                         │
│  │ Disabled text               │  bg: slate-100          │
│  └─────────────────────────────┘  text: slate-400        │
└──────────────────────────────────────────────────────────┘
```

### Badge Variants
```
┌──────────────────────────────────────────────────────────┐
│                                                           │
│  (● Pending)  (✅ Completed)  (🔴 Overdue)  (ℹ Info)     │
│   yellow bg    green bg        red bg        blue bg     │
│   yellow text  green text      red text      blue text   │
│                                                           │
│  Sizes:                                                   │
│  (sm)  (medium/default)  (LARGE)                          │
│   10px    12px              14px                          │
│                                                           │
└──────────────────────────────────────────────────────────┘
```

### Card Variants
```
┌──────────────────────────────────────────────────────────┐
│  Default Card:                                            │
│  ┌────────────────────────────────────────────┐          │
│  │  Header                                    │ border-b │
│  │──────────────────────────────────────────  │          │
│  │  Body content goes here                    │          │
│  │──────────────────────────────────────────  │          │
│  │  Footer                                    │ bg:50    │
│  └────────────────────────────────────────────┘          │
│  border: slate-200, radius: 12px, shadow: level 2        │
│                                                           │
│  Interactive Card (hover):                                │
│  ┌════════════════════════════════════════════┐          │
│  │  Content with hover shadow (level 3)       │          │
│  │  border: slate-300                         │          │
│  └════════════════════════════════════════════┘          │
│                                                           │
│  Stat Card:                                               │
│  ┌────────────────────────────────────────────┐          │
│  │  🐾  3                                     │          │
│  │  Total Pets                                │          │
│  └────────────────────────────────────────────┘          │
│  gradient bg, no header/footer                            │
└──────────────────────────────────────────────────────────┘
```

---

## 15. TOAST NOTIFICATIONS

```
Success:
┌──────────────────────────────────────┐
│ ▌ ✅ Pet added successfully       ✕  │
│ ▌    Buddy has been added to your    │
│ ▌    pet list.                       │
└──────────────────────────────────────┘
  green left-border, green-50 bg

Error:
┌──────────────────────────────────────┐
│ ▌ ❌ Failed to save                ✕  │
│ ▌    Please try again or contact     │
│ ▌    support.                        │
└──────────────────────────────────────┘
  red left-border, red-50 bg

Warning:
┌──────────────────────────────────────┐
│ ▌ ⚠ Session expiring soon         ✕  │
│ ▌    Your session will expire in     │
│ ▌    5 minutes.                      │
└──────────────────────────────────────┘
  orange left-border, orange-50 bg
```

---

## 16. LOADING STATES

### Skeleton Loading (Cards)
```
┌──────────────────────────────────────────────────────────┐
│  ┌──────────┐ ┌──────────┐ ┌──────────┐                 │
│  │ ████████ │ │ ████████ │ │ ████████ │  pulse animation │
│  │ ██████   │ │ ██████   │ │ ██████   │                 │
│  │ ████     │ │ ████     │ │ ████     │                 │
│  │          │ │          │ │          │                 │
│  │ ██████   │ │ ██████   │ │ ██████   │                 │
│  └──────────┘ └──────────┘ └──────────┘                 │
└──────────────────────────────────────────────────────────┘
```

### Skeleton Loading (Table Rows)
```
┌──────────────────────────────────────────────────────────┐
│  ████████  ██████████  ████████  ██████  ████            │
│  ────                                         shimmer   │
│  ████████  ██████████  ████████  ██████  ████            │
│  ────                                                    │
│  ████████  ██████████  ████████  ██████  ████            │
└──────────────────────────────────────────────────────────┘
```

### Spinner Loading
```
┌──────────────────────────────────────────────────────────┐
│                                                           │
│                      ◠ ◡ ◠                                │
│                                                           │
│                  Loading...                                │
│                                                           │
└──────────────────────────────────────────────────────────┘
```

### Empty State
```
┌──────────────────────────────────────────────────────────┐
│                                                           │
│                    🐾                                      │
│                                                           │
│              No pets found                                │
│    Add your first pet to get started.                     │
│                                                           │
│              [+ Add Your First Pet]                       │
│                                                           │
└──────────────────────────────────────────────────────────┘
```

---

*End of Wireframes Document*
