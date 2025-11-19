namespace TheOfficeAPI.Level3.Models;

/// <summary>
/// API response wrapper with HATEOAS links (Level 3 Richardson Maturity)
/// </summary>
public class ApiResponseWithLinks<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
    public List<Link> Links { get; set; } = new();
}
