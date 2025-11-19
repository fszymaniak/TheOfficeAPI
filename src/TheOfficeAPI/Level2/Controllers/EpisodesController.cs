using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level2.Services;

namespace TheOfficeAPI.Level2.Controllers;

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
    /// Retrieves all episodes for a specific season (Level 2 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>Episodes for the specified season</returns>
    /// <response code="200">Episodes retrieved successfully</response>
    /// <response code="404">Season not found</response>
    /// <remarks>
    /// This endpoint follows Level 2 Richardson Maturity Model:
    /// - Uses resource-based URI (/api/seasons/{seasonNumber}/episodes)
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Uses standard HTTP methods for CRUD operations
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/seasons/1/episodes
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "season": 1,
    ///       "episodeNumber": 1,
    ///       "title": "Pilot",
    ///       "releasedDate": "2005-03-24"
    ///     }
    ///   ],
    ///   "message": "Episodes for season 1 retrieved successfully"
    /// }
    /// </code>
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<Episode>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetSeasonEpisodes([FromRoute] int seasonNumber)
    {
        try
        {
            var seasonRangeValidation = ValidateSeasonRange(seasonNumber);
            if (seasonRangeValidation != null) return seasonRangeValidation;

            var episodes = _theOfficeService.GetSeasonEpisodes(seasonNumber);
            return Ok(new ApiResponse<List<Episode>>
            {
                Success = true,
                Data = episodes,
                Message = $"Episodes for season {seasonNumber} retrieved successfully"
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
    /// Retrieves a specific episode (Level 2 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <param name="episodeNumber">The episode number</param>
    /// <returns>The specified episode</returns>
    /// <response code="200">Episode retrieved successfully</response>
    /// <response code="404">Episode or season not found</response>
    /// <remarks>
    /// This endpoint follows Level 2 Richardson Maturity Model:
    /// - Uses resource-based URI (/api/seasons/{seasonNumber}/episodes/{episodeNumber})
    /// - Uses appropriate HTTP verb (GET for retrieval)
    /// - Returns proper HTTP status codes (200 OK, 404 Not Found)
    /// - Uses standard HTTP methods for CRUD operations
    /// </remarks>
    /// <example>
    /// <code>
    /// GET /api/seasons/2/episodes/1
    ///
    /// Response (200 OK):
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "season": 2,
    ///     "episodeNumber": 1,
    ///     "title": "The Dundies",
    ///     "releasedDate": "2005-09-20"
    ///   },
    ///   "message": "Episode retrieved successfully"
    /// }
    ///
    /// Response (404 Not Found):
    /// {
    ///   "success": false,
    ///   "error": "Episode 99 of season 2 not found",
    ///   "message": "Episode not found"
    /// }
    /// </code>
    /// </example>
    [HttpGet("{episodeNumber}")]
    [ProducesResponseType(typeof(ApiResponse<Episode>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
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
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = $"Episode {episodeNumber} of season {seasonNumber} not found",
                    Message = "Episode not found"
                });
            }

            return Ok(new ApiResponse<Episode>
            {
                Success = true,
                Data = episode,
                Message = "Episode retrieved successfully"
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

    private IActionResult? ValidateEpisodeRangeFromSpecificSeason(int season, int episode)
    {
        var episodesCountFromSpecificSeason = _theOfficeService.GetSeasonEpisodes(season).Count;
        if (episode < 1 || episode > episodesCountFromSpecificSeason)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Error = $"Episode parameter is outside of the scope. Please select the episode number between 1 and {episodesCountFromSpecificSeason} (inclusive).",
                Message = "Invalid request"
            });
        }
        return null;
    }
}
