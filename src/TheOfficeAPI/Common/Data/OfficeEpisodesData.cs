using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Data
{
    public static class OfficeEpisodesData
    {
        public static readonly List<Episode> Episodes = new List<Episode>()
            .Concat(Season1Episodes.Episodes)
            .Concat(Season2Episodes.Episodes)
            .Concat(Season3Episodes.Episodes)
            .Concat(Season4Episodes.Episodes)
            .Concat(Season5Episodes.Episodes)
            .Concat(Season6Episodes.Episodes)
            .Concat(Season7Episodes.Episodes)
            .Concat(Season8Episodes.Episodes)
            .Concat(Season9Episodes.Episodes)
            .ToList();
    }
}