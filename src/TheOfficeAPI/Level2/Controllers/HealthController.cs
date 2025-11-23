using Microsoft.AspNetCore.Mvc;

namespace TheOfficeAPI.Level2.Controllers;

[ApiController]
[Route("api/v2/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}
