using TheOfficeAPI.Common.Data;
using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Level2.Services
{
    public class TheOfficeService
    {
        private readonly List<Episode> _episodes;
        private readonly List<Season> _seasons;

        public TheOfficeService()
        {
            _episodes = OfficeEpisodesData.Episodes;
            _seasons = InitializeSeasons();
        }

        public List<Season> GetAllSeasons()
        {
            return _seasons;
        }

        public List<Episode> GetSeasonEpisodes(int season)
        {
            return _episodes.Where(e => e.Season == season).ToList();
        }

        public Episode? GetEpisode(int season, int episode)
        {
            return _episodes.FirstOrDefault(e => e.Season == season && e.EpisodeNumber == episode);
        }

        private List<Season> InitializeSeasons()
        {
            return _episodes
                .Where(HasValidSeason)
                .GroupBy(e => e.Season!.Value)
                .Select(CreateSeasonFromGroup)
                .OrderBy(s => s.SeasonNumber)
                .ToList();
        }

        private static bool HasValidSeason(Episode episode) => episode.Season.HasValue;

        private static Season CreateSeasonFromGroup(IGrouping<int, Episode> seasonGroup)
        {
            return new Season
            {
                SeasonNumber = seasonGroup.Key.ToString(),
                EpisodeCount = seasonGroup.Count()
            };
        }
    }
}
