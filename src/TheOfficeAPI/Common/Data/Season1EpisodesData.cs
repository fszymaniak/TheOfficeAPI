using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Data
{
    public static class Season1Episodes
    {
        private const int SeasonNumber = 1;
        
        public static readonly List<Episode> Episodes =
        [
            new Episode { Season = SeasonNumber, EpisodeNumber = 1, Title = "Pilot", ReleasedDate = "2005-03-24" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 2, Title = "Diversity Day", ReleasedDate = "2005-03-29" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 3, Title = "Health Care", ReleasedDate = "2005-04-05" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 4, Title = "The Alliance", ReleasedDate = "2005-04-12" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 5, Title = "Basketball", ReleasedDate = "2005-04-19" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 6, Title = "Hot Girl", ReleasedDate = "2005-04-26" }
        ];
    }
}