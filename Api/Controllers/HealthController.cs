using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/Health")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    [EndpointSummary("A simple health check to make sure the server is online")]
    public IActionResult Get()
    {
        return NoContent();
    }
}

