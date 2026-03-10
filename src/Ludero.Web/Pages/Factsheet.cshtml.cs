using Ludero.Web.Models;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ludero.Web.Pages;

public class FactsheetModel : PageModel
{
    private readonly IEmailService _emailService;

    public FactsheetModel(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [BindProperty]
    public FactsheetFormModel Form { get; set; } = new();

    public IActionResult OnGet()
    {
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        await _emailService.SendFactsheetConfirmationAsync(Form);
        await _emailService.SendFactsheetNotificationAsync(Form);

        var referer = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referer))
        {
            return RedirectToPage("/Index", new { factsheet = true });
        }

        // Add query param to referer to show success state
        var separator = referer.Contains('?') ? "&" : "?";
        return Redirect(referer + separator + "factsheet=true");
    }
}
