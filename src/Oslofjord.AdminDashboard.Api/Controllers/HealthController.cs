using Microsoft.AspNetCore.Mvc;

namespace Oslofjord.AdminDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    
    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    public ActionResult<object> GetHealth()
    {
        return Ok(new
        {
            status = "healthy",
            service = "AdminDashboard-API",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }
}
