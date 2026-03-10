# CLAUDE.md — Ludero Website

This file provides context for AI assistants (Claude, Copilot, etc.) working on this codebase.

---

## Project Purpose

Marketing website for **Ludero B.V.**, a Dutch Energy Management System (EMS) company. The site is entirely in Dutch (nl). Its primary goal is lead generation through a contact form and factsheet download flow.

---

## Tech Stack

- **ASP.NET Core 8 Razor Pages** — no MVC controllers; each page is `Pages/Pagename.cshtml` + `Pages/Pagename.cshtml.cs`
- **Postmark HTTP API** — email sending via `HttpClient`, no Postmark SDK
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

## Common Pitfalls

- Email `@` signs in Razor views must be escaped as `&#64;` in HTML attributes/href (e.g., `href="mailto:info&#64;ludero.nl"`)
- The `RazorViewRenderer` needs the full path to the view: `/Pages/Emails/ViewName.cshtml`
- `Factsheet.cshtml.cs` has no `.cshtml` view — it always redirects after POST
- The `.slnx` extension is the new solution format (Visual Studio 2022 17.x+); if you need the classic `.sln`, run `dotnet sln migrate Ludero.slnx`
