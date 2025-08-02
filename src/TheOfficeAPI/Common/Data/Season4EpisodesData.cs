using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Data
{
    public static class Season4Episodes
    {
        private const int SeasonNumber = 4;
    
        public static readonly List<Episode> Episodes =
        [
            new Episode { Season = SeasonNumber, EpisodeNumber = 1, Title = "Fun Run", ReleasedDate = "2007-09-27" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 2, Title = "Dunder Mifflin Infinity", ReleasedDate = "2007-10-04" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 3, Title = "Launch Party", ReleasedDate = "2007-10-11" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 4, Title = "Money", ReleasedDate = "2007-10-18" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 5, Title = "Local Ad", ReleasedDate = "2007-10-25" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 6, Title = "Branch Wars", ReleasedDate = "2007-11-01" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 7, Title = "Survivor Man", ReleasedDate = "2007-11-08" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 8, Title = "The Deposition", ReleasedDate = "2007-11-15" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 9, Title = "Dinner Party", ReleasedDate = "2008-04-10" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 10, Title = "Chair Model", ReleasedDate = "2008-04-17" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 11, Title = "Night Out", ReleasedDate = "2008-04-24" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 12, Title = "Did I Stutter?", ReleasedDate = "2008-05-01" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 13, Title = "Job Fair", ReleasedDate = "2008-05-08" },
            new Episode { Season = SeasonNumber, EpisodeNumber = 14, Title = "Goodbye, Toby", ReleasedDate = "2008-05-15" }
        ];
    }
}
