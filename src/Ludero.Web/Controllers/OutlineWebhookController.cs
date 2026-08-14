using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ludero.Web.Models.Outline;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ludero.Web.Controllers;

[ApiController]
[Route("api/outline/webhook")]
public class OutlineWebhookController : ControllerBase
{
    private readonly OutlineService _outlineService;
    private readonly ILogger<OutlineWebhookController> _logger;
    private readonly string _signingSecret;

    public OutlineWebhookController(
        OutlineService outlineService,
        ILogger<OutlineWebhookController> logger,
        IConfiguration configuration)
    {
        _outlineService = outlineService;
        _logger = logger;

        _signingSecret = configuration["Outline:SigningSecret"]
            ?? throw new InvalidOperationException(
                "Outline:SigningSecret is not configured.");
    }

    [HttpPost]
    public async Task<IActionResult> Receive()
    {
        // Check whether the signature header exists.
        if (!Request.Headers.TryGetValue(
                "Outline-Signature",
                out var signatureHeader))
        {
            _logger.LogWarning(
                "Outline webhook received without Outline-Signature header.");

            return Unauthorized();
        }

        // Read raw request body.
        using var reader = new StreamReader(
            Request.Body,
            Encoding.UTF8);

        var body = await reader.ReadToEndAsync();

        // Verify signature.
        if (!VerifySignature(
                signatureHeader.ToString(),
                body,
                _signingSecret))
        {
            _logger.LogWarning(
                "Outline webhook received with invalid signature.");

            return Unauthorized();
        }

        // Deserialize after signature verification.
        OutlineWebhookRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<OutlineWebhookRequest>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(
                ex,
                "Outline webhook contained invalid JSON.");

            return BadRequest();
        }

        if (request is null)
            return BadRequest();

        // We only care about document updates.
        if (!string.Equals(
                request.Event,
                "documents.update",
                StringComparison.OrdinalIgnoreCase))
        {
            return Ok();
        }

        var document = request.Payload?.Model;

        if (document is null ||
            string.IsNullOrWhiteSpace(document.UrlId) ||
            document.Text is null)
        {
            _logger.LogWarning(
                "Invalid Outline document update webhook.");

            return BadRequest();
        }

        var updated = await _outlineService.UpdatePageAsync(
            document.UrlId,
            document.Title,
            document.Text,
            document.UpdatedAt);

        if (!updated)
        {
            _logger.LogInformation(
                "Ignoring Outline document {UrlId}: page is not configured.",
                document.UrlId);

            return Ok();
        }

        _logger.LogInformation(
            "Updated Outline page {UrlId} from webhook. UpdatedAt: {UpdatedAt}",
            document.UrlId,
            document.UpdatedAt);

        return Ok();
    }

    private static bool VerifySignature(
        string header,
        string body,
        string secret)
    {
        // According to https://docs.getoutline.com/s/guide/doc/webhooks-gB7HYhS6yq#h-verifying-requests.
        if (string.IsNullOrWhiteSpace(header))
            return false;

        string? timestamp = null;
        string? providedSignature = null;

        foreach (var part in header.Split(','))
        {
            var pieces = part.Split('=', 2);

            if (pieces.Length != 2)
                continue;

            var key = pieces[0].Trim();
            var value = pieces[1].Trim();

            if (key.Equals("t", StringComparison.OrdinalIgnoreCase))
                timestamp = value;

            if (key.Equals("s", StringComparison.OrdinalIgnoreCase) ||
            key.Equals("v1", StringComparison.OrdinalIgnoreCase))
            {
                providedSignature = value;
            }
        }

        if (string.IsNullOrWhiteSpace(timestamp) ||
            string.IsNullOrWhiteSpace(providedSignature))
        {
            return false;
        }

        var payload = $"{timestamp}.{body}";

        using var hmac = new HMACSHA256(
            Encoding.UTF8.GetBytes(secret));

        var calculatedHash = hmac.ComputeHash(
            Encoding.UTF8.GetBytes(payload));

        byte[] providedHash;

        try
        {
            providedHash = Convert.FromHexString(providedSignature);
        }
        catch (FormatException)
        {
            return false;
        }

        // Constant-time comparison.
        return CryptographicOperations.FixedTimeEquals(
            calculatedHash,
            providedHash);
    }
}