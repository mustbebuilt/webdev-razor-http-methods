# HTTP Methods & Routing in ASP.NET Core Razor Pages

An educational, interactive web demonstration suite created for students and developers to explore **HTTP Methods (GET vs POST)**, **URL Query Strings**, **Route Parameters**, and the **Post-Redirect-Get (PRG) Pattern** in ASP.NET Core Razor Pages.

---

## Quick Start

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (or .NET 8+)

### Running Locally
```bash
# Clone the repository and navigate into the project directory
cd webdev-razor-http-methods

# Restore packages and run using the HTTP profile
dotnet run --launch-profile http
```

Open your browser and navigate to:
```
http://localhost:5110
```

---

## Overview of the Four Interactive Demos

The application includes four dedicated interactive demos, accessible from the top navigation bar or the home hub (`/Index`):

### 1. HTTP GET & Query Strings (`/GetDemo`)
* **URL**: `/GetDemo`
* **Razor Page**: `Pages/GetDemo.cshtml` & `Pages/GetDemo.cshtml.cs`
* **Educational Focus**:
  * Demonstrates form submission using `<form method="get">`.
  * Form inputs are serialized directly into the URL query string (e.g. `?Subject=Computer+Science&Level=Beginner&Keyword=web`).
  * Shows how properties decorated with `[BindProperty(SupportsGet = true)]` are automatically populated from matching query string keys in Razor Pages.
  * Explains key characteristics of HTTP GET:
    * **Bookmarkable & Shareable**: The complete search/filter state is preserved in the URL.
    * **Idempotent**: Executing the request repeatedly does not mutate server state.
* **Interactive Features**:
  * Filter dropdowns (Subject, Level) and keyword search over course records loaded from `Data/courses.json`.
  * **Real-time Protocol Inspector**: Displays the raw HTTP request line, the browser URL, and breaks down individual query parameters parsed from `Request.Query`.

---

### 2. Single Route Parameter vs Query Strings (`/RouteDemo`)
* **URL Template**: `/RouteDemo/{id:int?}`
* **Razor Page**: `Pages/RouteDemo.cshtml` & `Pages/RouteDemo.cshtml.cs`
* **Directive**: `@page "{id:int?}"`
* **Educational Focus**:
  * Explains the architectural distinction between **Route Parameters** and **Query Strings**:
    * **Route Parameters** (`/RouteDemo/101`): Used to identify a *unique resource or entity* on the server (e.g. staff member ID 101).
    * **Query Strings** (`?tab=compensation`): Used for *view modifiers, filters, or UI options* applied to that resource.
  * Demonstrates how ASP.NET Core routing extracts path segments into `RouteData.Values["id"]` and binds them to PageModel properties.
* **Interactive Features**:
  * Staff entity selector buttons that update the path parameter (`/RouteDemo/101`, `/RouteDemo/102`, etc.).
  * Tab navigation that updates the query string (`?tab=overview`, `?tab=employment`, `?tab=compensation`).
  * **Real-time Routing Inspector**: Breaks down path parameters vs query parameters in a color-coded terminal view.

---

### 3. Multiple Route Parameters (`/MultiRouteDemo`)
* **URL Template**: `/MultiRouteDemo/{department?}/{id:int?}`
* **Razor Page**: `Pages/MultiRouteDemo.cshtml` & `Pages/MultiRouteDemo.cshtml.cs`
* **Directive**: `@page "{department?}/{id:int?}"`
* **Educational Focus**:
  * Demonstrates mapping hierarchical URL path segments using multiple route tokens in the `@page` directive.
  * Explains how ASP.NET Core Tag Helpers resolve parameters:
    * Attributes that match route tokens (`asp-route-department="engineering"` and `asp-route-id="101"`) become URL path segments: `/MultiRouteDemo/engineering/101`.
    * Attributes that do *not* match route tokens (such as `asp-route-tab="compensation"`) automatically fallback to query strings: `?tab=compensation`.
* **Interactive Features**:
  * Department selection (Segment 1) and Staff member selection (Segment 2).
  * Tab switching via query strings.
  * **Real-time Multi-Route Inspector**: Displays Segment 1, Segment 2, and Query String keys independently.

---

