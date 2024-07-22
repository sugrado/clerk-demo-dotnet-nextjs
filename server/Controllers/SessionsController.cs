using ClerkDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClerkDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController(SessionService sessionService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }
    }
}
