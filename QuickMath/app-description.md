# QuickMath — Application Description

## Overview

QuickMath is a cross-platform math practice application built with **.NET MAUI Blazor** targeting **.NET 10**. It combines a native MAUI shell with Blazor WebView for the mobile/desktop app and a full ASP.NET Core Blazor Web app with Auto render mode (Interactive Server + Interactive WebAssembly) for the web version.

---

## Project Structure

The solution is split into **4 projects**:

| Project | Type | Target | Purpose |
|---------|------|--------|---------|
| `QuickMath` | MAUI Blazor App | `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, `net10.0-windows10.0.19041.0` | Native shell hosting a BlazorWebView |
| `QuickMath.Web` | ASP.NET Core Web App | `net10.0` | Server-side Blazor host with InteractiveAuto render mode |
| `QuickMath.Web.Client` | Blazor WebAssembly | `net10.0` | Client-side WASM assembly for InteractiveAuto prerendering |
| `QuickMath.Shared` | Razor Class Library | `net10.0` | Shared Blazor pages, layouts, components, and CSS used by all hosts |

---

## Architecture

### Render Mode (Web)
- **InteractiveAuto** — pages render as Blazor Server (SignalR) on first load, then hydrate to Blazor WebAssembly for offline-capable client-side execution
- Server uses `AddInteractiveServerComponents()` + `AddInteractiveWebAssemblyComponents()`
- Client assembly loaded via `AddAdditionalAssemblies(typeof(QuickMath.Shared._Imports).Assembly, typeof(QuickMath.Web.Client._Imports).Assembly)`

### MAUI Native Shell
- `MainPage.xaml` hosts a `<BlazorWebView>` pointing to `wwwroot/index.html`
- Root component: `QuickMath.Shared.Routes`
- Same shared Razor components render inside the native WebView

### Form Factor Abstraction
- `IFormFactor` interface in Shared defines `GetFormFactor()` and `GetPlatform()`
- Three platform-specific implementations:
  - **MAUI**: uses `DeviceInfo.Idiom` and `DeviceInfo.Platform`
  - **Web (Server)**: returns `"Web"` + `Environment.OSVersion`
  - **Web (Client/WASM)**: returns `"WebAssembly"` + `Environment.OSVersion`

---

## Pages / Routes

| Route | Page | Status | Description |
|-------|------|--------|-------------|
| `/` | `Home.razor` | Partial | Dashboard with XP bar, level, daily streak, weekly activity dots, world rank. Currently uses hardcoded static data |
| `/practice` | `Practice.razor` | Placeholder | Default Blazor counter template — no math practice logic implemented yet |
| `/profile` | `Profile.razor` | Placeholder | Default Blazor weather forecast template — no profile logic implemented yet |
| `/not-found` | `NotFound.razor` | Done | Simple 404 page |
| `/Error` | `Error.razor` (Web-only) | Done | Standard ASP.NET Core error page with RequestId |

---

## Layout

### `MainLayout.razor`
- Standard sidebar + main content layout
- Sidebar contains `<NavMenu />`
- Top row has an "About" link pointing to `https://github.com/Yel0w08/QuickMath/wiki/About`
- Includes `#blazor-error-ui` fallback banner

### `NavMenu.razor`
- Navigation items: **Home**, **Practice**, **Profile**
- Bootstrap Icons via inline SVG data URIs
- Responsive: collapses to hamburger menu on screens < 641px
- Dark theme overrides via inline `<style>` block

### `ReconnectModal.razor` (Web.Client)
- Blazor Server reconnection dialog with retry/resume/reload states
- Uses a `<dialog>` element with SignalR reconnection event classes

---

## UI / Theming

### Dark Theme (MAUI `app.css`)
```css
--bg:     #0a0a0f   (background)
--bg2:    #111118   (card/surface)
--bg3:    #1a1a24   (elevated)
--accent: #8e24ff   (purple accent)
--text:   #f0f0f5   (primary text)
--muted:  #6b6b80   (secondary text)
```

### Home Page Styling
- XP bar with gradient fill (`--accent` → `#a855f7`)
- Level badge with circular border
- Streak card with fire emoji and weekly dot grid
- Responsive grid: 2-column on desktop, single column on mobile (< 600px)
- Uses CSS custom properties from the MAUI theme

### Bootstrap
- Referenced from `Shared/wwwroot/lib/bootstrap/` (local copy)
- Standard Blazor template CSS also present in `Shared/wwwroot/app.css`

---

## Dependencies

### MAUI Project
| Package | Version |
|---------|---------|
| `Microsoft.Maui.Controls` | 10.0.60 |
| `Microsoft.AspNetCore.Components.WebView.Maui` | 10.0.60 |
| `Microsoft.Extensions.Logging.Debug` | 10.0.7 |

### Shared Project
| Package | Version | Purpose |
|---------|---------|---------|
| `Google.Apis.Auth` | 1.74.0 | Google OAuth (planned) |
| `Google.Cloud.Firestore` | 4.2.0 | Firebase Firestore (planned) |
| `Microsoft.AspNetCore.Components.Web` | 10.0.7 | Blazor component base |

### Web Project
| Package | Version |
|---------|---------|
| `Microsoft.AspNetCore.Components.WebAssembly.Server` | 10.0.0 |