### 4. HTTP POST & Post-Redirect-Get (PRG) Pattern (`/PostDemo`)
* **URL**: `/PostDemo`
* **Razor Page**: `Pages/PostDemo.cshtml` & `Pages/PostDemo.cshtml.cs`
* **Educational Focus**:
  * Demonstrates data transmission inside the **HTTP Request Body** using `<form method="post">`.
  * Contrasts POST with GET:
    * Form inputs are *not* exposed in the browser's address bar, bookmarks, or web server access logs.
    * Used for state-mutating (non-idempotent) actions such as creating or updating records.
  * Explains and demonstrates the **Post-Redirect-Get (PRG)** pattern:
    1. **POST**: Client submits the form payload to `/PostDemo`.
    2. **Redirect**: Server processes data, persists it to `Data/feedback.json`, sets a flash message in `TempData`, and returns an HTTP 302/303 redirect to `RedirectToPage("/PostDemo")`.
    3. **GET**: Browser issues a clean GET request to display the confirmation message and updated list.
    4. *Result*: Refreshing the browser (F5) will **never** trigger duplicate form submissions or browser resubmission warnings.
* **Interactive Features**:
  * Interactive feedback submission form with ASP.NET Core validation attributes (`[Required]`, `[EmailAddress]`, `[StringLength]`).
  * Success alert powered by `TempData`.
  * Simulated wire-format inspection showing request headers and `application/x-www-form-urlencoded` body payload.
  * Table of persisted submissions loaded live from `Data/feedback.json`.

---

## HTTP GET vs HTTP POST Comparison

| Feature | HTTP GET | HTTP POST |
| :--- | :--- | :--- |
| **Primary Purpose** | Retrieving data / Reading state | Submitting data / Modifying state |
| **Data Location** | URL Query String (`?key=value`) | HTTP Request Body payload |
| **Bookmarkable & Shareable** | Yes (all parameters are in the URL) | No (parameters reside in request body) |
| **Idempotency** | Idempotent (safe to re-execute repeatedly) | Non-Idempotent (re-executing may duplicate data) |
| **Sensitive Data** | Never use (visible in history & logs) | Recommended (hidden from URL & history) |
| **Browser Refresh (F5)** | Safe; re-executes clean query | Dangerous without PRG; prompts resubmission alert |
| **Data Storage in App** | Queries `Data/courses.json` | Persists records to `Data/feedback.json` |

---

## Project Structure

```
webdev-razor-http-methods/
├── Data/
│   ├── courses.json        # Course records for GET filtering demo
│   ├── feedback.json       # Persisted student submissions for POST demo
│   └── staff.json          # Staff records for Route and MultiRoute demos
├── Models/
│   ├── Course.cs           # C# record model for courses
│   ├── FeedbackItem.cs     # C# record model for feedback submissions
│   └── StaffMember.cs      # C# record model for staff entities
├── Pages/
│   ├── Shared/
│   │   ├── _Layout.cshtml  # Global master layout with sticky responsive navigation
│   │   └── _Layout.cshtml.css (rationalized into site.css)
│   ├── _ViewImports.cshtml # Global Razor namespaces and tag helpers
│   ├── _ViewStart.cshtml   # Layout configuration
│   ├── Error.cshtml        # Error handling view
│   ├── Index.cshtml        # Home hub introducing the 4 demos
│   ├── GetDemo.cshtml      # Demo 1: GET and Query Strings
│   ├── RouteDemo.cshtml    # Demo 2: Single Route Parameter vs Query Strings
│   ├── MultiRouteDemo.cshtml # Demo 3: Multiple Route Parameters
│   └── PostDemo.cshtml     # Demo 4: POST and PRG pattern
├── Properties/
│   └── launchSettings.json # Local dev server port configuration (HTTP 5110)
├── Services/
│   └── JsonDataService.cs  # Service reading/writing JSON files with thread-safety
├── wwwroot/
│   └── css/
│       └── site.css        # Unified, mobile-first stylesheet for all pages
├── appsettings.json        # Application configuration
├── how-the-data-works.md   # Architectural documentation for JSON & DI service
├── MyRazorApp.csproj       # Project file (.NET 10.0, zero external database packages)
└── Program.cs              # Application startup and service registration
```

---

## Data Architecture

For an in-depth explanation of how data is stored, read, and written without an external SQL database, see [how-the-data-works.md](how-the-data-works.md).
