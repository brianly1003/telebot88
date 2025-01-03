using CW88.TeleBot.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CW88.TeleBot.Filters;

/// <summary>
/// Check for "X-Telegram-Bot-Api-Secret-Token"
/// Read more: <see href="https://core.telegram.org/bots/api#setwebhook"/> "secret_token"
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidateTelegramBotAttribute() : TypeFilterAttribute(typeof(ValidateTelegramBotFilter))
{
    private class ValidateTelegramBotFilter(IOptions<CoreConfig> options) : IActionFilter
    {
        private readonly CoreConfig _coreConfig = options.Value;

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var routeValues = context.HttpContext.GetRouteData().Values;
            if (!routeValues.TryGetValue("botName", out var botNameObj) || botNameObj == null)
            {
                context.Result = new ObjectResult("Bot name is missing in the route")
                {
                    StatusCode = 400
                };
                return;
            }

            var botName = botNameObj.ToString();
            var botConfig = _coreConfig.Bots.FirstOrDefault(b => string.Equals(b.BotName, botName, StringComparison.InvariantCultureIgnoreCase));
            if (botConfig == null)
            {
                context.Result = new ObjectResult($"Bot '{botName}' is not configured")
                {
                    StatusCode = 404
                };
                return;
            }

            // Validate the "X-Telegram-Bot-Api-Secret-Token" header
            if (!IsValidRequest(context.HttpContext.Request, botConfig.SecretToken))
            {
                context.Result = new ObjectResult("\"X-Telegram-Bot-Api-Secret-Token\" is invalid")
                {
                    StatusCode = 403
                };
            }
        }

        private bool IsValidRequest(HttpRequest request, string secretToken)
        {
            var isSecretTokenProvided =
                request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var secretTokenHeader);
            if (!isSecretTokenProvided) return false;

            return string.Equals(secretTokenHeader, secretToken, StringComparison.Ordinal);
        }
    }
}