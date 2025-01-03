using CW88.TeleBot.Filters;
using CW88.TeleBot.Model;
using CW88.TeleBot.Services.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace CW88.TeleBot.Controllers;

[ApiController]
[Route("/{botName}")]
public class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromRoute] string botName,
        [FromBody] Update update,
        [FromServices] UpdateQueue updateQueue,
        IOptions<CoreConfig> coreConfig,
        ILogger<BotController> logger,
        CancellationToken cancellationToken)
    {
        var botConfig =
            coreConfig.Value.Bots.FirstOrDefault(o =>
                o.BotName.Equals(botName, StringComparison.InvariantCultureIgnoreCase));
        if (botConfig == null) {
            return NotFound($"Bot '{botName}' not configured.");
        }

        try
        {
            // Enqueue the update for background processing
            updateQueue.Enqueue(update, botName);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enqueuing update for bot '{BotName}': {UpdateId}", botName, update.Id);
            return StatusCode(500, "Failed to enqueue the update.");
        }
    }
}