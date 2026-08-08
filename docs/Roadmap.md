# QuickMath — Roadmap

$0 budget 😅

## Roadmap phases



- [x] Guest / sign-in decision screen
- [ ] **Fix Android guest + Google sign-in (debug session needed)**
- [x] **Fix/build Windows Google sign-in (Desktop OAuth client + PKCE flow)**
- [ ] Resolve `/practice` vs `/play` route naming — pick one, use everywhere
- [ ] First real exercise screen: one exercise type (e.g. multiplication tables), question generation, answer input, instant right/wrong feedback
- [ ] Local streak counter (`StreakService`, using `Preferences`, following the guest/signed-in split pattern)

- [ ] Multiple exercise types / skill tree (addition, fractions, algebra, etc.)
- [ ] XP, levels, streak visuals (progress bar, animations) — extend the playful visual style already established
- [ ] Difficulty progression / unlock gating

- [ ] Finish Google Sign-In on both Android and Windows
- [ ] Exchange Google ID token for a Firebase session (`signInWithIdp` REST endpoint)
- [ ] Sync local progress to Firestore once signed in
- [x] `AccountManagerService` / `StreakService` decide storage target based on `AccountState` (guest → `Preferences` only; signed in → Firestore, possibly cached locally too)

- [ ] Firestore collection: user, score, streak
- [ ] Leaderboard screen, sorted, maybe weekly reset
- [ ] Firestore security rules gated on `request.auth.uid`

- [ ] App icon, screenshots, store listing
- [ ] Play Store submission using developer account
- [ ] Confirm AGPL license terms are compatible with the store listing
