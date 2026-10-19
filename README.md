# Student Guide: HTTP Methods & Routing in ASP.NET Core Razor Pages

Welcome! As a web developer, understanding how browsers talk to web servers is one of the most essential skills you will learn. Every time you click a link, search for a product, or submit a form, your browser issues an HTTP request.

This repository is designed as your **hands-on interactive sandbox**. Through four focused demonstrations, you will explore and experiment with:
1. **HTTP GET & Query Strings**: How search filters are encoded into URLs.
2. **Route Parameters vs Query Strings**: When to use path segments vs query strings.
3. **Multiple Route Parameters**: How nested paths map to multiple route segments.
4. **HTTP POST & the PRG Pattern**: How form data is sent safely and how to prevent duplicate submissions.

---

## Getting Started

### Prerequisites
Make sure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed (.NET 10.0 or .NET 8+). You can verify this in your terminal:
```bash
dotnet --version
```

### Running Your Lab Locally
1. Clone this repository and open a terminal inside the project directory:
   ```bash
   cd webdev-razor-http-methods
   ```
2. Start the local development web server:
   ```bash
   dotnet run --launch-profile http
   ```
3. Open your browser and navigate to:
   ```
   http://localhost:5110
   ```
4. You will see the **HTTP & Routing Educational Demos Hub**. From here, you can click into each demo or use the top navigation bar.

---

## Tour of the Four Demos

---

### Demo 1: HTTP GET & Query Strings (`/GetDemo`)

> **Key Takeaway**: Use HTTP GET for retrieving data. GET requests put all parameters directly into the URL query string, making your search results bookmarkable, shareable, and safe to refresh.

* **URL to visit**: `http://localhost:5110/GetDemo`
* **Source code to inspect**: `Pages/GetDemo.cshtml` & `Pages/GetDemo.cshtml.cs`

#### The Core Concept
When you submit an HTML form with `method="get"`, the browser collects all the input names and values and appends them to the end of the URL after a `?`, separated by `&`:
```
/GetDemo?Subject=Computer+Science&Level=Beginner&Keyword=web
```
Because the entire filter state is stored in the URL, GET requests are:
* **Bookmarkable & Shareable**: You can copy the URL and send it to a friend; they will see the exact same filtered results.
* **Idempotent**: Requesting the page multiple times never modifies any data on the server.
* **Cached & Logged**: Browsers, proxies, and web servers can safely cache GET requests.

#### Things to Try (Experiments to Run)
1. **Filter by Category**: Select *Computer Science* and click **Filter Courses (GET)**. Look at the browser's address bar. Notice how `?Subject=Computer+Science` appeared.
2. **Look at the HTTP Inspector**: Scroll down to the black terminal box on the page. Notice how `Request.Query["Subject"]` extracted the value automatically.
3. **Copy the URL into a New Tab**: Open a new browser tab and paste your filtered URL. Notice how the page loads already filtered without needing to click anything.
4. **Try Keyword Search**: Type `SQL` in the keyword box and press Enter. Notice how multiple parameters are joined with `&`: `?Subject=...&Keyword=SQL`.

#### Behind the Scenes in C#
In Razor Pages, you automatically bind query string parameters to C# properties using the `[BindProperty(SupportsGet = true)]` attribute:
```csharp
[BindProperty(SupportsGet = true)]
public string? Subject { get; set; }

[BindProperty(SupportsGet = true)]
public string? Keyword { get; set; }
```
When `OnGet()` executes, ASP.NET Core automatically reads matching keys from the query string and populates these properties before your code runs!

---

### Demo 2: Single Route Parameter vs Query Strings (`/RouteDemo`)

