using Domain.Exceptions;

namespace Domain.Models.UserRefreshTokens;

public static class UserRefreshTokenGuard
{
    public static void ValidateRefreshTokenHash(string refreshTokenHash)
    {
        NotNullOrEmpty(refreshTokenHash, nameof(refreshTokenHash));
        
        if (refreshTokenHash.Length != 64) // Assuming SHA-256 hash
            throw new DomainException(
                $"Refresh token hash must be exactly 64 characters.",
                nameof(refreshTokenHash));
    }

    public static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID cannot be empty.", nameof(userId));
    }

    private static void NotNullOrEmpty(string value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
            throw new DomainException($"{parameterName} cannot be null or empty.", parameterName);
    }
}