using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level0.Services;

namespace TheOfficeAPI.Level0.Controllers;

[ApiController]
[Route("api")]
public class Level0Controller : ControllerBase
{
    private readonly OfficeService _officeService;

    public Level0Controller(OfficeService officeService)
    {
        _officeService = officeService;
    }

    /// <summary>
    /// Handles all API requests for The Office data using a single endpoint (Level 0 Richardson Maturity)
    /// </summary>
    /// <param name="request">The API request containing action and parameters</param>
    /// <returns>Always returns HTTP 200 OK with success/failure status in response body</returns>
    /// <response code="200">Request processed (check response body for actual status)</response>
    /// <remarks>
    /// This endpoint follows Level 0 Richardson Maturity Model:
    /// - Single URI endpoint for all operations
    /// - Always returns HTTP 200 OK
    /// - Actual operation status is contained in the response body
    /// - Uses POST method for all operations regardless of intent
    /// 
    /// Supported actions:
    /// - getAllSeasons: Retrieves all available seasons
    /// - getSeasonEpisodes: Gets episodes for a specific season
    /// - getEpisode: Gets details for a specific episode
    /// </remarks>
    /// <example>
    /// <para><strong>Get All Seasons:</strong></para>
    /// <code>
    /// POST /api/theOffice
    /// Content-Type: application/json
    /// 
    /// {
    ///   "action": "getAllSeasons"
    /// }
    /// 
    /// Response:
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "seasonNumber": "1",
    ///       "episodeCount": 6,
    ///       "year": "2005"
    ///     },
    ///     {
    ///       "seasonNumber": "2", 
    ///       "episodeCount": 22,
    ///       "year": "2005-2006"
    ///     }
    ///   ],
    ///   "message": "Seasons retrieved successfully"
    /// }
    /// </code>
    /// 
    /// <para><strong>Get Season Episodes:</strong></para>
    /// <code>
    /// POST /api/theOffice
    /// Content-Type: application/json
    /// 
    /// {
    ///   "action": "getSeasonEpisodes",
    ///   "season": "1"
    /// }
    /// 
    /// Response:
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "seasonNumber": "1",
    ///       "episodeNumber": "1",
    ///       "title": "Pilot",
    ///       "director": "Ken Kwapis",
    ///       "writer": "Ricky Gervais, Stephen Merchant, Greg Daniels"
    ///     }
    ///   ],
    ///   "message": "Episodes for season 1 retrieved successfully"
    /// }
    /// </code>
    /// 
    /// <para><strong>Get Specific Episode:</strong></para>
    /// <code>
    /// POST /api/theOffice
    /// Content-Type: application/json
    /// 
    /// {
    ///   "action": "getEpisode",
    ///   "season": "2",
    ///   "episode": "1"
    /// }
    /// 
    /// Response:
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "seasonNumber": "2",
    ///     "episodeNumber": "1", 
    ///     "title": "The Dundies",
    ///     "director": "Greg Daniels",
    ///     "writer": "Mindy Kaling",
    ///     "airDate": "2005-09-20",
    ///     "summary": "Michael hosts the annual Dundies awards at Chili's."
    ///   },
    ///   "message": "Episode retrieved successfully"
    /// }
    /// </code>
    /// </example>
    [HttpPost("theOffice")]
    public IActionResult HandleRequest([FromBody] ApiRequest request)
    {
        // Level 0: Always return 200 OK, put actual status in response body
        try
        {
            switch (request.Action.ToLower())
            {
                case "getallseasons":
                    var seasons = _officeService.GetAllSeasons();
                    return Ok(new ApiResponse<List<Season>>
                    {
                        Success = true,
                        Data = seasons,
                        Message = "Seasons retrieved successfully"
                    });

                case "getseasonepisodes":
                    if (request.Season == null)
                        return Ok(new ApiResponse<object>
                        {
                            Success = false,
                            Error = "Season parameter is required",
                            Message = "Invalid request"
                        });

                    var episodes = _officeService.GetSeasonEpisodes(request.Season);
                    var seasonsCount = _officeService.GetAllSeasons().Count;

                    if (request.Season > seasonsCount)
                    {
                        
                    }
                    
                    return Ok(new ApiResponse<List<Episode>>
                    {
                        Success = true,
                        Data = episodes,
                        Message = $"Episodes for season {request.Season} retrieved successfully"
                    });

                case "getepisode":
                    if (request.Season == null || request.Episode == null)
                        return Ok(new ApiResponse<object>
                        {
                            Success = false,
                            Error = "Both season and episode parameters are required",
                            Message = "Invalid request"
                        });

                    var episode = _officeService.GetEpisode(request.Season, request.Episode);
                    if (episode == null)
                        return Ok(new ApiResponse<object>
                        {
                            Success = false,
                            Error = $"Episode S{request.Season}E{request.Episode} not found",
                            Message = "Episode not found"
                        });

                    return Ok(new ApiResponse<Episode>
                    {
                        Success = true,
                        Data = episode,
                        Message = "Episode retrieved successfully"
                    });

                default:
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Error = $"Unknown action: {request.Action}",
                        Message = "Invalid action"
                    });
            }
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
}