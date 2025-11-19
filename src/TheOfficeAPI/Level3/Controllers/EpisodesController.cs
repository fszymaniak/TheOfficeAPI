using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Level3.Models;
using TheOfficeAPI.Level3.Services;

namespace TheOfficeAPI.Level3.Controllers;

[ApiController]
[Route("api/seasons/{seasonNumber}/episodes")]
public class EpisodesController : ControllerBase
{
    private readonly TheOfficeService _theOfficeService;

    public EpisodesController(TheOfficeService theOfficeService)
    {
        _theOfficeService = theOfficeService;
    }

    /// <summary>
    /// Retrieves all episodes for a specific season with HATEOAS links (Level 3 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>Episodes for the specified season with hypermedia controls</returns>
    /// <response code="200">Episodes retrieved successfully</response>
    /// <response code="404">Season not found</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/seasons/{seasonNumber}/episodes)
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Includes hypermedia links for navigation and discovery
    ///
    /// Each episode includes links to:
    /// - self: The episode's own URI
    /// - season: URI to get the parent season
    /// - allEpisodes: URI to get all episodes for this season
    /// - next: URI to the next episode (if available)
    /// - previous: URI to the previous episode (if available)
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithLinks<List<EpisodeWithLinks>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithLinks<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetSeasonEpisodes([FromRoute] int seasonNumber)
    {
        try
        {
            var seasonRangeValidation = ValidateSeasonRange(seasonNumber);
            if (seasonRangeValidation != null) return seasonRangeValidation;

            var episodes = _theOfficeService.GetSeasonEpisodes(seasonNumber);
            var episodesWithLinks = episodes.Select(episode =>
            {
                var episodeWithLinks = new EpisodeWithLinks(episode);
                episodeWithLinks.Links = new List<Link>
                {
                    new Link($"/api/seasons/{seasonNumber}/episodes/{episode.EpisodeNumber}", "self", "GET"),
                    new Link($"/api/seasons/{seasonNumber}", "season", "GET"),
                    new Link($"/api/seasons/{seasonNumber}/episodes", "allEpisodes", "GET")
                };

                // Add next/previous episode links
                if (episode.EpisodeNumber > 1)
                {
                    episodeWithLinks.Links.Add(new Link($"/api/seasons/{seasonNumber}/episodes/{episode.EpisodeNumber - 1}", "previous", "GET"));
                }
                if (episode.EpisodeNumber < episodes.Count)
                {
                    episodeWithLinks.Links.Add(new Link($"/api/seasons/{seasonNumber}/episodes/{episode.EpisodeNumber + 1}", "next", "GET"));
                }

                return episodeWithLinks;
            }).ToList();

            var response = new ApiResponseWithLinks<List<EpisodeWithLinks>>
            {
                Success = true,
                Data = episodesWithLinks,
                Message = $"Episodes for season {seasonNumber} retrieved successfully",
                Links = new List<Link>
                {
                    new Link($"/api/seasons/{seasonNumber}/episodes", "self", "GET"),
                    new Link($"/api/seasons/{seasonNumber}", "season", "GET"),
                    new Link("/api/seasons", "allSeasons", "GET")
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseWithLinks<object>
            {
                Success = false,
                Error = ex.Message,
                Message = "An error occurred while processing the request"
            });
        }
    }

    /// <summary>
    /// Retrieves a specific episode with HATEOAS links (Level 3 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <param name="episodeNumber">The episode number</param>
    /// <returns>The specified episode with hypermedia controls</returns>
    /// <response code="200">Episode retrieved successfully</response>
    /// <response code="404">Episode or season not found</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/seasons/{seasonNumber}/episodes/{episodeNumber})
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Includes hypermedia links for navigation and discovery
    ///
    /// The response includes links to:
    /// - self: The episode's own URI
    /// - season: URI to get the parent season
    /// - allEpisodes: URI to get all episodes for this season
    /// - next: URI to the next episode (if available)
    /// - previous: URI to the previous episode (if available)
    /// - nextSeason: URI to the first episode of next season (if available)
    /// </remarks>
    [HttpGet("{episodeNumber}")]
    [ProducesResponseType(typeof(ApiResponseWithLinks<EpisodeWithLinks>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithLinks<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetEpisode([FromRoute] int seasonNumber, [FromRoute] int episodeNumber)
    {
        try
        {
            var seasonRangeValidation = ValidateSeasonRange(seasonNumber);
            if (seasonRangeValidation != null) return seasonRangeValidation;

            var episodeRangeValidation = ValidateEpisodeRangeFromSpecificSeason(seasonNumber, episodeNumber);
            if (episodeRangeValidation != null) return episodeRangeValidation;

            var episode = _theOfficeService.GetEpisode(seasonNumber, episodeNumber);

            if (episode == null)
            {
                return NotFound(new ApiResponseWithLinks<object>
                {
                    Success = false,
                    Error = $"Episode {episodeNumber} of season {seasonNumber} not found",
                    Message = "Episode not found",
                    Links = new List<Link>
                    {
                        new Link($"/api/seasons/{seasonNumber}/episodes", "allEpisodes", "GET"),
                        new Link($"/api/seasons/{seasonNumber}", "season", "GET"),
                        new Link("/api/seasons", "allSeasons", "GET")
                    }
                });
            }

            var episodeWithLinks = new EpisodeWithLinks(episode);
            episodeWithLinks.Links = new List<Link>
            {
                new Link($"/api/seasons/{seasonNumber}/episodes/{episodeNumber}", "self", "GET"),
                new Link($"/api/seasons/{seasonNumber}", "season", "GET"),
                new Link($"/api/seasons/{seasonNumber}/episodes", "allEpisodes", "GET")
            };

            // Add next/previous episode links within the season
            var episodesInSeason = _theOfficeService.GetSeasonEpisodes(seasonNumber);
            if (episodeNumber > 1)
            {
                episodeWithLinks.Links.Add(new Link($"/api/seasons/{seasonNumber}/episodes/{episodeNumber - 1}", "previous", "GET"));
            }
            if (episodeNumber < episodesInSeason.Count)
            {
                episodeWithLinks.Links.Add(new Link($"/api/seasons/{seasonNumber}/episodes/{episodeNumber + 1}", "next", "GET"));
            }
            else
            {
                // Last episode of the season, add link to next season's first episode
                var allSeasons = _theOfficeService.GetAllSeasons();
                if (seasonNumber < allSeasons.Count)
                {
                    episodeWithLinks.Links.Add(new Link($"/api/seasons/{seasonNumber + 1}/episodes/1", "nextSeason", "GET"));
                }
            }

            var response = new ApiResponseWithLinks<EpisodeWithLinks>
            {
                Success = true,
                Data = episodeWithLinks,
                Message = "Episode retrieved successfully",
                Links = new List<Link>
                {
                    new Link($"/api/seasons/{seasonNumber}/episodes/{episodeNumber}", "self", "GET"),
                    new Link($"/api/seasons/{seasonNumber}/episodes", "allEpisodes", "GET"),
                    new Link($"/api/seasons/{seasonNumber}", "season", "GET"),
                    new Link("/api/seasons", "allSeasons", "GET")
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseWithLinks<object>
            {
                Success = false,
                Error = ex.Message,
                Message = "An error occurred while processing the request"
            });
        }
    }

    private IActionResult? ValidateSeasonRange(int season)
    {
        var seasonsCount = _theOfficeService.GetAllSeasons().Count;
        if (season < 1 || season > seasonsCount)
        {
            return NotFound(new ApiResponseWithLinks<object>
            {
                Success = false,
                Error = $"Season parameter is outside of the scope. Please select the season number between 1 and {seasonsCount} (inclusive).",
                Message = "Invalid request",
                Links = new List<Link>
                {
                    new Link("/api/seasons", "allSeasons", "GET")
                }
            });
        }
        return null;
    }

    private IActionResult? ValidateEpisodeRangeFromSpecificSeason(int season, int episode)
    {
        var episodesCountFromSpecificSeason = _theOfficeService.GetSeasonEpisodes(season).Count;
        if (episode < 1 || episode > episodesCountFromSpecificSeason)
        {
            return NotFound(new ApiResponseWithLinks<object>
            {
                Success = false,
                Error = $"Episode parameter is outside of the scope. Please select the episode number between 1 and {episodesCountFromSpecificSeason} (inclusive).",
                Message = "Invalid request",
                Links = new List<Link>
                {
                    new Link($"/api/seasons/{season}/episodes", "allEpisodes", "GET"),
                    new Link($"/api/seasons/{season}", "season", "GET"),
                    new Link("/api/seasons", "allSeasons", "GET")
                }
            });
        }
        return null;
    }
}
