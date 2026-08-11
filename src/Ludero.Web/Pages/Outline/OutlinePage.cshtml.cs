using Ludero.Web.Models.Outline;
using Ludero.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ludero.Web.Pages.Outline;

public class OutlinePageModel : PageModel
{
    private readonly OutlineService _outline;

    public OutlinePage? PageObject { get; private set; }

    public OutlinePageModel(OutlineService outline)
    {
        _outline = outline;
    }

    public async Task<IActionResult> OnGetAsync(string path)
    {
        PageObject = await _outline.GetPageAsync(path);
        if (PageObject is null)
            return NotFound();
        return Page();
    }
}
