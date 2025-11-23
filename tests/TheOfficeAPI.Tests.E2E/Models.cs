namespace TheOfficeAPI.Tests.E2E;

// Common Models
public class Episode
{
    public int? Season { get; set; }
    public int? EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReleasedDate { get; set; } = string.Empty;
}

public class Season
{
    public string SeasonNumber { get; set; } = string.Empty;
    public int EpisodeCount { get; set; }
}

public class ApiRequest
{
    public string Action { get; set; } = string.Empty;
    public int? Season { get; set; }
    public int? Episode { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
}

// Level 3 HATEOAS Models
public class Link
{
    public string Rel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
}

public class HateoasResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
    public List<Link> Links { get; set; } = new();
}

public class SeasonResource
{
    public string SeasonNumber { get; set; } = string.Empty;
    public int EpisodeCount { get; set; }
    public List<Link> Links { get; set; } = new();
}

public class EpisodeResource
{
    public int? Season { get; set; }
    public int? EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReleasedDate { get; set; } = string.Empty;
    public List<Link> Links { get; set; } = new();
}
