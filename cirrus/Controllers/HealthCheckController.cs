using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using cirrus.Models;

namespace cirrusRapiapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {

        [HttpGet]
        public IActionResult Check()
        {
           return Ok(new { statusCode = 200, message = "Healthy" });
        }
    }
}