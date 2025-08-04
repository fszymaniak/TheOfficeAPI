using TheOfficeAPI.Level0.Services;

namespace TheOfficeAPI.Level0.Tests.Unit;

public class TheOfficeServiceTests
{
    private readonly TheOfficeService _service;

    public TheOfficeServiceTests()
    {
        _service = new TheOfficeService();
    }

    [Fact]
    public void GetAllSeasons_ReturnsNonEmptyList()
    {
        // Act
        var seasons = _service.GetAllSeasons();

        // Assert
        Assert.NotNull(seasons);
        Assert.NotEmpty(seasons);
    }

    [Fact]
    public void GetAllSeasons_ReturnsOrderedSeasons()
    {
        // Act
        var seasons = _service.GetAllSeasons();

        // Assert
        for (int i = 1; i < seasons.Count; i++)
        {
            var currentSeason = int.Parse(seasons[i].SeasonNumber);
            var previousSeason = int.Parse(seasons[i - 1].SeasonNumber);
            Assert.True(currentSeason >= previousSeason, "Seasons should be ordered");
        }
    }

    [Fact]
    public void GetSeasonEpisodes_WithValidSeason_ReturnsEpisodes()
    {
        // Arrange
        var season = 1;

        // Act
        var episodes = _service.GetSeasonEpisodes(season);

        // Assert
        Assert.NotNull(episodes);
        Assert.All(episodes, episode => Assert.Equal(season, episode.Season));
    }

    [Fact]
    public void GetSeasonEpisodes_WithNullSeason_ReturnsEmptyList()
    {
        // Act
        var episodes = _service.GetSeasonEpisodes(null);

        // Assert
        Assert.NotNull(episodes);
        Assert.Empty(episodes);
    }

    [Fact]
    public void GetSeasonEpisodes_WithInvalidSeason_ReturnsEmptyList()
    {
        // Arrange
        var invalidSeason = 999;

        // Act
        var episodes = _service.GetSeasonEpisodes(invalidSeason);

        // Assert
        Assert.NotNull(episodes);
        Assert.Empty(episodes);
    }

    [Fact]
    public void GetEpisode_WithValidSeasonAndEpisode_ReturnsEpisode()
    {
        // Arrange
        var season = 1;
        var episodeNumber = 1;

        // Act
        var episode = _service.GetEpisode(season, episodeNumber);

        // Assert
        Assert.NotNull(episode);
        Assert.Equal(season, episode.Season);
        Assert.Equal(episodeNumber, episode.EpisodeNumber);
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData(1, null)]
    [InlineData(null, null)]
    public void GetEpisode_WithNullParameters_ReturnsNull(int? season, int? episode)
    {
        // Act
        var result = _service.GetEpisode(season, episode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetEpisode_WithInvalidSeasonAndEpisode_ReturnsNull()
    {
        // Arrange
        var invalidSeason = 999;
        var invalidEpisode = 999;

        // Act
        var episode = _service.GetEpisode(invalidSeason, invalidEpisode);

        // Assert
        Assert.Null(episode);
    }
}