using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PhotographersStoryApi
{
    [Route("api/Health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return NoContent();
        }
    }
}
