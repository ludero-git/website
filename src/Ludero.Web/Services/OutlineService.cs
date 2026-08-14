using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Ludero.Web.Models.Outline;
using Markdig;

namespace Ludero.Web.Services;

public class OutlineService
{
    private readonly Dictionary<string, OutlinePage> _outlinePages;
    private readonly SemaphoreSlim _fetchLock = new(1, 1);
    private readonly HttpClient _httpClient;
    private bool _hasFetched;

    private static readonly MarkdownPipeline _pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseCustomContainers()
            .Build();

    public OutlineService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        var pages = config.GetSection("Outline:Pages").Get<List<OutlinePage>>() ?? [];

        _outlinePages = pages.ToDictionary(
            page => page.Path,
            StringComparer.OrdinalIgnoreCase);

        _httpClient = httpClientFactory.CreateClient("OutlineApi");
    }

    private async Task FetchAllAsync()
    {
        if (_hasFetched)
            return;

        await _fetchLock.WaitAsync();

        try
        {
            if (_hasFetched)
                return;

            foreach (var page in _outlinePages.Values)
            {
                var result = await FetchPageContentAsync(page.UrlId);

                // Clean Outline markdown before converting to HTML.
                var markdown = CleanMarkdown(result.Content);

                page.ContentHtml = Markdown.ToHtml(
                    markdown,
                    _pipeline);

                page.LastUpdated = result.LastUpdated;
            }

            _hasFetched = true;
        }
        finally
        {
            _fetchLock.Release();
        }
    }

    public async Task<OutlinePage?> GetPageAsync(string path)
    {
        await FetchAllAsync();

        return _outlinePages.TryGetValue(path, out var page)
            ? page
            : null;
    }

    public async Task<bool> UpdatePageAsync(
        string urlId,
        string? title,
        string text,
        DateTime? updatedAt)
    {
        await _fetchLock.WaitAsync();

        try
        {
            var page = _outlinePages.Values.FirstOrDefault(
                page => page.UrlId.Equals(
                    urlId,
                    StringComparison.OrdinalIgnoreCase));

            if (page is null)
                return false;

            var markdown = CleanMarkdown(text);

            page.ContentHtml = Markdown.ToHtml(
                markdown,
                _pipeline);

            page.LastUpdated = updatedAt;

            return true;
        }
        finally
        {
            _fetchLock.Release();
        }
    }

    private async Task<(string Content, DateTime? LastUpdated)> FetchPageContentAsync(
        string urlId)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "documents.info",
            new { id = urlId });

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<OutlineResponse<OutlineDocument>>();

        if (result?.Data is null)
            throw new InvalidOperationException("API returned empty document response.");

        return (
            Content: result.Data.Text ?? "",
            LastUpdated: result.Data.UpdatedAt
        );
    }

    private static string CleanMarkdown(string text)
    {
        // According to https://github.com/outline/outline/blob/374f5ad4035e39903db1d968d3be9c7ed8adca2c/shared/utils/markdown.ts#L38.
        // Also removes the extra slash from some weirdly escaped newlines.
        return Regex.Replace(
            text,
            @"\\([\\*+\-\d.])",
            "$1")
            .Replace("\\\n", "\n");
    }
}