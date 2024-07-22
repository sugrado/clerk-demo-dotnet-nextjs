using ClerkDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClerkDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OAuthController : ControllerBase
    {
        [HttpGet("callback")]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }
    }
}
