namespace TheOfficeAPI.Level3.Models;

/// <summary>
/// Represents a hypermedia link following HATEOAS principles
/// </summary>
public class Link
{
    /// <summary>
    /// The relationship type of the link (e.g., "self", "collection", "episodes")
    /// </summary>
    public string Rel { get; set; } = string.Empty;

    /// <summary>
    /// The URL/URI of the linked resource
    /// </summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP method to use for this link (e.g., "GET", "POST")
    /// </summary>
    public string Method { get; set; } = "GET";
}

/// <summary>
/// Base response class with HATEOAS links
/// </summary>
public class HateoasResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// A message describing the result
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error message if the request failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Hypermedia links to related resources
    /// </summary>
    public List<Link> Links { get; set; } = new();
}

/// <summary>
/// Season resource with HATEOAS links
/// </summary>
public class SeasonResource
{
    /// <summary>
    /// The season number
    /// </summary>
    public string SeasonNumber { get; set; } = string.Empty;

    /// <summary>
    /// The number of episodes in the season
    /// </summary>
    public int EpisodeCount { get; set; }

    /// <summary>
    /// Hypermedia links to related resources
    /// </summary>
    public List<Link> Links { get; set; } = new();
}

/// <summary>
/// Episode resource with HATEOAS links
/// </summary>
public class EpisodeResource
{
    /// <summary>
    /// The season number
    /// </summary>
    public int? Season { get; set; }

    /// <summary>
    /// The episode number
    /// </summary>
    public int? EpisodeNumber { get; set; }

    /// <summary>
    /// The episode title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The release date
    /// </summary>
    public string ReleasedDate { get; set; } = string.Empty;

    /// <summary>
    /// Hypermedia links to related resources
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
