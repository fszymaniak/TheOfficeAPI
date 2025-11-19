using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level3.Models;

/// <summary>
/// Episode model with HATEOAS links (Level 3 Richardson Maturity)
/// </summary>
public class EpisodeWithLinks
{
    public int? Season { get; set; }
    public int? EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReleasedDate { get; set; } = string.Empty;
    public List<Link> Links { get; set; } = new();

    public EpisodeWithLinks()
    {
    }

    public EpisodeWithLinks(Episode episode)
    {
        Season = episode.Season;
        EpisodeNumber = episode.EpisodeNumber;
        Title = episode.Title;
        ReleasedDate = episode.ReleasedDate;
    }
}
