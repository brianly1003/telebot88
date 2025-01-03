namespace CW88.TeleBot.Services.Interfaces;

public interface IJwtTokenService
{
    Task StoreRefreshToken(string accessToken, string refreshToken);

    Task<bool> CheckRefreshTokenInDatabase(string accessToken, string refreshToken);

    (string AccessToken, string RefreshToken) GenerateTokens(long apiConsumerId, string username);

    (string NewAccessToken, string NewRefreshToken) RefreshAccessToken(string oldAccessToken, string refreshToken);
}