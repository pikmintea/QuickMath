# QuickMath — Roadmap v1

A Duolingo-style math practice app. Native app first (Android via Play Store), built with .NET MAUI Blazor Hybrid, C# , $0 budget.

## Stack decisions (locked in)

- **Framework:** .NET MAUI Blazor Hybrid (`maui-blazor-web` template — includes `QuickMath.Shared`, `QuickMath.Web`, `QuickMath.Web.Client` for a possible future web version, currently unused)
- **UI:** Razor (`.razor`), no XAML
- **Primary dev/test platform:** Windows (fastest iteration loop)
- **Primary ship target:** Android (Play Store — account already exists)
- **Backend:** Firebase (free tier) — Authentication + Firestore
- **Local storage (guest / offline):** `Microsoft.Maui.Storage.Preferences`
- **License:** AGPL-3.0
- **Repo:** public — `pikmintea/QuickMath` (old vibe-coded web version deleted, archived separately as reference only, not reused as code)

## Project structure

- **`QuickMath` (native)** — anything touching platform-specific APIs: `Preferences`, `WebAuthenticator`, Firebase SDKs, file storage.
- **`QuickMath.Shared`** — pure UI/Razor components only. When a page needs a platform API (e.g. `Login.razor`), define an **interface** in `.Shared` and the real implementation in `QuickMath`, registered via DI (`AddSingleton<IThing, Thing>()`). This pattern is already used for `IAccountManagerService` / `AccountManagerService` and `IGoogleAuthService` / `GoogleAuthService` — reuse it for anything similar going forward.

## AI usage policy (self-imposed, org-wide)

- No agent mode, no auto-apply, no autonomous file edits.
- One specific ask at a time. Extra suggestions get discarded, not folded in "while we're at it."
- All AI-suggested changes reviewed and applied manually, one at a time.
- Only sole help by chatbot can be tolerated but not like using them to gen full code.

## Roadmap phases

### Phase 1 — Core loop (in progress)

- [x] Guest / sign-in decision screen
- [ ] **Fix Android guest + Google sign-in (debug session needed)**
- [x] **Fix/build Windows Google sign-in (Desktop OAuth client + PKCE flow)**
- [ ] Resolve `/practice` vs `/play` route naming — pick one, use everywhere
- [ ] First real exercise screen: one exercise type (e.g. multiplication tables), question generation, answer input, instant right/wrong feedback
- [ ] Local streak counter (`StreakService`, using `Preferences`, following the guest/signed-in split pattern)

### Phase 2 — Make it feel like Duolingo

- [ ] Multiple exercise types / skill tree (addition, fractions, algebra, etc.)
- [ ] XP, levels, streak visuals (progress bar, animations) — extend the playful visual style already established
- [ ] Difficulty progression / unlock gating

### Phase 3 — Identity & sync

- [ ] Finish Google Sign-In on both Android and Windows
- [ ] Exchange Google ID token for a Firebase session (`signInWithIdp` REST endpoint)
- [ ] Sync local progress to Firestore once signed in
- [ ] `AccountManagerService` / `StreakService` decide storage target based on `AccountState` (guest → `Preferences` only; signed in → Firestore, possibly cached locally too)

### Phase 4 — Leaderboard

- [ ] Firestore collection: user, score, streak
- [ ] Leaderboard screen, sorted, maybe weekly reset
- [ ] Firestore security rules gated on `request.auth.uid`

### Phase 5 — Ship it

- [ ] App icon, screenshots, store listing
- [ ] Play Store submission using existing developer account
- [ ] Confirm AGPL license terms are compatible with the store listing

## Notes

- Guests keep full local progress and offline play — only cross-device sync and the leaderboard are gated behind sign-in. Don't remove features guests already have.
