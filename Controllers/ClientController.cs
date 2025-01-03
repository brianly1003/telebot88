using CW88.TeleBot.Model;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CW88.TeleBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController(
    ITelegramBotClient botClient,
    IOptions<CoreConfig> options,
    ITelegramAuth telegramAuth,
    ICommandHandler commandHandler)
    : ControllerBase
{
    private readonly CoreConfig _botConfig = options.Value;

    [HttpPost]
    public async Task<IActionResult> Post(ClientRequest request)
    {
        //var isValid = telegramAuth.ValidateData(request.AuthData, _botConfig.BotToken);
        //if (!isValid)
        //{
        //    return Ok(new
        //    {
        //        ok = isValid,
        //        description = "Validation Data Failed",
        //        error = "null"
        //    });
        //}

        var data = telegramAuth.GetData(request.AuthData);
        var userId = data.User?.Id;

        try
        {
            //var createUserResult = await userService.CreateUser(new CreateUserArgs
            //{
            //    TelegramChatId = (long)userId!,
            //    FirstName = data.User?.FirstName ?? string.Empty,
            //    LastName = data.User?.LastName ?? string.Empty,
            //    UserName = data.User?.Username ?? string.Empty
            //});

            //if (!createUserResult.Succeeded)
            //{
            //    return Ok(new
            //    {
            //        ok = false,
            //        description = "Create User Failed",
            //        error = createUserResult.Status.Message
            //    });
            //}
        }
        catch (Exception ex)
        {
            // Handle errors here, e.g. user has blocked the bot, etc.
            Console.WriteLine($"Error sending message to user {userId}: {ex.Message}");
        }

        var result = new
        {
            ok = true, //isValid,
            description = "Success",
        };

        return Ok(result);
    }

    [HttpPost]
    [Route("Action")]
    public async Task<IActionResult> Action(ClientRequest request)
    {
        //var isValid = telegramAuth.ValidateData(request.AuthData, _botConfig.BotToken, _botConfig.ValidationKey);
        //if (!isValid)
        //{
        //    return Ok(new
        //    {
        //        ok = isValid,
        //        description = "Validation Data Failed",
        //        error = "null"
        //    });
        //}

        var data = telegramAuth.GetMiniAppData(request.AuthData);
        var userId = data.User?.Id;

        var command = commandHandler.GetBotCommand(commandName: request.Command);
        if (command == null) return BadRequest();

        await command!.ExecuteAsync(botClient, new Message { Chat = new Chat { Id = (long)userId! }}, null, null, CancellationToken.None );

        var result = new
        {
            ok = true,//isValid,
            description = "Success",
        };

        return Ok(result);
    }
}