using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level2.Services;

namespace TheOfficeAPI.Level2.Controllers;

[ApiController]
[Route("api/v2/seasons")]
public class SeasonsController : ControllerBase
{
    private readonly TheOfficeService _theOfficeService;

    public SeasonsController(TheOfficeService theOfficeService)
    {
        _theOfficeService = theOfficeService;
    }

    /// <summary>
    /// Retrieves all seasons (Level 2 Richardson Maturity)
    /// </summary>
    /// <returns>All available seasons</returns>
    /// <response code="200">Seasons retrieved successfully</response>
    /// <remarks>
    /// This endpoint follows Level 2 Richardson Maturity Model:
    /// - Uses resource-based URI (/api/seasons)
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK)
    /// - Uses standard HTTP methods for CRUD operations
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
    ///       "episodeCount": 6
    ///     },
    ///     {
    ///       "seasonNumber": "2",
    ///       "episodeCount": 22
    ///     }
    ///   ],
    ///   "message": "Seasons retrieved successfully"
    /// }
    /// </code>
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<Season>>), StatusCodes.Status200OK)]
    public IActionResult GetAllSeasons()
    {
        try
        {
            var seasons = _theOfficeService.GetAllSeasons();
            return Ok(new ApiResponse<List<Season>>
            {
                Success = true,
                Data = seasons,
                Message = "Seasons retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Message = "An error occurred while processing the request"
            });
        }
    }

    /// <summary>
    /// Retrieves a specific season (Level 2 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>The specified season</returns>
    /// <response code="200">Season retrieved successfully</response>
    /// <response code="404">Season not found</response>
    /// <remarks>
    /// This endpoint follows Level 2 Richardson Maturity Model:
    /// - Uses resource-based URI (/api/seasons/{seasonNumber})
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Uses standard HTTP methods for CRUD operations
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/seasons/2
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "seasonNumber": "2",
    ///     "episodeCount": 22
    ///   },
    ///   "message": "Season 2 retrieved successfully"
    /// }
    /// </code>
    /// </example>
    [HttpGet("{seasonNumber}")]
    [ProducesResponseType(typeof(ApiResponse<Season>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
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
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = $"Season {seasonNumber} not found",
                    Message = "Season not found"
                });
            }

            return Ok(new ApiResponse<Season>
            {
                Success = true,
                Data = season,
                Message = $"Season {seasonNumber} retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
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
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Error = $"Season parameter is outside of the scope. Please select the season number between 1 and {seasonsCount} (inclusive).",
                Message = "Invalid request"
            });
        }
        return null;
    }
}
