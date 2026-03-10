using Ludero.Web.Models;

namespace Ludero.Web.Services;

public interface IEmailService
{
    Task SendContactConfirmationAsync(ContactFormModel model);
    Task SendContactNotificationAsync(ContactFormModel model);
    Task SendFactsheetConfirmationAsync(FactsheetFormModel model);
    Task SendFactsheetNotificationAsync(FactsheetFormModel model);
}
