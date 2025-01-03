using Microsoft.AspNetCore.Mvc;

namespace W88.TeleBot.Controllers;

[ApiController]
[Route("api")]
public class IndexController(ILogger<IndexController> logger) : ControllerBase
{
    [HttpGet]
    [Route("Ping")]
    public IActionResult Ping()
    {
        logger.LogInformation("Keep-alive request");
        return Ok("Pong!");
    }
}