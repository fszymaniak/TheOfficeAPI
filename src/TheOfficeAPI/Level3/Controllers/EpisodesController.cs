using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Level3.Models;
using TheOfficeAPI.Level3.Services;

namespace TheOfficeAPI.Level3.Controllers;

[ApiController]
[Route("api/v3/seasons/{seasonNumber}/episodes")]
public class EpisodesController : ControllerBase
{
    private const string RelCollection = "collection";
    private const string RelSeason = "season";

    private readonly TheOfficeService _theOfficeService;

    public EpisodesController(TheOfficeService theOfficeService)
    {
        _theOfficeService = theOfficeService;
    }

    /// <summary>
    /// Retrieves all episodes for a specific season (Level 3 HATEOAS)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>Episodes for the specified season with hypermedia links</returns>
    /// <response code="200">Episodes retrieved successfully with hypermedia links</response>
    /// <response code="404">Season not found</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/v3/seasons/{seasonNumber}/episodes)
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Includes hypermedia links for each episode to enable navigation
    /// - Links include: self, individual episodes, parent season, and all seasons
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/v3/seasons/1/episodes
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "season": 1,
    ///       "episodeNumber": 1,
    ///       "title": "Pilot",
    ///       "releasedDate": "2005-03-24",
    ///       "links": [
    ///         { "rel": "self", "href": "/api/v3/seasons/1/episodes/1", "method": "GET" },
    ///         { "rel": "season", "href": "/api/v3/seasons/1", "method": "GET" }
    ///       ]
    ///     }
    ///   ],
    ///   "message": "Episodes for season 1 retrieved successfully",
    ///   "links": [
    ///     { "rel": "self", "href": "/api/v3/seasons/1/episodes", "method": "GET" },
    ///     { "rel": "season", "href": "/api/v3/seasons/1", "method": "GET" },
    ///     { "rel": "collection", "href": "/api/v3/seasons", "method": "GET" }
    ///   ]
    /// }
    /// </code>
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(HateoasResponse<List<EpisodeResource>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HateoasResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetSeasonEpisodes([FromRoute] int seasonNumber)
    {
        try
        {
            var seasonRangeValidation = ValidateSeasonRange(seasonNumber);
            if (seasonRangeValidation != null) return seasonRangeValidation;

            var episodes = _theOfficeService.GetSeasonEpisodes(seasonNumber);
            var episodeResources = episodes.Select(e => new EpisodeResource
            {
                Season = e.Season,
                EpisodeNumber = e.EpisodeNumber,
                Title = e.Title,
                ReleasedDate = e.ReleasedDate,
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = $"/api/v3/seasons/{seasonNumber}/episodes/{e.EpisodeNumber}", Method = "GET" },
                    new Link { Rel = RelSeason, Href = $"/api/v3/seasons/{seasonNumber}", Method = "GET" }
                }
            }).ToList();

            var response = new HateoasResponse<List<EpisodeResource>>
            {
                Success = true,
                Data = episodeResources,
                Message = $"Episodes for season {seasonNumber} retrieved successfully",
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = $"/api/v3/seasons/{seasonNumber}/episodes", Method = "GET" },
                    new Link { Rel = RelSeason, Href = $"/api/v3/seasons/{seasonNumber}", Method = "GET" },
                    new Link { Rel = RelCollection, Href = "/api/v3/seasons", Method = "GET" }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new HateoasResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Message = "An error occurred while processing the request"
            });
        }
    }

    /// <summary>
    /// Retrieves a specific episode (Level 3 HATEOAS)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <param name="episodeNumber">The episode number</param>
    /// <returns>The specified episode with hypermedia links</returns>
    /// <response code="200">Episode retrieved successfully with hypermedia links</response>
    /// <response code="404">Episode or season not found</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/v3/seasons/{seasonNumber}/episodes/{episodeNumber})
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Includes hypermedia links to related resources for full discoverability
    /// - Links include: self, next/previous episodes, parent season, all episodes, and all seasons
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/v3/seasons/2/episodes/1
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "season": 2,
    ///     "episodeNumber": 1,
    ///     "title": "The Dundies",
    ///     "releasedDate": "2005-09-20",
    ///     "links": [
    ///       { "rel": "self", "href": "/api/v3/seasons/2/episodes/1", "method": "GET" },
    ///       { "rel": "next", "href": "/api/v3/seasons/2/episodes/2", "method": "GET" },
    ///       { "rel": "season", "href": "/api/v3/seasons/2", "method": "GET" },
    ///       { "rel": "episodes", "href": "/api/v3/seasons/2/episodes", "method": "GET" },
    ///       { "rel": "collection", "href": "/api/v3/seasons", "method": "GET" }
    ///     ]
    ///   },
    ///   "message": "Episode retrieved successfully",
    ///   "links": [
    ///     { "rel": "self", "href": "/api/v3/seasons/2/episodes/1", "method": "GET" },
    ///     { "rel": "collection", "href": "/api/v3/seasons", "method": "GET" }
    ///   ]
    /// }
    ///
    /// Response (404 Not Found):
    /// {
    ///   "success": false,
    ///   "error": "Episode 99 of season 2 not found",
    ///   "message": "Episode not found",
    ///   "links": [
    ///     { "rel": "episodes", "href": "/api/v3/seasons/2/episodes", "method": "GET" },
    ///     { "rel": "season", "href": "/api/v3/seasons/2", "method": "GET" },
    ///     { "rel": "collection", "href": "/api/v3/seasons", "method": "GET" }
    ///   ]
    /// }
    /// </code>
    /// </example>
    [HttpGet("{episodeNumber}")]
    [ProducesResponseType(typeof(HateoasResponse<EpisodeResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HateoasResponse<object>), StatusCodes.Status404NotFound)]
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
                return NotFound(new HateoasResponse<object>
                {
                    Success = false,
                    Error = $"Episode {episodeNumber} of season {seasonNumber} not found",
                    Message = "Episode not found",
                    Links = new List<Link>
                    {
                        new Link { Rel = "episodes", Href = $"/api/v3/seasons/{seasonNumber}/episodes", Method = "GET" },
                        new Link { Rel = RelSeason, Href = $"/api/v3/seasons/{seasonNumber}", Method = "GET" },
                        new Link { Rel = RelCollection, Href = "/api/v3/seasons", Method = "GET" }
                    }
                });
            }

            var episodeResource = new EpisodeResource
            {
                Season = episode.Season,
                EpisodeNumber = episode.EpisodeNumber,
                Title = episode.Title,
                ReleasedDate = episode.ReleasedDate,
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = $"/api/v3/seasons/{seasonNumber}/episodes/{episodeNumber}", Method = "GET" }
                }
            };

            // Add next episode link if exists
            var allEpisodes = _theOfficeService.GetSeasonEpisodes(seasonNumber);
            if (episodeNumber < allEpisodes.Count)
            {
                episodeResource.Links.Add(new Link
                {
                    Rel = "next",
                    Href = $"/api/v3/seasons/{seasonNumber}/episodes/{episodeNumber + 1}",
                    Method = "GET"
                });
            }

            // Add previous episode link if exists
            if (episodeNumber > 1)
            {
                episodeResource.Links.Add(new Link
                {
                    Rel = "previous",
                    Href = $"/api/v3/seasons/{seasonNumber}/episodes/{episodeNumber - 1}",
                    Method = "GET"
                });
            }

            // Add parent and collection links
            episodeResource.Links.Add(new Link { Rel = RelSeason, Href = $"/api/v3/seasons/{seasonNumber}", Method = "GET" });
            episodeResource.Links.Add(new Link { Rel = "episodes", Href = $"/api/v3/seasons/{seasonNumber}/episodes", Method = "GET" });
            episodeResource.Links.Add(new Link { Rel = RelCollection, Href = "/api/v3/seasons", Method = "GET" });

            var response = new HateoasResponse<EpisodeResource>
            {
                Success = true,
                Data = episodeResource,
                Message = "Episode retrieved successfully",
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = $"/api/v3/seasons/{seasonNumber}/episodes/{episodeNumber}", Method = "GET" },
                    new Link { Rel = RelCollection, Href = "/api/v3/seasons", Method = "GET" }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new HateoasResponse<object>
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
            return NotFound(new HateoasResponse<object>
            {
                Success = false,
                Error = $"Season parameter is outside of the scope. Please select the season number between 1 and {seasonsCount} (inclusive).",
                Message = "Invalid request",
                Links = new List<Link>
                {
                    new Link { Rel = RelCollection, Href = "/api/v3/seasons", Method = "GET" }
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
            return NotFound(new HateoasResponse<object>
            {
                Success = false,
                Error = $"Episode parameter is outside of the scope. Please select the episode number between 1 and {episodesCountFromSpecificSeason} (inclusive).",
                Message = "Invalid request",
                Links = new List<Link>
                {
                    new Link { Rel = "episodes", Href = $"/api/v3/seasons/{season}/episodes", Method = "GET" },
                    new Link { Rel = RelSeason, Href = $"/api/v3/seasons/{season}", Method = "GET" },
                    new Link { Rel = RelCollection, Href = "/api/v3/seasons", Method = "GET" }
                }
            });
        }
        return null;
    }
}
