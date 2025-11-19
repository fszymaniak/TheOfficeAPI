namespace TheOfficeAPI.Level3.Models;

/// <summary>
/// Represents a hypermedia link for HATEOAS (Level 3 Richardson Maturity)
/// </summary>
public class Link
{
    /// <summary>
    /// The URI of the linked resource
    /// </summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>
    /// The relationship type (e.g., "self", "episodes", "season")
    /// </summary>
    public string Rel { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP method to use (e.g., "GET", "POST", "PUT", "DELETE")
    /// </summary>
    public string Method { get; set; } = "GET";

    public Link()
    {
    }

    public Link(string href, string rel, string method = "GET")
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}
