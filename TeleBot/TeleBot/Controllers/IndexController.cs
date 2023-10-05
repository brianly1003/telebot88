using Microsoft.AspNetCore.Mvc;

namespace TeleBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Ping");
        }
    }
}
