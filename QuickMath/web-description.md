# QuickMath — Web Platform Description

## Overview

The web platform consists of two projects working together:

- **`QuickMath.Web`** — ASP.NET Core server hosting the Blazor Auto render mode (Interactive Server + Interactive WebAssembly)
- **`QuickMath.Web.Client`** — Blazor WebAssembly client assembly that gets downloaded to the browser for client-side interactivity after server-side prerendering

Both projects reference `QuickMath.Shared`, a Razor Class Library containing all pages, layouts, components, and static web assets.

---

## Architecture: InteractiveAuto Render Mode

### How It Works

1. **Initial load** — Pages render on the server via Blazor Server (SignalR circuit)
2. **WASM download** — The .NET WebAssembly runtime and assemblies download in the background
3. **Hydration** — Once WASM is ready, interactivity transfers from Server to Client
4. **Offline capable** — After hydration, the client can handle UI interactions without server round-trips

### Server Configuration (`QuickMath.Web/Program.cs`)

```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()       // Server-side SignalR
    .AddInteractiveWebAssemblyComponents(); // WASM client support

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(QuickMath.Shared._Imports).Assembly,
        typeof(QuickMath.Web.Client._Imports).Assembly);
```

- **Static assets**: `app.MapStaticAssets()` — serves files from `wwwroot` and RCLs
- **Antiforgery**: `app.UseAntiforgery()` — enabled for form protection
- **HTTPS redirect**: `app.UseHttpsRedirection()` — enforced
- **HSTS**: `app.UseHsts()` — production only
- **WASM debug proxy**: `app.UseWebAssemblyDebugging()` — development only
- **Error handling**: `app.UseExceptionHandler("/Error")` — production
- **Status code pages**: `app.UseStatusCodePagesWithReExecute("/not-found")` — 404 handling

### Client Configuration (`QuickMath.Web.Client/Program.cs`)

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<IFormFactor, FormFactor>();
await builder.Build().RunAsync();
```

- Minimal setup — just registers the `IFormFactor` service
- No HTTP client, no auth, no additional services configured

### HTML Shell (`QuickMath.Web/Components/App.razor`)

```html
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <ResourcePreloader />
    <link rel="stylesheet" href="_content/QuickMath.Shared/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="_content/QuickMath.Shared/app.css" />
    <link rel="stylesheet" href="QuickMath.Web.styles.css" />
    <ImportMap />
    <link rel="icon" type="image/png" href="_content/QuickMath.Shared/favicon.png" />
    <HeadOutlet @rendermode="InteractiveAuto" />
</head>
<body>
    <Routes @rendermode="InteractiveAuto" />
    <ReconnectModal />
    <script src="@Assets["_framework/blazor.web.js"]"></script>
</body>
</html>
```

Key elements:
- `<ResourcePreloader />` — preloads WASM assets for faster hydration
- `<ImportMap />` — ES module import map for browser-native module loading (.NET 10 feature)
- `<HeadOutlet @rendermode="InteractiveAuto" />` — SSR-capable head management
- `<Routes @rendermode="InteractiveAuto" />` — all routing uses Auto mode
- `<ReconnectModal />` — SignalR reconnection UI (from Web.Client)
- `@Assets["_framework/blazor.web.js"]` — new .NET 10 asset resolution syntax

---

## Launch Settings

**Location:** `QuickMath.Web/Properties/launchSettings.json`

| Profile | URL | Protocol |
|---------|-----|----------|
| `http` | `http://localhost:5078` | HTTP |
| `https` | `https://localhost:7264` / `http://localhost:5078` | HTTPS + HTTP |

- Both profiles auto-launch browser
- `inspectUri` configured for Blazor WebAssembly debugging in browser DevTools
- `ASPNETCORE_ENVIRONMENT` set to `Development`

---

## Static Assets

### Sources

