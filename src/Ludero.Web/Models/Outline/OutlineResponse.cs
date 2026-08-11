namespace Ludero.Web.Models.Outline;

public class OutlineResponse<T>
{
    public T? Data { get; set; }
}

public class OutlineDocument
{
    public string? Text { get; set; }

    public DateTime? UpdatedAt { get; set; }
}