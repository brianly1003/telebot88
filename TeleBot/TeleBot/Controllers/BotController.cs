using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeleBot.Services;
using Telegram.Bot.Types;

namespace TeleBot.Controllers
{
    public class BotController : ControllerBase
    {
        [HttpPost]
        //[ValidateTelegramBot]
        public async Task<IActionResult> Post(
            [FromBody] Update update,
            [FromServices] UpdateHandlers handleUpdateService,
            CancellationToken cancellationToken)
        {
            await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
            return Ok();
        }
    }
}
