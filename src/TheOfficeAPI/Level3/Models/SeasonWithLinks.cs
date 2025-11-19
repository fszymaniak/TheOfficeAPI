using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level3.Models;

/// <summary>
/// Season model with HATEOAS links (Level 3 Richardson Maturity)
/// </summary>
public class SeasonWithLinks
{
    public string SeasonNumber { get; set; } = string.Empty;
    public int EpisodeCount { get; set; }
    public List<Link> Links { get; set; } = new();

    public SeasonWithLinks()
    {
    }

    public SeasonWithLinks(Season season)
    {
        SeasonNumber = season.SeasonNumber;
        EpisodeCount = season.EpisodeCount;
    }
}