| Source | Path | Contents |
|--------|------|----------|
| `QuickMath.Web` | `wwwroot/` | (empty — no local assets) |
| `QuickMath.Web.Client` | `wwwroot/` | `appsettings.json`, `appsettings.Development.json` |
| `QuickMath.Shared` | `wwwroot/` | `app.css`, `favicon.png`, `lib/bootstrap/` |

### Asset Resolution in App.razor

| Asset | Resolved Path | Source |
|-------|--------------|--------|
| Bootstrap CSS | `_content/QuickMath.Shared/lib/bootstrap/dist/css/bootstrap.min.css` | Shared RCL |
| App CSS | `_content/QuickMath.Shared/app.css` | Shared RCL |
| Generated CSS | `QuickMath.Web.styles.css` | Auto-generated (CSS isolation) |
| Favicon | `_content/QuickMath.Shared/favicon.png` | Shared RCL |
| Blazor JS | `@Assets["_framework/blazor.web.js"]` | Framework |
| Reconnect JS | `@Assets["Layout/ReconnectModal.razor.js"]` | Web.Client |

### Bootstrap

- Local copy in `Shared/wwwroot/lib/bootstrap/dist/css/`
- **Only CSS is included** — no JS file
- `bootstrap.min.css.map` is also present (source map)
- Version: unknown (no version comment in file, appears to be Bootstrap 5 based on class names)

### App CSS (Shared)

**Location:** `Shared/wwwroot/app.css`

Standard Blazor template styles:
- `.btn-primary` — blue `#1b6ec2`
- Validation styles (`.valid`, `.invalid`, `.validation-message`)
- `.blazor-error-boundary` — yellow/red error banner
- Form control focus shadows
- `.content` padding

**⚠️ Issue:** These styles conflict with the dark theme. The `.btn-primary` color (`#1b6ec2`) and link color (`#006bb7`) don't match the app's dark theme accent (`#8e24ff`). The MAUI `app.css` overrides these with `!important` on `body`, but the shared `app.css` is also loaded in the web app where the dark theme CSS vars may not be applied consistently.

---

## WebAssembly Download Size

### Framework Files (from build output)

The WASM client downloads the .NET runtime and all assemblies to the browser. Key files include:

| File | Type |
|------|------|
| `dotnet.native.*.wasm` | .NET runtime (AOT/native code) |
| `dotnet.native.*.js` | Runtime loader |
| `dotnet.runtime.*.js` | Runtime bootstrap |
| `dotnet.js` | Entry point |
| `icudt_*.dat` | ICU globalization data (3 variants) |
| `blazor.webassembly.js` | Blazor WASM bootstrapper |

### Application Assemblies (WASM, gzipped)

| Assembly | Purpose |
|----------|---------|
| `QuickMath.Web.Client.wasm` | Client project |
| `QuickMath.Shared.wasm` | Shared pages/components |
| `Microsoft.AspNetCore.Components.WebAssembly.wasm` | Blazor WASM framework |
| `Microsoft.AspNetCore.Components.Web.wasm` | Blazor web components |
| `Microsoft.AspNetCore.Components.wasm` | Core Blazor |
| `Microsoft.AspNetCore.Authorization.wasm` | Auth primitives |
| `Microsoft.AspNetCore.Metadata.wasm` | Metadata |
| `Microsoft.JSInterop.wasm` | JS interop |
| `Microsoft.JSInterop.WebAssembly.wasm` | WASM JS interop |
| `Microsoft.Extensions.*.wasm` | DI, Config, Options, etc. |
| `Google.Apis.wasm` | Google API client |
| `Google.Apis.Auth.wasm` | Google Auth |
| `Google.Apis.Core.wasm` | Google API Core |
| `Google.Cloud.Firestore.wasm` | Firestore client |
| `Google.Cloud.Firestore.V1.wasm` | Firestore V1 |
| `Google.Cloud.Location.wasm` | Google Location |
| `Google.Protobuf.wasm` | Protocol Buffers |
| `Google.Api.Gax.wasm` | Google API Extensions |
| `Google.Api.Gax.Grpc.wasm` | gRPC extensions |
| `Google.Api.CommonProtos.wasm` | Common protos |
| `Google.LongRunning.wasm` | Long-running ops |
| `Grpc.Core.Api.wasm` | gRPC Core API |
| `Grpc.Auth.wasm` | gRPC Auth |
| `Grpc.Net.Client.wasm` | gRPC .NET Client |
| `Grpc.Net.Common.wasm` | gRPC Common |
| `Microsoft.Bcl.AsyncInterfaces.wasm` | Async interfaces |
| `Microsoft.DotNet.HotReload.WebAssembly.Browser.wasm` | Hot Reload (debug only) |
| `Microsoft.VisualBasic.wasm` | VB compat |
| `Microsoft.VisualBasic.Core.wasm` | VB Core |

