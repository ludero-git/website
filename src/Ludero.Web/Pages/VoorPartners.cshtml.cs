using System.Text.Json;
using Ixnas.AltchaNet;
using Ludero.Web.Models;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ludero.Web.Pages;

public class VoorPartnersModel : PageModel
{
    private readonly IEmailService _emailService;
    private readonly AltchaService _altchaService;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public VoorPartnersModel(IEmailService emailService, AltchaService altchaService)
    {
        _emailService = emailService;
        _altchaService = altchaService;
    }

    [BindProperty]
    public PartnerFormModel Form { get; set; } = new();

    [BindProperty(Name = "altcha")]
    public string? AltchaPayload { get; set; }

    public string? AltchaChallenge { get; set; }

    [BindProperty]
    public string? WorkEmail { get; set; }

    [BindProperty]
    public long RenderTimestamp { get; set; }

    public bool Success { get; set; }

    public void OnGet()
    {
        Success = Request.Query["success"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
        PrepareSpamProtection();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(WorkEmail) || DateTimeOffset.UtcNow.ToUnixTimeSeconds() - RenderTimestamp < 3)
        {
            return RedirectToPage("/VoorPartner", new { success = true });
        }

        if (!ModelState.IsValid)
        {
            PrepareSpamProtection();
            return Page();
        }

        var altchaResult = _altchaService.Validate(AltchaPayload);
        if (!altchaResult.IsCompletedSuccessfully)
        {
            ModelState.AddModelError(string.Empty, "Spamcontrole mislukt. Probeer het opnieuw.");
            PrepareSpamProtection();
            return Page();
        }

        var contact = new ContactFormModel
        {
            Name = Form.Name,
            Company = Form.Company,
            Email = Form.Email,
            Phone = Form.Phone,
            Subject = "Partnerschap met Ludero",
            Message = $"Partneraanvraag\nFocusgebieden: {string.Join(", ", Form.FocusAreas)}"
        };

        await _emailService.SendContactConfirmationAsync(contact);
        await _emailService.SendContactNotificationAsync(contact);

        return RedirectToPage("/VoorPartner", new { success = true });
    }

    private void PrepareSpamProtection()
    {
        AltchaChallenge = JsonSerializer.Serialize(_altchaService.Generate(), _jsonOptions);
        RenderTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