### Web.Client Project
| Package | Version |
|---------|---------|
| `Microsoft.AspNetCore.Components.WebAssembly` | 10.0.0 |

---

## Planned Backend (Not Yet Implemented)

### Firebase Integration (planned)
- **Auth**: Google login, email/password
- **Firestore**: XP, stats, user profiles
- **Hosting**: Web deployment
- **Cloud Functions**: Server-side logic (leaderboard, etc.)

### Firestore is referenced in packages but NOT wired up:
- `Google.Apis.Auth` and `Google.Cloud.Firestore` are installed in `QuickMath.Shared`
- No service classes, DI registration, or actual Firestore calls exist in code yet
- All Home page data is hardcoded (`Username = "Yelow"`, `XP = 70`, etc.)

---

## Home Page Data Model (Hardcoded)

```
Username: "Yelow"
XP: 70 / 100 (70%)
Level: 0 — "Newbie"
Streak: 1 day
BestStreak: 1
DoneToday: true
Rank: #1 / 1 total players
WeekDays: 7 days (only Sunday marked done + today)
```

---

## Platform Support

| Platform | Target Framework | Min Version |
|----------|-----------------|-------------|
| Android | `net10.0-android` | API 24 |
| iOS | `net10.0-ios` | 15.0 |
| Mac Catalyst | `net10.0-maccatalyst` | 15.0 |
| Windows | `net10.0-windows10.0.19041.0` | 10.0.17763.0 |

- Windows package type: `None` (not packaged for Microsoft Store)
- App ID: `com.companyname.quickmath`
- Version: 1.0 (build 1)

---

## Build / Debug Features

### MAUI Debug Mode
- BlazorWebView developer tools enabled (`AddBlazorWebViewDeveloperTools()`)
- Android WebView remote debugging: `WebView.SetWebContentsDebuggingEnabled(true)`
- Debug logging via `Microsoft.Extensions.Logging.Debug`

### Web Debug Mode
- WebAssembly debug proxy enabled in development
- Standard ASP.NET Core exception handler for production
- HSTS enabled in production
- Status code pages: 404 → `/not-found`

---

## App Icon & Assets

- **Icon**: `Resources/AppIcon/appicon.svg` with foreground `appiconfg.svg`, color `#512BD4`
- **Splash**: `Resources/Splash/splash.svg`, color `#512BD4`, base size 128x128
- **Images**: `Resources/Images/*` (includes `dotnet_bot.svg` at 168x208)
- **Fonts**: `Resources/Fonts/*` (OpenSans-Regular.ttf registered)
- **Raw Assets**: `Resources/Raw/**`
- **Favicon**: `Shared/wwwroot/favicon.png`

---

## XAML Performance
- `MauiXamlInflator` set to `SourceGen` — XAML is compiled to C# at build time instead of runtime inflation

---

## Current State Summary

| Area | Status |
|------|--------|
| Project scaffolding | Complete — 4-project MAUI Blazor solution |
| Home page UI | Complete (visual only, static data) |
| Practice page | Placeholder (counter demo) |
| Profile page | Placeholder (weather demo) |
| Firebase/Auth | Packages installed, zero implementation |
| Backend/API | None |
| Leaderboard | UI placeholder only |
| Navigation/Layout | Complete |
| Dark theme | Complete |
| Responsive design | Basic (sidebar collapse, card grid) |
| Multi-platform | Configured for Android/iOS/Mac/Windows |
| Web Auto render mode | Configured (Server → WASM hydration) |
| Error handling | Basic (Blazor error UI, Error page, NotFound) |
| Reconnection modal | Standard Blazor Server template |

---

## Key File Locations

```
QuickMath/
├── QuickMath/                          # MAUI app
│   ├── MauiProgram.cs                  # DI, fonts, BlazorWebView setup
│   ├── App.xaml.cs                     # Window creation
│   ├── MainPage.xaml                   # BlazorWebView host
│   └── wwwroot/app.css                 # Dark theme CSS vars
│
├── QuickMath.Web/                      # ASP.NET Core server
│   ├── Program.cs                      # RazorComponents, render modes
│   ├── Components/App.razor            # HTML shell, Routes @rendermode
│   └── Components/Pages/Error.razor    # Error page
│
├── QuickMath.Web.Client/               # WASM client
│   ├── Program.cs                      # WASM host builder
│   ├── Services/FormFactor.cs          # "WebAssembly" impl
│   └── Layout/ReconnectModal.razor     # SignalR reconnect UI
│
└── QuickMath.Shared/                   # Shared Razor library
    ├── Routes.razor                    # Router component
    ├── _Imports.razor                  # Global usings
    ├── Pages/
    │   ├── Home.razor                  # Dashboard (static data)
    │   ├── Practice.razor              # Placeholder
    │   ├── Profile.razor               # Placeholder
    │   └── NotFound.razor              # 404 page
    ├── Layout/
    │   ├── MainLayout.razor            # Sidebar + content
    │   └── NavMenu.razor               # Navigation links
    ├── Services/
    │   └── IFormFactor.cs              # Platform abstraction
    └── wwwroot/
        ├── app.css                     # Default Blazor CSS
        ├── lib/bootstrap/              # Local Bootstrap
        └── favicon.png
```
