# QuickMath — Checklist Roadmap

## Cleanup / idk

- [ ] Remove any leftover `Debug.WriteLine` calls that are no longer useful
- [ ] Add a proper `README.md` describing the project and setup steps
- [ ] Document the `Secrets.cs.example` → `Secrets.cs` setup step in the README

## Phase 1 — Core gameplay loop

- [ ] Design `Exercise` model (question text, correct answer, operands)
- [ ] Create `IExerciseGenerator` interface in `.Shared`
- [ ] Implement `AdditionExerciseGenerator` in native or shared (pure logic, no platform APIs needed)
- [ ] Decide number range for addition (e.g. 1–20 to start)
- [ ] Generate random addition problems with correct answer precomputed
- [ ] Build `/play` page skeleton matching prototype layout
- [ ] Wire question card to display a generated exercise
- [ ] Build answer input field (numeric only)
- [ ] Build "Check" button and click handler
- [ ] Implement correct-answer feedback state (visual)
- [ ] Implement incorrect-answer feedback state (visual)
- [ ] Auto-advance to next question after feedback (with short delay)
- [ ] Decide session length (e.g. 10 questions per round)
- [ ] Track current question index within a session
- [ ] Build progress bar reflecting session position
- [ ] Build end-of-session summary screen (score, accuracy)
- [ ] Add "Play again" / "Back to Home" actions on summary screen
- [ ] Create `IStreakService` interface in `.Shared`
- [ ] Implement `StreakService` in native project using `Preferences`
- [ ] Register `StreakService` in `MauiProgram.cs`
- [ ] Increment streak on session completion
- [ ] Reset streak logic if a day is missed (decide exact rule)
- [ ] Persist streak across app restarts
- [ ] Display real streak number on Home page (replace prototype's hardcoded "4")
- [ ] Handle empty/first-time state (streak = 0) gracefully on Home
- [ ] Add keyboard "Enter" key support for submitting an answer
- [ ] Add input validation (prevent non-numeric input)
- [ ] Add basic error handling if exercise generation fails

## Phase 2 — More skills + progression

- [ ] Create `SubtractionExerciseGenerator`
- [ ] Create `MultiplicationExerciseGenerator`
- [ ] Create `DivisionExerciseGenerator` (decide how to handle non-integer results)
- [ ] Add skill selector so `/play` can be parameterized by skill type
- [ ] Wire Home page's skill grid to real, clickable skill cards
- [ ] Implement locked/unlocked skill state logic
- [ ] Decide unlock criteria (e.g. complete addition to unlock subtraction)
- [ ] Persist unlocked-skills state
- [ ] Design XP model (points per correct answer)
- [ ] Create `IXpService` interface in `.Shared`
- [ ] Implement `XpService` in native project
- [ ] Award XP on correct answers
- [ ] Award bonus XP for streak milestones
- [ ] Persist XP across restarts
- [ ] Display real XP on Home page (replace hardcoded "120")
- [ ] Design level thresholds based on total XP
- [ ] Compute current level from XP
- [ ] Display real level on Home page (replace hardcoded "Lvl 3")
- [ ] Add difficulty tiers within each skill (e.g. easy/medium/hard number ranges)
- [ ] Progress difficulty automatically as player improves
- [ ] Add a short "correct" micro-animation
- [ ] Add a short "incorrect" micro-animation (gentle, not punishing)
- [ ] Add a streak-milestone celebration moment (e.g. every 5 in a row)
- [ ] Add a level-up moment/notification

## Phase 3 — Identity & cloud sync

- [ ] Design Firestore schema for user progress (streak, XP, level, skill state)
- [ ] Create `IProgressSyncService` interface in `.Shared`
- [ ] Implement Firestore read call (native, using existing Firebase setup)
- [ ] Implement Firestore write call (native)
- [ ] Update `StreakService`/`XpService` to check `AccountManagerService.State`
- [ ] Route guest reads/writes to `Preferences` only (existing behavior)
- [ ] Route signed-in reads/writes to Firestore
- [ ] Cache signed-in user's data locally for offline play
- [ ] Sync local cache to Firestore when connectivity returns
- [ ] Decide merge behavior when a guest with local progress signs in
- [ ] Implement chosen merge behavior
- [ ] Pull latest Firestore data on app start for signed-in users
- [ ] Handle Firestore read/write errors gracefully (no silent data loss)
- [ ] Add a loading state while syncing on app start
- [ ] Test sign-in on a second device to confirm sync actually works

## Phase 4 — Leaderboard

- [ ] Design `leaderboard/{uid}` Firestore document shape
- [ ] Write Firestore security rules: user can only write their own document
- [ ] Write Firestore security rules: any authenticated user can read the collection
- [ ] Test security rules using Firebase Console's rules simulator
- [ ] Build `/leaderboard` page skeleton
- [ ] Query and display top N players sorted by score
- [ ] Highlight current user's row if visible in the list
- [ ] Show current user's rank even if outside the visible top N
- [ ] Decide all-time vs weekly vs both
- [ ] Implement chosen reset/timeframe logic
- [ ] Add empty state for a brand-new leaderboard with no entries
- [ ] Add pull-to-refresh or manual refresh action
- [ ] Link Home page or nav to the new leaderboard screen

## Phase 5 — Ship it

- [ ] Design adaptive icon foreground layer (safe-zone padded)
- [ ] Design adaptive icon background layer
- [ ] Wire adaptive icon into Android build config
- [ ] Take Home screen screenshot for store listing
- [ ] Take Play screen screenshot for store listing
- [ ] Take Leaderboard screen screenshot for store listing
- [ ] Take Account screen screenshot for store listing
- [ ] Resize screenshots to Play Store's required dimensions
- [ ] Write short store description (~80 chars)
- [ ] Write full store description
- [ ] Write "what's new" text for first release
- [ ] Write a short privacy policy page
- [ ] Host privacy policy somewhere reachable by URL
- [ ] Decide whether to build the optional landing website
- [ ] If building website: reuse `QuickMath.Web.Client` for shared UI
- [ ] If building website: deploy it somewhere free (GitHub Pages or similar)
- [ ] Re-read Play Store developer policy for anything AGPL-relevant
- [ ] Confirm app complies with Play Store's data safety disclosure requirements
- [ ] Fill out Play Console's data safety form
- [ ] Set up Play Console app listing (category, contact email, etc.)
- [ ] Upload signed release build (`.aab`) to Play Console
- [ ] Submit for review
- [ ] Monitor Firebase Authentication usage against free tier limits
- [ ] Monitor Firestore reads/writes against free tier limits
- [ ] Plan a fallback if free tier limits are ever exceeded

## Post-launch / stretch ideas

- [ ] Push notifications for streak reminders
- [ ] Daily challenge mode
- [ ] Sound effects for correct/incorrect answers
- [ ] Optional dark mode toggle (deferred earlier decision)
- [ ] Achievements/badges system
- [ ] Friend leaderboard (not just global)
- [ ] iOS build (currently Android/Windows-only in practice)
- [ ] Localization / multi-language support
