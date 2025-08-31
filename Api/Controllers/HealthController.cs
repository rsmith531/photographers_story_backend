// Api/Controllers/HealthController.cs

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("health")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Server health check")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Get()
    {
        return NoContent();
    }
}

