using CW88.TeleBot.Model;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.ServiceBot.Models;
using CW88.TeleBot.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CW88.TeleBot.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController(
    IJwtTokenService jwtTokenService,
    IHttpContextAccessor contextAccessor,
    ITelegramAuth telegramAuth,
    CoreConfig botConfig)
    : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(TokenRequest request)
    {
        try
        {
            var headers = contextAccessor.HttpContext?.Request.Headers;
            _ = headers!.TryGetValue("Authorization", out var bearerToken);
            var tokenValue = bearerToken.ToString()["Bearer ".Length..].Trim();

            //var isValid = telegramAuth.ValidateData(request.AuthData, botConfig.BotToken, botConfig.ValidationKey);
            //if (!isValid)
            //{
            //    return Unauthorized();
            //}

            var result = jwtTokenService.RefreshAccessToken(tokenValue, request.RefreshToken);

            return Ok(result);
        }
        catch (SecurityTokenException)
        {
            return Unauthorized();
        }
    }

    [HttpPost]
    [Route("Generate")]
    public Task<IActionResult> Token()
    {
        var apiConsumerId = 6589150482;
        var username = "wtv88_bot";

        var result = jwtTokenService.GenerateTokens(apiConsumerId, username);

        return Task.FromResult<IActionResult>(Ok(result));
    }
}