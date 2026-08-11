namespace Ludero.Web.Models.Outline;

public class OutlinePage
{
    public string Path { get; init; } = "";
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public string UrlId { get; init; } = "";
    public DateTime? LastUpdated { get; set; }
    public string? ContentHtml { get; set; }
}