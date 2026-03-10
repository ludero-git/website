# Ludero Website

Marketing website for **Ludero B.V.** — an Energy Management System (EMS) company based in Hendrik-Ido-Ambacht, the Netherlands.

Built with **ASP.NET Core 8 Razor Pages** and **Postmark** for transactional email.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 8 Razor Pages |
| Language | C# 12, Dutch (nl) content |
| Email | Postmark HTTP API (no SDK) |
| Styling | Vanilla CSS (CSS custom properties) |
| Fonts | Fraunces + Plus Jakarta Sans (Google Fonts) |
| JavaScript | Vanilla JS (modals, mobile nav) |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Visual Studio 2022+ or VS Code with C# extension
- A [Postmark](https://postmarkapp.com) account and server API key (for email sending)

---

## Project Structure

```
src/Ludero.Web/
├── Pages/
│   ├── Shared/          # Layout, navbar, footer, modals
│   ├── Emails/          # HTML email templates (Razor)
│   ├── Index.cshtml          → /
│   ├── VoorOndernemers.cshtml → /voor-ondernemers
│   ├── VoorEnergyHubs.cshtml  → /voor-energy-hubs
│   ├── Contact.cshtml         → /contact
│   └── Factsheet.cshtml.cs    → POST /factsheet
├── Models/              # Form view models
├── Services/            # IEmailService + PostmarkEmailService
└── wwwroot/
    ├── css/site.css     # All styles
    ├── js/site.js       # Modals + mobile nav
    └── images/          # Logo assets
```

---

## Setup

### 1. Clone the repository

```bash
git clone <repo-url>
cd website-ludero
```

### 2. Configure Postmark

Create `src/Ludero.Web/appsettings.Development.json` (already gitignored) with your Postmark credentials:

```json
{
  "Postmark": {
    "ApiKey": "your-postmark-server-api-key-here",
    "FromAddress": "noreply@ludero.nl",
    "FromName": "Ludero B.V.",
    "NotificationAddress": "info@ludero.nl"
  }
}
```

> **Note:** The `FromAddress` must be a verified sender in your Postmark account.
> For local development, you can use [Postmark's sandbox mode](https://postmarkapp.com/support/article/1207-sandbox-mode).

### 3. Run locally

```bash
dotnet run --project src/Ludero.Web
```

Or open `Ludero.slnx` in Visual Studio and press **F5**.

The site starts at `https://localhost:5001` (or `http://localhost:5000`).

---

## Pages

| URL | Page |
|-----|------|
| `/` | Home — hero, problem statement, feature cards |
| `/voor-ondernemers` | Voor Ondernemers — 7 EMS benefits + factsheet download |
| `/voor-energy-hubs` | Voor Energy Hubs — collective energy management features |
| `/contact` | Contact — contact form + company info |

---

## Email Flow

| Trigger | Emails sent |
|---------|------------|
| Contact form submit | Confirmation to visitor + notification to `info@ludero.nl` |
| Factsheet modal submit | Confirmation to visitor + notification to `info@ludero.nl` |

Email templates are Razor views in `src/Ludero.Web/Pages/Emails/` and use inline CSS for email client compatibility.

---

## Deployment

For production, set the Postmark API key as an environment variable instead of in `appsettings.json`:

```bash
# Linux/macOS
export Postmark__ApiKey=your-key-here

# Windows (PowerShell)
$env:Postmark__ApiKey = "your-key-here"
```

Or use your hosting platform's secret management (Azure App Service → Configuration, etc.).

Build for production:

```bash
dotnet publish src/Ludero.Web -c Release -o ./publish
```

---

## Customization

- **Content:** Edit the `.cshtml` files in `src/Ludero.Web/Pages/`
- **Styles:** All CSS is in `src/Ludero.Web/wwwroot/css/site.css` with CSS variables at the top
- **Email templates:** Edit `src/Ludero.Web/Pages/Emails/` — use inline styles for maximum email client compatibility
- **Adding a page:** Add `NewPage.cshtml` + `NewPage.cshtml.cs` to `Pages/`, then add a nav link in `Pages/Shared/_Navbar.cshtml`
