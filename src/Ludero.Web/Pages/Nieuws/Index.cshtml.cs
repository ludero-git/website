using Ludero.Web.Models;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ludero.Web.Pages.Nieuws;

public class IndexModel : PageModel
{
    private readonly NewsService _news;

    public List<NewsArticle> Articles { get; private set; } = [];

    public IndexModel(NewsService news) => _news = news;

    public async Task OnGetAsync()
    {
        Articles = await _news.GetAllAsync();
    }
}
