using System.Text;
using System.Text.Json;
using Ludero.Web.Models;
using Microsoft.Extensions.Options;

namespace Ludero.Web.Services;

public class PostmarkEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly PostmarkOptions _options;
    private readonly RazorViewRenderer _renderer;
    private readonly ILogger<PostmarkEmailService> _logger;

    private const string PostmarkApiUrl = "https://api.postmarkapp.com/email";

    public PostmarkEmailService(
        HttpClient httpClient,
        IOptions<PostmarkOptions> options,
        RazorViewRenderer renderer,
        ILogger<PostmarkEmailService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _renderer = renderer;
        _logger = logger;

        _httpClient.DefaultRequestHeaders.Add("X-Postmark-Server-Token", _options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task SendContactConfirmationAsync(ContactFormModel model)
    {
        var body = await _renderer.RenderViewToStringAsync("/Pages/Emails/ContactConfirmation.cshtml", model);
        await SendEmailAsync(
            to: model.Email,
            toName: model.Name,
            subject: "Bedankt voor uw bericht – Ludero B.V.",
            htmlBody: body
        );
    }

    public async Task SendContactNotificationAsync(ContactFormModel model)
    {
        var body = await _renderer.RenderViewToStringAsync("/Pages/Emails/ContactNotification.cshtml", model);
        await SendEmailAsync(
            to: _options.NotificationAddress,
            toName: "Ludero",
            subject: $"Nieuw contactverzoek van {model.Name}",
            htmlBody: body
        );
    }

    public async Task SendFactsheetConfirmationAsync(FactsheetFormModel model)
    {
        var body = await _renderer.RenderViewToStringAsync("/Pages/Emails/FactsheetConfirmation.cshtml", model);
        await SendEmailAsync(
            to: model.Email,
            toName: model.Name,
            subject: "Uw factsheet van Ludero B.V.",
            htmlBody: body
        );
    }

    public async Task SendFactsheetNotificationAsync(FactsheetFormModel model)
    {
        var body = await _renderer.RenderViewToStringAsync("/Pages/Emails/FactsheetNotification.cshtml", model);
        await SendEmailAsync(
            to: _options.NotificationAddress,
            toName: "Ludero",
            subject: $"Factsheet aanvraag van {model.Name}",
            htmlBody: body
        );
    }

    private async Task SendEmailAsync(string to, string toName, string subject, string htmlBody)
    {
        var payload = new
        {
            From = $"{_options.FromName} <{_options.FromAddress}>",
            To = $"{toName} <{to}>",
            Subject = subject,
            HtmlBody = htmlBody,
            MessageStream = "outbound"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(PostmarkApiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Postmark API returned {StatusCode}: {Body}", response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
        }
    }
}
