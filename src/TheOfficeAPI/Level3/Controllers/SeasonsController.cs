using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Level3.Models;
using TheOfficeAPI.Level3.Services;

namespace TheOfficeAPI.Level3.Controllers;

[ApiController]
[Route("api/v3/seasons")]
public class SeasonsController : ControllerBase
{
    private const string SeasonsApiPath = "/api/v3/seasons";
    private const string RelCollection = "collection";

    private readonly TheOfficeService _theOfficeService;

    public SeasonsController(TheOfficeService theOfficeService)
    {
        _theOfficeService = theOfficeService;
    }

    /// <summary>
    /// Retrieves all seasons (Level 3 HATEOAS)
    /// </summary>
    /// <returns>All available seasons with hypermedia links</returns>
    /// <response code="200">Seasons retrieved successfully with hypermedia links</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/v3/seasons)
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK)
    /// - Includes hypermedia links to related resources for discoverability
    /// - Links include: self, individual seasons, and season episodes
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/v3/seasons
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "seasonNumber": "1",
    ///       "episodeCount": 6,
    ///       "links": [
    ///         { "rel": "self", "href": "/api/v3/seasons/1", "method": "GET" },
    ///         { "rel": "episodes", "href": "/api/v3/seasons/1/episodes", "method": "GET" }
    ///       ]
    ///     }
    ///   ],
    ///   "message": "Seasons retrieved successfully",
    ///   "links": [
    ///     { "rel": "self", "href": "/api/v3/seasons", "method": "GET" }
    ///   ]
    /// }
    /// </code>
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(HateoasResponse<List<SeasonResource>>), StatusCodes.Status200OK)]
    public IActionResult GetAllSeasons()
    {
        try
        {
            var seasons = _theOfficeService.GetAllSeasons();
            var seasonResources = seasons.Select(s => new SeasonResource
            {
                SeasonNumber = s.SeasonNumber,
                EpisodeCount = s.EpisodeCount,
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = $"{SeasonsApiPath}/{s.SeasonNumber}", Method = "GET" },
                    new Link { Rel = "episodes", Href = $"{SeasonsApiPath}/{s.SeasonNumber}/episodes", Method = "GET" }
                }
            }).ToList();

            var response = new HateoasResponse<List<SeasonResource>>
            {
                Success = true,
                Data = seasonResources,
                Message = "Seasons retrieved successfully",
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = SeasonsApiPath, Method = "GET" }
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
    /// Retrieves a specific season (Level 3 HATEOAS)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>The specified season with hypermedia links</returns>
    /// <response code="200">Season retrieved successfully with hypermedia links</response>
    /// <response code="404">Season not found</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/v3/seasons/{seasonNumber})
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Includes hypermedia links to navigate to related resources
    /// - Links include: self, collection (all seasons), and episodes for this season
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/v3/seasons/2
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "seasonNumber": "2",
    ///     "episodeCount": 22,
    ///     "links": [
    ///       { "rel": "self", "href": "/api/v3/seasons/2", "method": "GET" },
    ///       { "rel": "episodes", "href": "/api/v3/seasons/2/episodes", "method": "GET" },
    ///       { "rel": "collection", "href": "/api/v3/seasons", "method": "GET" }
    ///     ]
    ///   },
    ///   "message": "Season 2 retrieved successfully",
    ///   "links": [
    ///     { "rel": "self", "href": "/api/v3/seasons/2", "method": "GET" },
    ///     { "rel": "collection", "href": "/api/v3/seasons", "method": "GET" }
    ///   ]
    /// }
    /// </code>
    /// </example>
    [HttpGet("{seasonNumber}")]
    [ProducesResponseType(typeof(HateoasResponse<SeasonResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HateoasResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetSeason([FromRoute] int seasonNumber)
    {
        try
        {
            var seasonRangeValidation = ValidateSeasonRange(seasonNumber);
            if (seasonRangeValidation != null) return seasonRangeValidation;

            var seasons = _theOfficeService.GetAllSeasons();
            var season = seasons.FirstOrDefault(s => s.SeasonNumber == seasonNumber.ToString());

            if (season == null)
            {
                return NotFound(new HateoasResponse<object>
                {
                    Success = false,
                    Error = $"Season {seasonNumber} not found",
                    Message = "Season not found",
                    Links = new List<Link>
                    {
                        new Link { Rel = RelCollection, Href = SeasonsApiPath, Method = "GET" }
                    }
                });
            }

            var seasonResource = new SeasonResource
            {
                SeasonNumber = season.SeasonNumber,
                EpisodeCount = season.EpisodeCount,
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = $"{SeasonsApiPath}/{seasonNumber}", Method = "GET" },
                    new Link { Rel = "episodes", Href = $"{SeasonsApiPath}/{seasonNumber}/episodes", Method = "GET" },
                    new Link { Rel = RelCollection, Href = SeasonsApiPath, Method = "GET" }
                }
            };

            var response = new HateoasResponse<SeasonResource>
            {
                Success = true,
                Data = seasonResource,
                Message = $"Season {seasonNumber} retrieved successfully",
                Links = new List<Link>
                {
                    new Link { Rel = "self", Href = $"{SeasonsApiPath}/{seasonNumber}", Method = "GET" },
                    new Link { Rel = RelCollection, Href = SeasonsApiPath, Method = "GET" }
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
                    new Link { Rel = RelCollection, Href = SeasonsApiPath, Method = "GET" }
                }
            });
        }
        return null;
    }
}
