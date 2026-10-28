using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Ludero.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public int HttpStatusCode { get; private set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet(int? statusCode)
    {
        HttpStatusCode = statusCode ?? Response.StatusCode;
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
