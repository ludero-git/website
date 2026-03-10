using Ludero.Web.Models;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ludero.Web.Pages;

public class ContactModel : PageModel
{
    private readonly IEmailService _emailService;

    public ContactModel(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [BindProperty]
    public ContactFormModel Form { get; set; } = new();

    public bool Success { get; set; }

    public void OnGet()
    {
        Success = Request.Query["success"] == "true";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _emailService.SendContactConfirmationAsync(Form);
        await _emailService.SendContactNotificationAsync(Form);

        return RedirectToPage("/Contact", new { success = true });
    }
}
