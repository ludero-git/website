namespace Ludero.Web.Models;

public class NewsArticle
{
    public string Slug        { get; init; } = "";
    public string Title       { get; init; } = "";
    public DateOnly Date      { get; init; }
    public string Description { get; init; } = "";
    public string Image       { get; init; } = "";
    public List<string> ImageExts   { get; init; } = new();
    public List<int> ImageWidths    { get; init; } = new();
    public string ContentHtml { get; init; } = "";
}