> **Key Takeaway**: Use **Route Parameters** to locate a specific resource (e.g. Employee #101). Use **Query Strings** to modify how that resource is viewed (e.g. which tab to display).

* **URL to visit**: `http://localhost:5110/RouteDemo`
* **Source code to inspect**: `Pages/RouteDemo.cshtml` & `Pages/RouteDemo.cshtml.cs`

#### The Core Concept
Web developers often wonder: *Should I put data in the URL path or in a query string?* This demo teaches the architectural rule of thumb:
* **Route Parameter** (`/RouteDemo/101`): Answers **WHAT** resource you want to view. Think of it like a primary key or unique identifier.
* **Query String** (`?tab=compensation`): Answers **HOW** you want to view that resource (e.g. view options, tabs, sorting, pagination).

#### Things to Try (Experiments to Run)
1. **Click Staff Member Buttons**: Click *ID 101: Alice Morgan*, then *ID 102: Bob Chen*. Notice how the URL path changes (`/RouteDemo/101` $\rightarrow$ `/RouteDemo/102`).
2. **Click the Tab Modifiers**: While viewing Alice Morgan, click **Tab: Compensation**. Notice how the URL becomes:
   ```
   /RouteDemo/101?tab=compensation
   ```
   The `101` is part of the path (the resource), while `?tab=compensation` is a query string (the view option).
3. **Inspect the Inspector**: Check the green and purple output in the inspector box. See how ASP.NET Core splits the URL between `RouteData.Values["id"]` and `Model.Tab`.

#### Behind the Scenes in C#
At the top of `Pages/RouteDemo.cshtml`, the `@page` directive defines the route template:
```csharp
@page "{id:int?}"
```
The `{id:int?}` syntax tells ASP.NET Core:
* There is an optional route parameter called `id`.
* The `:int` inline constraint ensures it only matches numeric digits.
* In `RouteDemo.cshtml.cs`, `public void OnGet(int? id)` receives the value directly as a parameter.

---

### Demo 3: Multiple Route Parameters (`/MultiRouteDemo`)

> **Key Takeaway**: You can nest multiple route parameters in a URL path to express clean, hierarchical relationships like `/department/employeeId`.

* **URL to visit**: `http://localhost:5110/MultiRouteDemo`
* **Source code to inspect**: `Pages/MultiRouteDemo.cshtml` & `Pages/MultiRouteDemo.cshtml.cs`

#### The Core Concept
Real-world web APIs and websites often have hierarchical relationships:
* An e-commerce store: `/products/electronics/laptops/42`
* A university portal: `/courses/computing/cs101`
* Our staff directory: `/MultiRouteDemo/engineering/101`

ASP.NET Core allows you to define multiple path segments in a single `@page` directive.

#### Things to Try (Experiments to Run)
1. **Select a Department**: Click `/design`. Notice the URL becomes `/MultiRouteDemo/design`. The list of staff members dynamically updates to only show designers.
2. **Select an Employee**: Click *ID 102: Bob Chen*. The URL becomes:
   ```
   /MultiRouteDemo/design/102
   ```
3. **Switch Tabs**: Click `?tab=compensation`. The final URL combines both route segments with a query modifier:
   ```
   /MultiRouteDemo/design/102?tab=compensation
   ```
4. **Look at the Tag Helpers**: Open `Pages/MultiRouteDemo.cshtml` in your editor. Notice how the links are written:
   ```html
   <a asp-page="/MultiRouteDemo"
      asp-route-department="@dept.Slug"
      asp-route-id="@staff.Id"
      asp-route-tab="compensation">
   ```
   ASP.NET Core is smart: `department` and `id` match the route tokens in `@page "{department?}/{id:int?}"`, so they become path segments. `tab` does *not* match, so ASP.NET Core automatically appends it as a query string!

---

### Demo 4: HTTP POST & the Post-Redirect-Get (PRG) Pattern (`/PostDemo`)

> **Key Takeaway**: Always use HTTP POST when submitting form data that modifies state. Always follow a successful POST with a redirect to a GET endpoint (the PRG pattern) to protect against duplicate submissions.

* **URL to visit**: `http://localhost:5110/PostDemo`
* **Source code to inspect**: `Pages/PostDemo.cshtml` & `Pages/PostDemo.cshtml.cs`

#### The Core Concept
Unlike GET, an HTTP POST request does not put data into the URL. Instead, form fields are packed inside the **HTTP Request Body**.

* **Why use POST for forms?**
  * Passwords, emails, and credit card numbers are hidden from the URL bar, bookmarks, and server access logs.
  * Payloads have no arbitrary length limit (essential for file uploads and long essays).
  * Indicates to the browser and server that data is being created or mutated (**non-idempotent**).

* **What is the Post-Redirect-Get (PRG) Pattern?**
  Have you ever submitted a form online, hit **Refresh (F5)**, and seen a scary browser popup saying:
  > *"Confirm Form Resubmission: Are you sure you want to resubmit this form?"*

  If you hit "Yes", your credit card might be charged twice! The PRG pattern completely prevents this bug:
  1. **POST**: Your browser submits the form data to the server.
  2. **REDIRECT**: The server saves the record to `Data/feedback.json` and responds with an HTTP `302 Redirect` back to `/PostDemo`.
  3. **GET**: Your browser automatically follows the redirect and issues a clean `GET` request.

#### Things to Try (Experiments to Run)
1. **Submit Feedback**: Fill out the form with your name, email, category, and a comment. Click **Submit Feedback via HTTP POST**.
2. **Observe the Green Success Alert**: Notice that the page redirected and displays your submission in the green flash box and in the persisted JSON table below.
3. **Press F5 (Refresh)**: Refresh your browser right now. Notice what happens: **No warning popup appears!** Because the current request is a clean GET request, refreshing simply reloads the page safely.
4. **Check the JSON File**: Open `Data/feedback.json` in your editor. Notice your new feedback was appended to the file!

---

## Quick Reference: HTTP GET vs HTTP POST

| Question to Ask Yourself | HTTP GET | HTTP POST |
| :--- | :--- | :--- |
| **What is my goal?** | Read or search existing data | Create, update, or delete data |
| **Where does data travel?** | In the URL query string (`?key=val`) | Hidden inside the HTTP Request Body |
| **Can users bookmark the result?** | **Yes** (all inputs are in the URL) | **No** (data is not in the URL) |
| **Is it safe to repeat (Idempotent)?** | **Yes** (repeating does not change state) | **No** (repeating may duplicate records) |
| **What happens on browser refresh?** | Cleanly reloads the same search | Harmless with PRG; dangerous without PRG |
| **How does this app store data?** | Reads from `Data/courses.json` | Writes new records to `Data/feedback.json` |

---

## Project Structure at a Glance

```
webdev-razor-http-methods/
├── Data/
│   ├── courses.json        # Course records read by Demo 1 (GET)
│   ├── feedback.json       # Submissions stored by Demo 4 (POST)
│   └── staff.json          # Entity records used by Demos 2 & 3 (Routing)
├── Models/
│   ├── Course.cs           # C# record for courses
│   ├── FeedbackItem.cs     # C# record for feedback
│   └── StaffMember.cs      # C# record for staff members
├── Pages/
│   ├── Shared/
│   │   └── _Layout.cshtml  # Global master navigation & layout
│   ├── Index.cshtml        # Home hub introducing the four demos
│   ├── GetDemo.cshtml      # Demo 1: GET and Query Strings
│   ├── RouteDemo.cshtml    # Demo 2: Single Route Parameter vs Query Strings
│   ├── MultiRouteDemo.cshtml # Demo 3: Multiple Route Parameters
│   └── PostDemo.cshtml     # Demo 4: POST & Post-Redirect-Get (PRG)
├── Services/
│   └── JsonDataService.cs  # Centralized service reading & writing JSON files
├── wwwroot/
│   └── css/
│       └── site.css        # Mobile-first stylesheet for all pages
├── how-the-data-works.md   # Student guide explaining the JSON service architecture
└── Program.cs              # Web application startup & dependency injection setup
```

---

## Where to Go Next

To understand how the data is loaded, how `JsonDataService` works, and how dependency injection functions in ASP.NET Core, check out the companion student guide:
👉 **[Read: How the Data Works (JSON & Services Guide)](how-the-data-works.md)**
