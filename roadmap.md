# QuickMath Roadmap

## Vision

Créer une application d'apprentissage des mathématiques type "Duolingo", avec progression, XP et exercices interactifs.

---

## 🧱 Stack

- Frontend : Blazor (Web + éventuellement WASM)
- Backend : ASP.NET Core
- Database : SQLite (début) → PostgreSQL (plus tard)
- Language : C#
- License : AGPLv3

---

## Phase 0 — Prototype local (WinForms / test)

- [x] Prototype addition
- [x] Soustraction
- [ ] Multiplication
- [ ] Division
- [x] Système de difficulté simple
- [ ] Tester si le gameplay est fun

---

## Phase 1 — MVP Web (Blazor)

- [x] Setup projet Blazor
- [ ] Page Home
- [ ] Système exercices (addition/soustraction)
- [ ] Système XP
- [ ] Progression utilisateur
- [ ] Sauvegarde SQLite

---

## Phase 2 — Backend propre

- [ ] ASP.NET Core API
- [ ] Auth (simple email/password)
- [ ] Gestion utilisateurs
- [ ] Sauvegarde cloud (optionnel)

---

## Phase 3 — Contenu éducatif

- [ ] Système de niveaux
- [ ] Exercices progressifs
- [ ] Difficulté adaptative
- [ ] Streaks
- [ ] Badges

---

## Phase 4 — Extensions

- [ ] Leaderboard
- [ ] Défis quotidiens
- [ ] Algèbre
- [ ] Géométrie
- [ ] Version Android (MAUI ou autre)

---

## 🌐 Hosting

- Dev : localhost + Raspberry Pi 5
- Public : VPS ou Oracle Cloud Free Tier
- Option simple : Blazor WASM + GitHub Pages (si statique)

---

## ⚠️ Règles

- Pas de React
- Pas de JavaScript frontend complexe
- Priorité au C# / .NET
- Simple avant scalable
- pas de electron
- idk
- et respecter svp les [rules](https://raw.githubusercontent.com/pikmintea/RULES/refs/heads/main/RULES.TXT)
