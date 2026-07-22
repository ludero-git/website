using Ixnas.AltchaNet;
using Ludero.Web.Models;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Ludero.Web.Pages;

public class ContactModel : PageModel
{
    private readonly IEmailService _emailService;
    private readonly AltchaService _altchaService;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(){PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

    public ContactModel(IEmailService emailService, AltchaService altchaService)
    {
        _emailService = emailService;
        _altchaService = altchaService;
    }

    [BindProperty]
    public ContactFormModel Form { get; set; } = new();

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
        Success = Request.Query["success"].ToString().ToLower() == "true";
        PrepareSpamProtection();
    }
    private void PrepareSpamProtection()
    {
        AltchaChallenge = JsonSerializer.Serialize(_altchaService.Generate(), _jsonOptions);
        RenderTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(WorkEmail))
        {
            return RedirectToPage("/Contact", new { success = true });
        }
        var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (currentTimestamp - RenderTimestamp < 3)
        {
            return RedirectToPage("/Contact", new { success = true });
        }

        if (!ModelState.IsValid)
        {
            PrepareSpamProtection();
            return Page();
        }
        var altchaResult = _altchaService.Validate(AltchaPayload);
        if (!altchaResult.IsCompletedSuccessfully)
        {
            ModelState.AddModelError(string.Empty, "Spam check failed. Please try again.");
            PrepareSpamProtection();
            return Page();
        }

        await _emailService.SendContactConfirmationAsync(Form);
        await _emailService.SendContactNotificationAsync(Form);

        return RedirectToPage("/Contact", new { success = true });
    }
}
