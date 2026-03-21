using System.Security.Cryptography;
using System.Text;
using Application.Services.Interfaces;
using Domain.Models.UserRefreshTokens;

namespace Application.Services.Implementations;

public class RefreshTokenProvider() : IRefreshTokenProvider
{
    public string GenerateNewRefreshToken()
    {
        // 32 bytes of randomness provides 256 bits of entropy
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        // Convert to Base64 to make it a URL-safe string for the user
        return Convert.ToBase64String(randomNumber);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var inputBytes = Encoding.UTF8.GetBytes(refreshToken);
        var hashBytes = SHA256.HashData(inputBytes);

        // Convert the byte array to a lowercase 64-character hexadecimal string
        return Convert.ToHexString(hashBytes).ToLower();
    }

    public bool IsInvalidRefreshToken(UserRefreshToken? RefreshTokenFromDb, string refreshTokenFromCookie)
    {
        if (RefreshTokenFromDb == null ||
        RefreshTokenFromDb.RefreshTokenExpiryTime <= DateTime.UtcNow ||
        RefreshTokenFromDb.RefreshTokenHash != HashRefreshToken(refreshTokenFromCookie) ||
        IsRefreshTokenUsedOutsideGracePeriod(RefreshTokenFromDb))
        {
            return true;
        }
        return false;
    }

    private static bool IsRefreshTokenUsedOutsideGracePeriod(UserRefreshToken userRefreshToken)
    {
        return userRefreshToken.IsUsed && userRefreshToken.UsedAt.HasValue && (DateTime.UtcNow - userRefreshToken.UsedAt.Value).TotalSeconds > 40;
    }
}