### ⚠️ Critical Issue: Google Packages Inflate WASM Bundle

**`Google.Apis.Auth`** and **`Google.Cloud.Firestore`** (plus all their transitive dependencies: gRPC, Protobuf, Google.Api.Gax, etc.) are included in the WASM download even though they are **not used** in the web app.

These packages add **significant weight** to the initial download:
- ~25+ additional assemblies
- gRPC stack (not needed for Firestore REST)
- Protobuf serialization
- Multiple Google API layers

**Impact:** Longer initial load time, larger WASM payload, slower time-to-interactive.

**Recommendation:** Either remove these packages from `QuickMath.Shared` (they shouldn't be in a shared web library) or move them to a server-only project (`QuickMath.Web`) and use HTTP APIs from the client instead.

---

## Reconnection Modal

**Location:** `QuickMath.Web.Client/Layout/ReconnectModal.razor` + `ReconnectModal.razor.js`

### HTML Structure
- `<dialog id="components-reconnect-modal">` with multiple state-based visibility classes
- States: `show`, `hide`, `failed`, `rejected`, `paused`
- Retry button, Resume button
- Countdown timer for retry attempts (`#components-seconds-to-next-attempt`)
- Rejoining animation (CSS-only, two divs)

### JavaScript Logic (`ReconnectModal.razor.js`)

| Event/Function | Behavior |
|----------------|----------|
| `components-reconnect-state-changed` → `show` | Opens modal via `showModal()` |
| `components-reconnect-state-changed` → `hide` | Closes modal |
| `components-reconnect-state-changed` → `failed` | Registers `visibilitychange` listener for retry when tab becomes visible |
| `components-reconnect-state-changed` → `rejected` | Auto-reloads page (circuit lost) |
| `retry()` | Calls `Blazor.reconnect()`, falls back to `Blazor.resumeCircuit()`, reloads if both fail |
| `resume()` | Calls `Blazor.resumeCircuit()`, reloads on failure |
| `retryWhenDocumentBecomesVisible()` | Retries when user returns to the tab |

**⚠️ Issue:** The `resumeCircuit()` call in the `retry()` function's catch block is incorrect — `resumeCircuit` should only be called when the server explicitly paused the circuit, not as a fallback for a failed reconnect. The current logic tries `reconnect()` first, then unconditionally tries `resumeCircuit()` if reconnect returns `false`. This may cause unnecessary circuit resume attempts when a simple reconnect retry or reload would be more appropriate.

---

## Server-Side Error Handling

### Error Page (`QuickMath.Web/Components/Pages/Error.razor`)

- Route: `/Error`
- Displays `RequestId` from `Activity.Current?.Id` or `HttpContext?.TraceIdentifier`
- Development mode message about switching environment
- Standard ASP.NET Core error template

### NotFound Page (`QuickMath.Shared/Pages/NotFound.razor`)

- Route: `/not-found`
- Uses `MainLayout`
- Simple "Not Found" message
- Triggered by `app.UseStatusCodePagesWithReExecute("/not-found")`

### Blazor Error UI (MainLayout)

- `#blazor-error-ui` div at bottom of page
- "An unhandled error has occurred" with Reload link and dismiss button
- Standard Blazor template error boundary

---

## App Settings

### Server (`QuickMath.Web/appsettings.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

- `AllowedHosts: "*"` — accepts requests from any host header (fine for dev, restrict for production)

### Server Dev (`QuickMath.Web/appsettings.Development.json`)

Same logging config, no `AllowedHosts` override.

### Client (`QuickMath.Web.Client/wwwroot/appsettings.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

- Identical to dev config — no differentiation
- **⚠️ Issue:** Client `appsettings.json` is served to the browser. Currently it only contains logging config, but if any secrets or API keys are added here in the future, they will be exposed to clients.

---

## CSS Isolation

| File | Scope |
|------|-------|
| `MainLayout.razor.css` | `MainLayout` component |
| `NavMenu.razor.css` | `NavMenu` component |
| `ReconnectModal.razor.css` | `ReconnectModal` component (in Web.Client) |
| `Home.razor` (inline `<style>`) | Page-level inline styles |

- Generated CSS bundle: `QuickMath.Web.styles.css` (auto-generated at build)
- NavMenu also has inline `<style>` block in the `.razor` file that duplicates/overrides the `.razor.css` file — potential conflict

---

## Routing Configuration

### Server-Side (`Program.cs`)

```csharp
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(QuickMath.Shared._Imports).Assembly,
        typeof(QuickMath.Web.Client._Imports).Assembly);
```

- Scans `Shared` and `Web.Client` assemblies for `@page` directives
- `App.razor` is the root component in `QuickMath.Web/Components/`

### Client-Side Router (`QuickMath.Shared/Routes.razor`)

```razor
<Router AppAssembly="typeof(Layout.MainLayout).Assembly" NotFoundPage="typeof(Pages.NotFound)">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
```

- `AppAssembly` points to `QuickMath.Shared` (where `MainLayout` lives)
- `FocusOnNavigate` focuses `<h1>` on route change for accessibility

### Defined Routes

| Route | Component | Assembly |
|-------|-----------|----------|
| `/` | `Home.razor` | Shared |
| `/practice` | `Practice.razor` | Shared |
| `/profile` | `Profile.razor` | Shared |
| `/not-found` | `NotFound.razor` | Shared |
| `/Error` | `Error.razor` | Web |

---

## Global Imports (`_Imports.razor`)

### Shared (`QuickMath.Shared/_Imports.razor`)

```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using QuickMath
@using QuickMath.Shared
@using QuickMath.Shared.Layout
```

- `RenderMode` imported as static — enables `@InteractiveAuto`, `@InteractiveServer`, etc. shorthand
- `Virtualization` imported but not used anywhere in the codebase
- `System.Net.Http` and `Json` imported but no HTTP client is registered or used

### Web (`QuickMath.Web/Components/_Imports.razor`)

Empty/minimal — inherits from Shared.

### Web.Client (`QuickMath.Web.Client/_Imports.razor`)

Empty/minimal — inherits from Shared.

### MAUI (`QuickMath/Components/_Imports.razor`)

Empty/minimal — for MAUI BlazorWebView components.

---

## Potential Issues & Risks

### 1. Google Packages Bloat WASM Bundle
**Severity: High**

As detailed above, `Google.Apis.Auth` and `Google.Cloud.Firestore` with all transitive dependencies (~25+ assemblies including gRPC, Protobuf, Google.Api.*) are downloaded to the browser but never used.

**Impact:**
- Increased initial page load time
- Larger bandwidth consumption
- Slower time-to-interactive
- Worse performance on slow networks

**Recommendation:** Move Firebase packages to `QuickMath.Web` (server-only) and implement a thin API layer. The client should call server endpoints, not talk directly to Firestore from the browser.

---

### 2. CSS Conflicts Between Shared and Dark Theme
**Severity: Medium**

The `Shared/wwwroot/app.css` contains default Blazor template styles:
- `.btn-primary` with `#1b6ec2` background
- Links with `#006bb7` color
- Light-mode `.blazor-error-boundary` with `lightyellow` background

The MAUI app overrides these with dark theme CSS vars, but the web app loads the same `app.css` from Shared. The dark theme variables (`--bg`, `--accent`, etc.) are only defined in `QuickMath/wwwroot/app.css` (the MAUI wwwroot), **not** in any web-accessible CSS file.

**Result:** The web app's body styling relies on inline overrides in `NavMenu.razor` and `Home.razor`, but base elements like buttons, links, and the error boundary may render with wrong colors.

**Recommendation:** Move the dark theme CSS variables (`:root { --bg: ...; --accent: ...; }`) from `QuickMath/wwwroot/app.css` to `QuickMath.Shared/wwwroot/app.css` so all hosts share the same theme.

---

### 3. No Server-Side API or Backend Logic
**Severity: High**

The web server (`QuickMath.Web`) is essentially a static file host + Blazor circuit manager:
- No controllers, minimal APIs, or signal hubs
- No authentication middleware
- No database connections
- All data is hardcoded in components

The server does nothing beyond serving the Blazor app. For a production app with user accounts, XP tracking, and leaderboards, a proper backend is needed.

---

### 4. No Authentication or Authorization
**Severity: High**

- No auth middleware configured (`AddAuthentication`, `AddAuthorization` missing)
- No login/logout pages
- No JWT, cookie, or OAuth configuration
- Google Auth package installed but not wired up
- All pages are publicly accessible with no access control

---

### 5. AllowedHosts is Wildcard
**Severity: Medium (for production)**

```json
"AllowedHosts": "*"
```

This accepts any `Host` header, which can lead to:
- DNS rebinding attacks
- Cache poisoning via malicious host headers
- Password reset token leakage through Host-based URL generation

**Recommendation:** Set to the actual domain name(s) in production (e.g., `"quickmath.example.com"`).

---

### 6. No CORS Configuration
**Severity: Medium**

- No CORS policy is configured
- If the app needs to call external APIs from the browser (like Firebase REST directly), CORS will block those requests unless the external service allows them
- If the server exposes APIs in the future, they'll be inaccessible to other origins without CORS setup

---

### 7. No Compression Configuration
**Severity: Medium**

- No response compression middleware (`AddResponseCompression`)
- Blazor WASM files are served pre-gzipped (`.wasm.gz`), which ASP.NET Core handles automatically
- But other static assets (CSS, HTML, JSON) are not compressed
- **Recommendation:** Add `AddResponseCompression()` with Brotli and Gzip providers

---

### 8. No Caching Headers for Static Assets
**Severity: Medium**

- WASM framework files have immutable content hashes in filenames, making them ideal for long-term caching
- But no cache-control headers are configured
- **Recommendation:** Configure `StaticFileOptions` with caching headers, especially for `_framework/` assets (cache for 1 year) and `appsettings.json` (no cache)

---

### 9. ImportMap and ResourcePreloader (.NET 10 Features)
**Severity: Low**

`App.razor` uses two .NET 10 features:
- `<ResourcePreloader />` — preloads WASM assets
- `<ImportMap />` — generates ES module import maps

These are cutting-edge features that may not be fully supported in all browsers:
- Import maps require Chromium 89+, Firefox 108+, Safari 16.4+
- Older browsers will fail to load the app

**Recommendation:** Test on target browsers and consider a fallback strategy for browsers without import map support.

---

### 10. Base Href and Deployment Path
**Severity: Medium**

```html
<base href="/" />
```

The app assumes it will be deployed at the root path (`/`). If deployed to a sub-path (e.g., `https://example.com/quickmath/`):
- All relative URLs will break
- The Blazor router won't match routes correctly
- Asset paths will be wrong

**Recommendation:** Make `<base href>` configurable via appsettings or environment variable, or ensure deployment always uses root path.

---

### 11. No PWA Support
**Severity: Low-Informational**

- No `manifest.json` or `service-worker.js`
- No `<link rel="manifest">` in HTML
- Despite being a "practice app" that could benefit from offline capability, there's no PWA setup
- Blazor WebAssembly supports PWA out of the box with the `BlazorWebAssemblyPwa` template

---

### 12. Virtualization Imported But Unused
**Severity: Informational**

`Microsoft.AspNetCore.Components.Web.Virtualization` is imported in `_Imports.razor` but no `<Virtualize>` components exist in the codebase. This adds unnecessary namespace pollution.

---

### 13. No Input Validation or Sanitization
**Severity: Medium**

- The `Profile.razor` page uses the default Blazor template with a weather forecast table — no user input at all
- When forms are added, validation (`DataAnnotationsValidator`, `EditForm`) and sanitization will be needed
- No XSS protection beyond Blazor's default HTML encoding (which is sufficient but should be verified for any `MarkupString` usage in the future)

---

### 14. No Rate Limiting
**Severity: Medium (for production)**

- No rate limiting middleware configured
- If/when API endpoints are added, they will be vulnerable to abuse
- .NET 10 includes built-in rate limiting (`AddRateLimiter()`)

---

### 15. Bootstrap CSS Only — No JS
**Severity: Low**

- Only `bootstrap.min.css` is included, no `bootstrap.bundle.min.js`
- Bootstrap JS components (modals, dropdowns, tooltips, collapse) won't work
- The `NavMenu` implements its own hamburger toggle with a hidden checkbox + CSS — this works but means Bootstrap's JS collapse is not used
- **Note:** This is actually fine for the current app since it doesn't use Bootstrap JS features. Just be aware that adding Bootstrap JS later would require the full bundle.

---

### 16. MudBlazor Reference in App.razor (Dead Code)
**Severity: Low**

`App.razor` contains a stray code snippet at the bottom:

```razor
@page "/"
<PageTitle>QuickMath</PageTitle>
<MudText Typo="Typo.h4" Color="Color.Primary">
    Welcome back <strong>@Username</strong> 👋
</MudText>
@code {
    public string Username = "John Doe";
}
```

This is dead code — `App.razor` is the HTML shell component, not a routable page (the `@page "/"` directive here is overridden by the `Routes` component). The `<MudText>` component references MudBlazor, which is **not** installed as a package, so this code would throw if ever executed.

**Recommendation:** Remove this dead code from `App.razor`.

---

### 17. No Open Graph / SEO Meta Tags
**Severity: Low**

The `<head>` in `App.razor` contains only basic meta tags:
- No `<meta name="description">`
- No Open Graph tags (`og:title`, `og:description`, `og:image`)
- No Twitter Card tags
- No `<title>` — it relies on `<PageTitle>` components (which works for SPA routing but not for initial server render/SEO)

**Recommendation:** Add SEO meta tags to the HTML shell and use `<PageTitle>` consistently.

---

### 18. Duplicate Inline Styles in NavMenu
**Severity: Low**

`NavMenu.razor` has both:
- A scoped `NavMenu.razor.css` file
- An inline `<style>` block in the `.razor` file

The inline styles override the scoped CSS with `!important` flags. This creates maintenance confusion — two places to edit for the same styles.

**Recommendation:** Consolidate into one location (preferably the `.razor.css` file or the inline block, not both).

---

### 19. No Analytics or Telemetry
**Severity: Informational**

- No Application Insights, Google Analytics, or any tracking
- No custom logging beyond default ASP.NET Core
- For a game/practice app, analytics would be valuable for understanding usage patterns

---

### 20. Web.Client Appsettings Duplicated
**Severity: Low**

`QuickMath.Web.Client/wwwroot/appsettings.json` and `appsettings.Development.json` are identical. The Development version doesn't override anything, making it pointless.

---

## Build & Run Commands

### Run (development, HTTPS)
```bash
cd QuickMath/QuickMath.Web
dotnet run --launch-profile https
```

### Run (development, HTTP)
```bash
dotnet run --launch-profile http
```

### Publish (self-contained for deployment)
```bash
cd QuickMath/QuickMath.Web
dotnet publish -c Release
```

Output: `bin/Release/net10.0/publish/`

### Debug WASM in Browser
1. Run with HTTPS profile
2. Open browser DevTools
3. Navigate to the `inspectUri` endpoint for Blazor debugging

---

## File Tree (Web-Specific)

```
QuickMath.Web/                          # ASP.NET Core Server
├── Properties/
│   └── launchSettings.json             # HTTP/HTTPS profiles
├── Components/
│   ├── App.razor                       # HTML shell (root component)
│   ├── _Imports.razor                  # Global usings
│   └── Pages/
│       └── Error.razor                 # Error page
├── Services/
│   └── FormFactor.cs                   # "Web" implementation
├── Program.cs                          # Server setup, render modes
├── appsettings.json                    # Server config
├── appsettings.Development.json        # Dev config
└── QuickMath.Web.csproj                # Project file

QuickMath.Web.Client/                   # Blazor WebAssembly Client
├── wwwroot/
│   ├── appsettings.json                # Client config
│   └── appsettings.Development.json    # Dev config (identical)
├── Layout/
│   ├── ReconnectModal.razor            # SignalR reconnect dialog
│   └── ReconnectModal.razor.js         # Reconnect logic
├── Services/
│   └── FormFactor.cs                   # "WebAssembly" implementation
├── Program.cs                          # WASM host builder
├── _Imports.razor                      # Global usings
└── QuickMath.Web.Client.csproj         # Project file

QuickMath.Shared/                       # Shared Razor Class Library
├── wwwroot/
│   ├── app.css                         # Default Blazor styles
│   ├── favicon.png                     # Site icon
│   └── lib/
│       └── bootstrap/
│           └── dist/
│               └── css/
│                   ├── bootstrap.min.css
│                   └── bootstrap.min.css.map
├── Pages/
│   ├── Home.razor                      # Dashboard (static data)
│   ├── Practice.razor                  # Placeholder
│   ├── Profile.razor                   # Placeholder
│   └── NotFound.razor                  # 404 page
├── Layout/
│   ├── MainLayout.razor                # Sidebar + content
│   ├── MainLayout.razor.css            # Scoped styles
│   ├── NavMenu.razor                   # Navigation
│   └── NavMenu.razor.css               # Scoped styles
├── Services/
│   └── IFormFactor.cs                  # Platform abstraction interface
├── Routes.razor                        # Router component
├── _Imports.razor                      # Global usings
├── QuickMath-Win.ico                   # Windows icon (not used in web)
└── QuickMath.Shared.csproj             # Project file
```

---

## Deployment Considerations

| Consideration | Status | Notes |
|--------------|--------|-------|
| HTTPS required | ✅ | `UseHttpsRedirection` enabled |
| HSTS | ✅ | Enabled in production |
| Static asset caching | ❌ | No cache headers configured |
| Response compression | ❌ | Only WASM gzipped, no Brotli |
| CORS | ❌ | Not configured |
| Rate limiting | ❌ | Not configured |
| Custom domain | ⚠️ | `AllowedHosts: "*"` — too permissive |
| Base path | ⚠️ | Hardcoded to `/` |
| Firebase hosting | Planned | Per project plan |
| GitHub Pages | Possible | Static output compatible |
| Vercel | Possible | Static output compatible |
| Docker | Not configured | No Dockerfile exists |
| CI/CD | Not configured | No GitHub Actions workflows |
