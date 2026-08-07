using Ludero.Web.Models;

namespace Ludero.Web.Services;

public static class ImageSrcSetHelper
{
public static (string WebpSrcSet, string FallbackSrcSet, string FallbackSrc, string FallbackType, string PlaceholderSrc, string PlaceholderWebpSrc) Build(
        NewsArticle article)
    {
        if (string.IsNullOrEmpty(article.Image) || article.ImageWidths.Count == 0)
        {
            var mimeType = GetMimeType(article.Image);

            return ("", "", article.Image, mimeType, article.Image, "");
        }

        var directory = Path.GetDirectoryName(article.Image);
        directory = directory?.Replace('\\', '/') ?? "";

        var fileName = Path.GetFileNameWithoutExtension(article.Image);

        var widths = article.ImageWidths
            .OrderBy(width => width)
            .ToList();

        var webpExtension = GetWebpExtension(article);
        var fallbackExtension = GetFallbackExtension(article);

        var webpSrcSet = "";
        if (webpExtension != null)
            webpSrcSet = CreateSrcSet(directory, fileName, widths, webpExtension);

        var fallbackSrcSet = CreateSrcSet(directory, fileName, widths, fallbackExtension);

        var largestWidth = widths.Last();
        var fallbackSrc = $"{directory}/{fileName}-{largestWidth}.{fallbackExtension}";
        var fallbackType = GetMimeType(fallbackExtension);

        // Use smallest image as placeholder (progressive loading)
        var smallestWidth = widths.First();
        var placeholderSrc = $"{directory}/{fileName}-{smallestWidth}.{fallbackExtension}";
        var placeholderWebpSrc = webpExtension != null
            ? $"{directory}/{fileName}-{smallestWidth}.{webpExtension}"
            : "";

        return (
            webpSrcSet,
            fallbackSrcSet,
            fallbackSrc,
            fallbackType,
            placeholderSrc,
            placeholderWebpSrc
        );
    }

    private static string CreateSrcSet(
        string directory,
        string fileName,
        List<int> widths,
        string extension)
    {
        var sources = new List<string>();

        foreach (var width in widths)
        {
            var source = $"{directory}/{fileName}-{width}.{extension} {width}w";
            sources.Add(source);
        }

        // 1.jpg 500w, 2.jpg 1000w, 3.jpg 2000w
        return string.Join(", ", sources);
    }

    private static string? GetWebpExtension(NewsArticle article)
    {
        foreach (var extension in article.ImageExts)
        {
            if (extension.Equals("webp", StringComparison.OrdinalIgnoreCase))
                return extension;
        }

        return null;
    }

    private static string GetFallbackExtension(NewsArticle article)
    {
        foreach (var extension in article.ImageExts)
        {
            if (!extension.Equals("webp", StringComparison.OrdinalIgnoreCase))
                return extension;
        }

        return Path.GetExtension(article.Image).TrimStart('.');
    }

    private static string GetMimeType(string extensionOrPath)
    {
        var extension = Path.GetExtension(extensionOrPath)
            .TrimStart('.')
            .ToLowerInvariant();

        if (string.IsNullOrEmpty(extension))
            extension = extensionOrPath.ToLowerInvariant();

        return extension switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "webp" => "image/webp",
            _ => $"image/{extension}"
        };
    }
}