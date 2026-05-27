using Ludero.Web.Models;
using Markdig;

namespace Ludero.Web.Services;

public class NewsService
{
    private readonly string _contentPath;

    private static readonly MarkdownPipeline _pipeline =
        new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    public NewsService(IWebHostEnvironment env)
    {
        _contentPath = Path.Combine(env.ContentRootPath, "Content", "Nieuws");
    }

    public async Task<List<NewsArticle>> GetAllAsync()
    {
        if (!Directory.Exists(_contentPath))
            return [];

        var articles = new List<NewsArticle>();

        foreach (var file in Directory.EnumerateFiles(_contentPath, "*.md"))
        {
            var raw = await File.ReadAllTextAsync(file);
            var (meta, _) = ParseFile(raw);
            var slug = Path.GetFileNameWithoutExtension(file);
            articles.Add(ToArticle(slug, meta, ""));
        }

        return [.. articles.OrderByDescending(a => a.Date)];
    }

    public async Task<NewsArticle?> GetBySlugAsync(string slug)
    {
        if (!Directory.Exists(_contentPath))
            return null;

        var file = Path.Combine(_contentPath, slug + ".md");
        if (!File.Exists(file))
            return null;

        var raw = await File.ReadAllTextAsync(file);
        var (meta, body) = ParseFile(raw);
        var html = Markdown.ToHtml(body, _pipeline);
        return ToArticle(slug, meta, html);
    }

    private static (Dictionary<string, string> meta, string body) ParseFile(string raw)
    {
        var meta = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Normalise line endings
        raw = raw.Replace("\r\n", "\n");

        if (!raw.StartsWith("---\n"))
            return (meta, raw);

        var secondDelimiter = raw.IndexOf("\n---\n", 4);
        if (secondDelimiter < 0)
            return (meta, raw);

        var frontmatter = raw[4..secondDelimiter];
        var body = raw[(secondDelimiter + 5)..].TrimStart('\n');

        foreach (var line in frontmatter.Split('\n'))
        {
            var colon = line.IndexOf(':');
            if (colon < 1) continue;
            var key = line[..colon].Trim();
            var value = line[(colon + 1)..].Trim();
            meta[key] = value;
        }

        return (meta, body);
    }

    private static NewsArticle ToArticle(string slug, Dictionary<string, string> meta, string htmlContent)
    {
        meta.TryGetValue("title", out var title);
        meta.TryGetValue("description", out var description);
        meta.TryGetValue("image", out var image);

        DateOnly date = DateOnly.MinValue;
        if (meta.TryGetValue("date", out var dateStr))
            DateOnly.TryParse(dateStr, out date);

        return new NewsArticle
        {
            Slug        = slug,
            Title       = title ?? "",
            Date        = date,
            Description = description ?? "",
            Image       = image ?? "",
            ContentHtml = htmlContent,
        };
    }
}
