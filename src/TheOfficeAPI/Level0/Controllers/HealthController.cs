using Microsoft.AspNetCore.Mvc;

namespace TheOfficeAPI.Level0.Controllers;

[ApiController]
[Route("api/v0/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}