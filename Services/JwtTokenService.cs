using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using W88.TeleBot.Model;
using W88.TeleBot.Services.Interfaces;

namespace W88.TeleBot.Services;

public class JwtTokenService(ICacheService cacheService, IOptions<CoreConfig> options) : IJwtTokenService
{
    private readonly CoreConfig _botConfig = options.Value;

    public async Task StoreRefreshToken(string accessToken, string refreshToken)
    {
        await cacheService.SetAsync(accessToken, refreshToken);
    }

    public async Task<bool> CheckRefreshTokenInDatabase(string accessToken, string refreshToken)
    {
        var data = await cacheService.GetAsync(accessToken, typeof(string));
        if (data == null) return false;

        return (string)data == refreshToken;
    }

    public (string AccessToken, string RefreshToken) GenerateTokens(long apiConsumerId, string username)
    {
        var accessTokenExpire = DateTime.UtcNow.AddMinutes(15); // example: 15 minutes
        var accessToken = GenerateJwtToken(apiConsumerId, username, accessTokenExpire);
        var refreshToken = Guid.NewGuid().ToString("N");

        // Store the association between refreshToken and accessToken in the backend.
        StoreRefreshToken(accessToken, refreshToken);

        return (accessToken, refreshToken);
    }

    public (string NewAccessToken, string NewRefreshToken) RefreshAccessToken(string oldAccessToken, string refreshToken)
    {
        if (!IsValidRefreshTokenForAccessToken(oldAccessToken, refreshToken))
        {
            throw new SecurityTokenException("Invalid refresh token or access token.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_botConfig.JwtPublicKey);

        // Extract claims from the old access token
        SecurityToken validatedToken;
        var principal = tokenHandler.ValidateToken(oldAccessToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        }, out validatedToken);

        if (!principal.Identity!.IsAuthenticated)
        {
            throw new SecurityTokenException("Invalid refresh token or access token.");
        }

        // Remove the old refresh token
        RemoveRefreshToken(oldAccessToken);

        var apiConsumerId = long.Parse(principal.FindFirst("id").Value);
        var username = principal.FindFirst(ClaimTypes.Name).Value;

        // Generate a new access token using old token's claims
        var newAccessToken = GenerateJwtToken(apiConsumerId, username, DateTime.UtcNow.AddMinutes(15));

        // Generate a new refresh token
        var newRefreshToken = Guid.NewGuid().ToString();

        // Store the association between the new refresh token and the new access token in your backend.
        StoreRefreshToken(newAccessToken, newRefreshToken);

        return (newAccessToken, newRefreshToken);
    }

    private string GenerateJwtToken(long apiConsumerId, string username, DateTime tokenExpire, bool isRefreshToken = false)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_botConfig.JwtPublicKey);

        var claims = new List<Claim>
        {
            new Claim("id", apiConsumerId.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        // If it's a refresh token, we can add a specific claim to distinguish it
        if (isRefreshToken)
        {
            claims.Add(new Claim("refreshToken", "true"));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = tokenExpire,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private bool IsValidRefreshTokenForAccessToken(string accessToken, string refreshToken)
    {
        // Check if the refreshToken is valid for the provided accessToken in your backend.
        // This can be a database lookup or however you have stored the association.
        return CheckRefreshTokenInDatabase(accessToken, refreshToken).GetAwaiter().GetResult();
    }

    private async Task RemoveRefreshToken(string accessToken)
    {
        await cacheService.RemoveAsync(accessToken);
    }
}