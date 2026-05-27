# CLAUDE.md — Ludero Website

This file provides context for AI assistants (Claude, Copilot, etc.) working on this codebase.

---

## Project Purpose

Marketing website for **Ludero B.V.**, a Dutch Energy Management System (EMS) company. The site is entirely in Dutch (nl). Its primary goal is lead generation through a contact form and factsheet download flow.

---

## Tech Stack

- **ASP.NET Core 8 Razor Pages** — no MVC controllers; each page is `Pages/Pagename.cshtml` + `Pages/Pagename.cshtml.cs`
- **Postmark HTTP API** — email sending via `HttpClient`, no Postmark SDK
- **Markdig** (NuGet) — Markdown-to-HTML rendering for the news section
- **Vanilla CSS** with CSS custom properties (`--green`, `--blue`, `--yellow`, etc.) defined at the top of `wwwroot/css/site.css`
- **Vanilla JavaScript** — only modal open/close and mobile hamburger nav in `wwwroot/js/site.js`
- **No frontend framework** — no React, Vue, Alpine, HTMX, etc.

---

## Key Conventions

### Razor Pages
- All pages live in `src/Ludero.Web/Pages/`
- Use `asp-page` tag helpers for navigation links (not hardcoded `href`)
- Use `[BindProperty]` on page models for form binding
- Use `DataAnnotations` on models for validation (`[Required]`, `[EmailAddress]`, etc.)
- Always include `@Html.AntiForgeryToken()` in POST forms

### Routing
| URL | File |
|-----|------|
| `/` | `Pages/Index.cshtml` |
| `/voor-ondernemers` | `Pages/VoorOndernemers.cshtml` (has `@page "/voor-ondernemers"`) |
| `/voor-energy-hubs` | `Pages/VoorEnergyHubs.cshtml` (has `@page "/voor-energy-hubs"`) |
| `/nieuws` | `Pages/Nieuws/Index.cshtml` — news overview |
| `/nieuws/{slug}` | `Pages/Nieuws/Detail.cshtml` — single article |
| `/contact` | `Pages/Contact.cshtml` |
| POST `/factsheet` | `Pages/Factsheet.cshtml.cs` (no view) |

### Language
All user-facing text is in Dutch. Do not change content to English unless specifically asked. Error messages in models are in Dutch.

---

## Email Service

- **Interface:** `Services/IEmailService.cs`
- **Implementation:** `Services/PostmarkEmailService.cs` — calls `POST https://api.postmarkapp.com/email` with `X-Postmark-Server-Token` header
- **Template rendering:** `Services/RazorViewRenderer.cs` — renders Razor views to HTML string for use as email body
- **Templates:** `Pages/Emails/` — four templates: `ContactConfirmation`, `ContactNotification`, `FactsheetConfirmation`, `FactsheetNotification`
- **Email layout:** `Pages/Emails/_EmailLayout.cshtml` — shared wrapper with inline CSS

---

## Configuration

```json
// appsettings.json (placeholder)
{
  "Postmark": {
    "ApiKey": "POSTMARK_API_KEY_HERE",
    "FromAddress": "noreply@ludero.nl",
    "FromName": "Ludero B.V.",
    "NotificationAddress": "info@ludero.nl"
  }
}
```

Never commit real API keys. Real keys go in `appsettings.Development.json` (gitignored) or as env vars (`Postmark__ApiKey`).

---

## Modals

The four modal overlays (factsheet, cookies, privacy, terms) are in `Pages/Shared/`:
- `_ModalFactsheet.cshtml` — real form that POSTs to `/factsheet`
- `_ModalCookies.cshtml`, `_ModalPrivacy.cshtml`, `_ModalAV.cshtml` — static content only

Open/close is controlled by JS functions `openModal(id)` / `closeModal(id)` in `wwwroot/js/site.js`.

---

## Styling

- All CSS: `wwwroot/css/site.css`
- Brand colors as CSS variables: `--green: #0C614E`, `--blue: #2F799E`, `--yellow: #e8bb5e`
- Responsive breakpoints: 1024px (tablet) and 768px (mobile)
- Do not add inline styles to new elements unless necessary — prefer adding CSS classes

---

## Adding a New Page

1. Create `Pages/NewPage.cshtml` with `@page "/new-page"` directive
2. Create `Pages/NewPage.cshtml.cs` with `public class NewPageModel : PageModel`
3. Add nav link to `Pages/Shared/_Navbar.cshtml`
4. Add footer link if applicable to `Pages/Shared/_Footer.cshtml`

---

## News Section (Nieuws)

The news/blog section is powered by Markdown files — no database required.

### Content files
- **Location:** `src/Ludero.Web/Content/Nieuws/*.md` — not web-accessible; served by `NewsService` via `IWebHostEnvironment.ContentRootPath`
- **Slug:** the filename without extension becomes the URL slug (e.g. `peak-shaving.md` → `/nieuws/peak-shaving`)
- **Sort order:** articles are shown newest-first by `date` frontmatter field

### Adding a new article
Create a `.md` file in `Content/Nieuws/` with this exact frontmatter structure:

```markdown
---
title: Jouw artikeltitel
date: 2026-05-06
description: Korte samenvatting (shown on card and in banner).
image: https://...
---

## Inhoud begint hier
```

All four frontmatter keys are required. Markdown body supports headings, lists, bold, blockquotes, links, etc. (full CommonMark + Markdig advanced extensions).

### Service
- **`Services/NewsService.cs`** — reads and parses `.md` files, exposes `GetAllAsync()` and `GetBySlugAsync(slug)`
- Frontmatter is parsed by a hand-written flat `key: value` parser (no YamlDotNet dependency)
- Markdown body is rendered to HTML by **Markdig** with `UseAdvancedExtensions()`
- No caching — files are read fresh on every request

### Model
- **`Models/NewsArticle.cs`** — `Slug`, `Title`, `Date`, `Description`, `Image`, `ContentHtml`

### CSS classes
- `.nieuws-grid` / `.nieuws-card` — 3-col card grid on the overview page (collapses to 2-col at 1024px, 1-col at 768px)
- `.nieuws-detail-content` — article body typography (h2, h3, p, ul, ol, blockquote with yellow left border)

---

## Common Pitfalls

- Email `@` signs in Razor views must be escaped as `&#64;` in HTML attributes/href (e.g., `href="mailto:info&#64;ludero.nl"`)
- The `RazorViewRenderer` needs the full path to the view: `/Pages/Emails/ViewName.cshtml`
- `Factsheet.cshtml.cs` has no `.cshtml` view — it always redirects after POST
- The solution file is `Ludero.sln` (classic format). If `dotnet new sln` ever regenerates it as `.slnx` (default on .NET 9+ SDK), delete the `.slnx` and recreate with `dotnet new sln --format sln -n Ludero` or write the classic format manually
