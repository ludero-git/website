namespace Ludero.Web.Models.Outline;

public class OutlineWebhookRequest
{
    public string? Event { get; set; }
    public OutlineWebhookPayload? Payload { get; set; }
}

public class OutlineWebhookPayload
{
    public OutlineWebhookDocument? Model { get; set; }
}

public class OutlineWebhookDocument
{
    public string? Id { get; set; }
    public string? UrlId { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public DateTime? UpdatedAt { get; set; }
}