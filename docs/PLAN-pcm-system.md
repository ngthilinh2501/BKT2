# PLAN-pcm-system.md - Pickleball Club Management (PCM)

> **Student ID (MSSV):** 357
> **Project:** "Vợt Thủ Phố Núi" Database & Management System
> **Tech Stack:** ASP.NET Core Razor Pages, EF Core, Identity, SQL Server

---

## 📅 Roadmap Overview

| Phase | Focus | Description | Est. Complexity |
| :--- | :--- | :--- | :--- |
| **P1** | **Foundation** | Project setup, Database Schema (MSSV prefixed), Identity, Seeding. | 🟡 Medium |
| **P2** | **Operations** | Admin features: Members, Courts, Treasury, News. | 🟢 Easy |
| **P3** | **Sports Core** | Booking logic, Match recording, Ranking math. | 🔴 Hard |
| **P4** | **Events** | Challenges (Duel/MiniGame), Team Battle logic. | 🟠 Medium |
| **P5** | **UI & Polish** | Dashboard, Validation, Role security, Bonus features. | 🟢 Easy |

---

## ✅ Phase 1: Foundation & Database (The "Skeleton")

**Goal:** Provide a runnable application with a populated database containing all required entities.

- [ ] **1.1. Project Init**
    - Create ASP.NET Core Web App (Razor Pages).
    - Install NuGet: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.
- [ ] **1.2. Database Entities (Prefix: `357_`)**
    - `357_Members`: Link to IdentityUser, RankLevel, Info.
    - `357_Courts`: Id, Name, IsActive.
    - `357_News`: Title, Content, IsPinned.
    - `357_TransactionCategories`: Name, Type (Thu/Chi).
    - `357_Transactions`: Amount, Description, Date, FK Category.
    - `357_Bookings`: CourtId, StartTime, EndTime, MemberId.
    - `357_Challenges`: Type (Duel/Mini), GameMode, PrizePool.
    - `357_Participants`: FK Challenge, FK Member, Team (A/B).
    - `357_Matches`: Date, IsRanked, Format (1v1/2v2), FK Players, Result.
- [ ] **1.3. DbContext force**
    - Configure Identity.
    - Configure Relationships (Fluent API if needed).
- [ ] **1.4. Data Seeding (CRITICAL)**
    - Implement `DbInitializer`.
    - Seed **1 Admin Account** (`admin@pcm.com`).
    - Seed **6-8 Members**.
    - Seed **2 Courts**.
    - Seed Financial Data (Transactions) so Fund > 0.
    - Seed **1 Ongoing TeamBattle Challenge** with participants & history matches.
- [ ] **1.5. Migration**
    - `Add-Migration InitialCreate`
    - `Update-Database`

---

## 🛠 Phase 2: Internal Operations (Admin)

**Goal:** Enable Admin and Treasurer to manage the club's resources.

- [ ] **2.1. Member Management**
    - View List (Pagination).
    - Update Info (Phone, DOB). *Note: Rank is read-only.*
- [ ] **2.2. Court Management**
    - CRUD Courts (Add/Edit/Disable).
- [ ] **2.3. Treasury (Money)**
    - Manage Categories (Add types like "Tiền sân", "Nước").
    - Record Transaction (Input: Category, Amount, Date).
    - **Logic:** Calculate Total Fund.
    - **UI:** Show 🛑 RED WARNING if Fund < 0.
- [ ] **2.4. News**
    - Create News (Input: Title, Content, Checkbox IsPinned).
    - Pin/Unpin logic.

---

## 🏸 Phase 3: Sports Core (The "Flesh")

**Goal:** Member booking and Match result recording (Referee).

- [ ] **3.1. Booking System**
    - **UI:** Form to pick Court, Date, Time (Start-End).
    - **Logic (Validation):**
        - StartTime >= Now.
        - EndTime > StartTime.
        - **Overlap Check:** `!ExistingBookings.Any(b => b.CourtId == Id && b.Start < New.End && b.End > New.Start)`
- [ ] **3.2. Match Recording (Referee Role)**
    - **UI:**
        - Select Format (Singles/Doubles).
        - Select Players (Dynamic Dropdowns: 2 for Singles, 4 for Doubles).
        - Validation: Player cannot be in both teams.
        - Checkbox `IsRanked`.
        - Dropdown `Challenge` (Optional).
    - **Logic:** Save Match execution.
- [ ] **3.3. Ranking System (Service)**
    - If `IsRanked == true`:
        - Update `357_Members.RankLevel`.
        - Formula: Simple (+/- 0.1) or Bonus (Elo).

---

## 🏆 Phase 4: Challenges & Events

**Goal:** Manage Mini-games and Team Battles.

- [ ] **4.1. Challenge Management**
    - Create Challenge (EntryFee, Mode).
    - Add Participants (Assign Team A/B).
- [ ] **4.2. Team Battle Logic**
    - When a Match is saved with `ChallengeId`:
        - Detect Winning Team (A or B).
        - Update `CurrentScore_TeamA` or `B`.
        - Check `Config_TargetWins`. If reached -> `Status = Finished`.

---

## 🎨 Phase 5: UI/UX & Finishing Touches

**Goal:** Polished interface conforming to "Vợt Thủ Phố Núi" theme.

- [ ] **5.1. Dashboard (Home)**
    - Top 5 Ranking Widget.
    - Pinned News.
    - Fund Status (Hidden for normal members).
- [ ] **5.2. My Area**
    - My Profile (View Rank, Matches).
    - My Bookings.
- [ ] **5.3. Authorization**
    - `[Authorize(Roles = "Admin")]` for Ops.
    - `[Authorize(Roles = "Referee")]` for Match Recording.
- [ ] **5.4. Bonus Checking**
    - Verify Age (<10 or >80 warning).
    - Export Reports (if time permits).

---

## 📝 Agent Assignments

| Agent | Responsibilities |
| :--- | :--- |
| **Backend-Specialist** | Entity creation, DbContext, Migrations, Seeding Logic, Ranking Service. |
| **Frontend-Specialist** | Razor Pages, Bootstrap UI, Responsive Design, Dashboard Widgets, Booking Calendar UI. |
| **Security-Auditor** | Identity Setup, Role Configuration (Admin/Referee/Member), Policy implementation. |
