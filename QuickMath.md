# Plan QwickMath — Liste officielle du projet

## Platforms & ordre de build

1. **Windows Forms** — Full lokal, QuickMath-Desktop
2. **Web** — version principale (React)
3. **Android** — React Native (même codebase que le web quasi)

---

## Clarification React

React ≠ Electron.

- **Electron** = app desktop avec React/JS (lourd, pas ce qu'on veut)
- **React** = site web normal
- **React Native** = app mobile Android/iOS, code séparé mais même logique/langage

Donc le plan est :

- React pour le site web
- React Native pour Android
- Beaucoup de code/logique réutilisable entre les deux 

---

## Hosting (tout gratuit)

| Option           | Prix | Domaine custom |
| ---------------- | ---- | -------------- |
| Firebase Hosting | 0€   | (payant)       |
| GitHub Pages     | 0€   | (gratuit)      |
| Vercel           | 0€   | (gratuit)      |

**Recommandation : Firebase Hosting** — tout au même endroit (auth + db + hosting)

---

## Backend — Firebase (tout gratuit lol)

- **Auth** — login Google, email/password
- **Firestore** — XP, stats, profil utilisateur
- **Hosting** — site web
- **Functions** — logique serveur si besoin (leaderboard sécurisé etc.)

---

## Liste des choses à faire — dans l'ordre (je crois)

### 🟡 Maintenant — WinForms

- [x] Addition Fini
- [x] Ajouter soustraction 
- [ ] Ajouter division
- [ ] Ajouter multiplication
- [x] Système de difficulté qui marche (WIP)
- [ ] UI plus propre (WIP)
- [ ] Tester le gameplay — est-ce que c'est fun ? (non)
- [ ] Trouver meilleur que json pour les saves (SQL?)

### 🔵 Ensuite — Firebase setup

- [x] Créer projet Firebase
- [x] Configurer Auth Google
- [ ] Définir structure Firestore (XP, username, stats...)
- [ ] Tester API

### 🟣 Web — ~~React~~ vue.js

- [x] Setup projet ~~React~~ vue
- [ ] Pages : Home, Jeu, Profil, Leaderboard
- [x] Intégrer Firebase Auth
- [ ] Intégrer Firestore
- [ ] Deploy sur Firebase Hosting

### 🟢 Android — ~~React Native~~ a mediter

- [ ] Setup React Native
- [ ] Réutiliser logique du web
- [ ] Tester sur émulateur
- [ ] Préparer pour Play Store
- [ ] Compte dev Play Store — ~20€ une seule fois

### ⚪ Plus tard

- [ ] Streaks journaliers
- [ ] Défis quotidiens
- [ ] Badges / succès
- [ ] Modes de jeu (contre la montre, survie)
- [ ] Leaderboard global

---

## Budget

| Chose                   | Coût                         |
| ----------------------- | ---------------------------- |
| Firebase                | $0                           |
| GitHub Pages / Vercel   | $0                           |
| Domaine custom          | $0 (sans domaine) ou ~10€/an |
| Compte Play Store       | ~20€ une fois                |
| **Total minimum**       | **$0**                       |
| **Total si Play Store** | **~20€**                     |

Allright ca a l'air pas mal
