using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Level3.Models;
using TheOfficeAPI.Level3.Services;

namespace TheOfficeAPI.Level3.Controllers;

[ApiController]
[Route("api/seasons")]
public class SeasonsController : ControllerBase
{
    private readonly TheOfficeService _theOfficeService;

    public SeasonsController(TheOfficeService theOfficeService)
    {
        _theOfficeService = theOfficeService;
    }

    /// <summary>
    /// Retrieves all seasons with HATEOAS links (Level 3 Richardson Maturity)
    /// </summary>
    /// <returns>All available seasons with hypermedia controls</returns>
    /// <response code="200">Seasons retrieved successfully</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/seasons)
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK)
    /// - Includes hypermedia links for navigation and discovery
    /// - Client can follow links without hardcoding URIs
    ///
    /// Each season includes links to:
    /// - self: The season's own URI
    /// - episodes: URI to get all episodes for this season
    /// - allSeasons: URI to get all seasons
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/seasons
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "seasonNumber": "1",
    ///       "episodeCount": 6,
    ///       "links": [
    ///         { "href": "/api/seasons/1", "rel": "self", "method": "GET" },
    ///         { "href": "/api/seasons/1/episodes", "rel": "episodes", "method": "GET" },
    ///         { "href": "/api/seasons", "rel": "allSeasons", "method": "GET" }
    ///       ]
    ///     }
    ///   ],
    ///   "message": "Seasons retrieved successfully",
    ///   "links": [
    ///     { "href": "/api/seasons", "rel": "self", "method": "GET" }
    ///   ]
    /// }
    /// </code>
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithLinks<List<SeasonWithLinks>>), StatusCodes.Status200OK)]
    public IActionResult GetAllSeasons()
    {
        try
        {
            var seasons = _theOfficeService.GetAllSeasons();
            var seasonsWithLinks = seasons.Select(season =>
            {
                var seasonWithLinks = new SeasonWithLinks(season);
                seasonWithLinks.Links = new List<Link>
                {
                    new Link($"/api/seasons/{season.SeasonNumber}", "self", "GET"),
                    new Link($"/api/seasons/{season.SeasonNumber}/episodes", "episodes", "GET"),
                    new Link("/api/seasons", "allSeasons", "GET")
                };
                return seasonWithLinks;
            }).ToList();

            var response = new ApiResponseWithLinks<List<SeasonWithLinks>>
            {
                Success = true,
                Data = seasonsWithLinks,
                Message = "Seasons retrieved successfully",
                Links = new List<Link>
                {
                    new Link("/api/seasons", "self", "GET")
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
    /// Retrieves a specific season with HATEOAS links (Level 3 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>The specified season with hypermedia controls</returns>
    /// <response code="200">Season retrieved successfully</response>
    /// <response code="404">Season not found</response>
    /// <remarks>
    /// This endpoint follows Level 3 Richardson Maturity Model (HATEOAS):
    /// - Uses resource-based URI (/api/seasons/{seasonNumber})
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Includes hypermedia links for navigation and discovery
    ///
    /// The response includes links to:
    /// - self: The season's own URI
    /// - episodes: URI to get all episodes for this season
    /// - allSeasons: URI to get all seasons
    /// - next: URI to the next season (if available)
    /// - previous: URI to the previous season (if available)
    /// </remarks>
    [HttpGet("{seasonNumber}")]
    [ProducesResponseType(typeof(ApiResponseWithLinks<SeasonWithLinks>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseWithLinks<object>), StatusCodes.Status404NotFound)]
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
                return NotFound(new ApiResponseWithLinks<object>
                {
                    Success = false,
                    Error = $"Season {seasonNumber} not found",
                    Message = "Season not found",
                    Links = new List<Link>
                    {
                        new Link("/api/seasons", "allSeasons", "GET")
                    }
                });
            }

            var seasonWithLinks = new SeasonWithLinks(season);
            seasonWithLinks.Links = new List<Link>
            {
                new Link($"/api/seasons/{seasonNumber}", "self", "GET"),
                new Link($"/api/seasons/{seasonNumber}/episodes", "episodes", "GET"),
                new Link("/api/seasons", "allSeasons", "GET")
            };

            // Add next/previous season links
            if (seasonNumber > 1)
            {
                seasonWithLinks.Links.Add(new Link($"/api/seasons/{seasonNumber - 1}", "previous", "GET"));
            }
            if (seasonNumber < seasons.Count)
            {
                seasonWithLinks.Links.Add(new Link($"/api/seasons/{seasonNumber + 1}", "next", "GET"));
            }

            var response = new ApiResponseWithLinks<SeasonWithLinks>
            {
                Success = true,
                Data = seasonWithLinks,
                Message = $"Season {seasonNumber} retrieved successfully",
                Links = new List<Link>
                {
                    new Link($"/api/seasons/{seasonNumber}", "self", "GET"),
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
}
