using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level1.Services;

namespace TheOfficeAPI.Level1.Controllers;

[ApiController]
[Route("api/v1/seasons/{seasonNumber}/episodes")]
public class EpisodesController : ControllerBase
{
    private readonly TheOfficeService _theOfficeService;

    public EpisodesController(TheOfficeService theOfficeService)
    {
        _theOfficeService = theOfficeService;
    }

    /// <summary>
    /// Retrieves all episodes for a specific season (Level 1 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>Episodes for the specified season</returns>
    /// <response code="200">Episodes retrieved successfully</response>
    /// <remarks>
    /// This endpoint follows Level 1 Richardson Maturity Model:
    /// - Uses resource-based URI (/api/seasons/{seasonNumber}/episodes)
    /// - Still uses POST method for all operations
    /// - Always returns HTTP 200 OK
    /// - Actual operation status is contained in the response body
    /// </remarks>
    /// <example>
    /// <code>
    /// POST /api/seasons/1/episodes
    /// Content-Type: application/json
    ///
    /// Response:
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
    [HttpPost]
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
            return Ok(new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Message = "An error occurred while processing the request"
            });
        }
    }

    /// <summary>
    /// Retrieves a specific episode (Level 1 Richardson Maturity)
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <param name="episodeNumber">The episode number</param>
    /// <returns>The specified episode</returns>
    /// <response code="200">Episode retrieved successfully</response>
    /// <remarks>
    /// This endpoint follows Level 1 Richardson Maturity Model:
    /// - Uses resource-based URI (/api/seasons/{seasonNumber}/episodes/{episodeNumber})
    /// - Still uses POST method for all operations
    /// - Always returns HTTP 200 OK
    /// - Actual operation status is contained in the response body
    /// </remarks>
    /// <example>
    /// <code>
    /// POST /api/seasons/2/episodes/1
    /// Content-Type: application/json
    ///
    /// Response:
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
    /// </code>
    /// </example>
    [HttpPost("{episodeNumber}")]
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
                return Ok(new ApiResponse<object>
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
            return Ok(new ApiResponse<object>
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
            return Ok(new ApiResponse<object>
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
            return Ok(new ApiResponse<object>
            {
                Success = false,
                Error = $"Episode parameter is outside of the scope. Please select the episode number between 1 and {episodesCountFromSpecificSeason} (inclusive).",
                Message = "Invalid request"
            });
        }
        return null;
    }
}
