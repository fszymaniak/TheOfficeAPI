using System.Collections.ObjectModel;
using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Data;

public static class Season3Episodes
{
    private const int SeasonNumber = 3;

    public static readonly ReadOnlyCollection<Episode> Episodes = new(new List<Episode>
    {
        new Episode { Season = SeasonNumber, EpisodeNumber = 1, Title = "Gay Witch Hunt", ReleasedDate = "2006-09-21" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 2, Title = "The Convention", ReleasedDate = "2006-09-28" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 3, Title = "The Coup", ReleasedDate = "2006-10-05" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 4, Title = "Grief Counseling", ReleasedDate = "2006-10-12" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 5, Title = "Initiation", ReleasedDate = "2006-10-19" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 6, Title = "Diwali", ReleasedDate = "2006-11-02" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 7, Title = "Branch Closing", ReleasedDate = "2006-11-09" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 8, Title = "The Merger", ReleasedDate = "2006-11-16" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 9, Title = "The Convict", ReleasedDate = "2006-11-30" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 10, Title = "A Benihana Christmas", ReleasedDate = "2006-12-14" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 11, Title = "Back from Vacation", ReleasedDate = "2007-01-04" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 12, Title = "Traveling Salesmen", ReleasedDate = "2007-01-11" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 13, Title = "The Return", ReleasedDate = "2007-01-18" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 14, Title = "Ben Franklin", ReleasedDate = "2007-02-01" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 15, Title = "Phyllis' Wedding", ReleasedDate = "2007-02-08" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 16, Title = "Business School", ReleasedDate = "2007-02-15" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 17, Title = "Cocktails", ReleasedDate = "2007-02-22" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 18, Title = "The Negotiation", ReleasedDate = "2007-04-05" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 19, Title = "Safety Training", ReleasedDate = "2007-04-12" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 20, Title = "Product Recall", ReleasedDate = "2007-04-26" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 21, Title = "Women's Appreciation", ReleasedDate = "2007-05-03" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 22, Title = "Beach Games", ReleasedDate = "2007-05-10" },
        new Episode { Season = SeasonNumber, EpisodeNumber = 23, Title = "The Job", ReleasedDate = "2007-05-17" }
    });
}