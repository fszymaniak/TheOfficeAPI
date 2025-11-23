using Microsoft.AspNetCore.Mvc;
using TheOfficeAPI.Common.Models;
using TheOfficeAPI.Level1.Services;

namespace TheOfficeAPI.Level1.Controllers;

[ApiController]
[Route("api/v1/seasons")]
public class SeasonsController : ControllerBase
{
    private readonly TheOfficeService _theOfficeService;

    public SeasonsController(TheOfficeService theOfficeService)
    {
        _theOfficeService = theOfficeService;
    }

    /// <summary>
    /// Retrieves all seasons (Level 1 Richardson Maturity)
    /// </summary>
    /// <returns>All available seasons</returns>
    /// <response code="200">Seasons retrieved successfully</response>
    /// <remarks>
    /// This endpoint follows Level 1 Richardson Maturity Model:
    /// - Uses resource-based URI (/api/seasons)
    /// - Still uses POST method for all operations
    /// - Always returns HTTP 200 OK
    /// - Actual operation status is contained in the response body
    /// </remarks>
    /// <example>
    /// <code>
    /// POST /api/seasons
    /// Content-Type: application/json
    ///
    /// Response:
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
    [HttpPost]
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
            return Ok(new ApiResponse<object>
            {
                Success = false,
                Error = ex.Message,
                Message = "An error occurred while processing the request"
            });
        }
    }
}
