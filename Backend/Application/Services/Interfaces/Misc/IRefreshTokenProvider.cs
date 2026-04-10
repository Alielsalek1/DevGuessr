using Domain.Models.UserRefreshTokens;

namespace Application.Services.Interfaces;

public interface IRefreshTokenProvider
{
    string HashRefreshToken(string refreshToken);
    bool IsInvalidRefreshToken(UserRefreshToken? refreshTokenFromDb, string refreshTokenFromCookie);
    string GenerateNewRefreshToken();
}