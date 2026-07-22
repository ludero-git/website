using Ixnas.AltchaNet;
using Ludero.Web.Models;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ludero.Web.Pages;

public class FactsheetModel : PageModel
{
    private readonly IEmailService _emailService; 
    private readonly AltchaService _altchaService;

    public FactsheetModel(IEmailService emailService, AltchaService altchaService)
    {
        _emailService = emailService;
        _altchaService = altchaService;
    }

    [BindProperty]
    public FactsheetFormModel Form { get; set; } = new(); 

    [BindProperty(Name = "altcha")]
    public string? AltchaPayload { get; set; }

    [BindProperty]
    public string? WorkEmail { get; set; }

    [BindProperty]
    public long RenderTimestamp { get; set; }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(WorkEmail))
        {
            return RedirectToSuccess(Request.Headers["Referer"].ToString());
        }
        var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (currentTimestamp - RenderTimestamp < 3)
        {
            return RedirectToSuccess(Request.Headers["Referer"].ToString());
        }
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var altchaResult = _altchaService.Validate(AltchaPayload);
        if (!altchaResult.IsCompletedSuccessfully)
        {
            return BadRequest();
        }

        await _emailService.SendFactsheetConfirmationAsync(Form);
        await _emailService.SendFactsheetNotificationAsync(Form);

        return RedirectToSuccess(Request.Headers["Referer"].ToString());
    }
    private IActionResult RedirectToSuccess(string referer)
    {
        if (string.IsNullOrEmpty(referer))
        {
            return RedirectToPage("/Index", new { factsheet = true });
        }

        var separator = referer.Contains('?') ? "&" : "?";
        return Redirect(referer + separator + "factsheet=true");
    }
}
