using Ludero.Web.Models;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ludero.Web.Pages.Nieuws;

public class DetailModel : PageModel
{
    private readonly NewsService _news;

    [BindProperty(SupportsGet = true)]
    public string Slug { get; set; } = "";

    public NewsArticle? Article { get; private set; }

    public DetailModel(NewsService news) => _news = news;

    public async Task<IActionResult> OnGetAsync()
    {
        Article = await _news.GetBySlugAsync(Slug);
        if (Article is null)
            return NotFound();
        return Page();
    }
}
