namespace Ludero.Web.Services;

public class PostmarkOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string NotificationAddress { get; set; } = string.Empty;
}
