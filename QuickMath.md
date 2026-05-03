# Plan QwickMath — Liste officielle du projet

## Platforms & ordre de build

1. **Windows Forms** — Full lokal, QuickMath-Desktop
2. **Web** — version principale (React)
3. **Android** — React Native (même codebase que le web quasi)

---

## le plan :

- ~~React pour le site web~~
- ~~Vue pour le site web~~
- ~~React Native pour Android~~
- Maui Blazor, multiplateforme.

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

## Liste des choses à faire — (je crois)

### 🟡 WinForms

- [x] Addition Fini
- [x] Ajouter soustraction 
- [ ] Ajouter division
- [ ] Ajouter multiplication
- [x] Système de difficulté qui marche (WIP)
- [ ] UI plus propre (WIP)
- [ ] Tester le gameplay — est-ce que c'est fun ? (non)
- [ ] Trouver meilleur que json pour les saves (SQL?)
- [ ] refractor tout. (wip)
- [ ] Clen le code, arete les spageti (wip)
- [ ] Metre les fonction bcp utilisé (ex : SaveData) Dans des Class (v0.3.X) (wip)
- [ ] rendre plus simple a upgrade.

### 🔵 Firebase setup

- [x] Créer projet Firebase
- [x] Configurer Auth Google
- [ ] Définir structure Firestore (XP, username, stats...)
- [ ] Tester API

### 🟣 Web — ~~React~~ | ~~vue.js~~ | Maui Blazor

- [x] Setup projet ~~React~~ | ~~vue~~ | Maui Blazor
- [ ] Pages : Home, Jeu, Profil, Leaderboard
- [ ] Intégrer Firebase Auth
- [ ] Intégrer Firestore
- [ ] Deploy sur Firebase Hosting
- [ ] avoir un bon Ui

### 🟢 Android — ~~React Native~~ | ~~a mediter~~ | Maui blazor

- [ ] Setup ~~React Native~~ | Maui Blazor
- [ ] Réutiliser logique du web
- [ ] Tester sur émulateur
- [ ] Préparer pour Play Store
- [ ] Compte dev Play Store — ~20€ une seule fois
- [ ] Trouver meilleur moyent que react native...

### ⚪ Plus tard

- [ ] Streaks journaliers
- [ ] Défis quotidiens
- [ ] Badges / succès
- [ ] Modes de jeu (contre la montre, survie)
- [ ] Leaderboard global

---

## Budget

buget total : 0€ 

| Chose                   | Coût                         |
| ----------------------- | ---------------------------- |
| Firebase                | $0                           |
| GitHub Pages / Vercel   | $0                           |
| Domaine custom          | $0 (sans domaine) ou ~10€/an |
| Compte Play Store       | ~20€ une fois                |
| **Total minimum**       | **$0**                       |
| **Total si Play Store** | **~20€**                     |

Allright ca a l'air pas mal
