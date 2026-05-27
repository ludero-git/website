namespace Ludero.Web.Models;

public class NewsArticle
{
    public string Slug        { get; init; } = "";
    public string Title       { get; init; } = "";
    public DateOnly Date      { get; init; }
    public string Description { get; init; } = "";
    public string Image       { get; init; } = "";
    public string ContentHtml { get; init; } = "";
}
