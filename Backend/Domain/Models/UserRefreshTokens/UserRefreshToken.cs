namespace Domain.Models.UserRefreshTokens;

public class UserRefreshToken
{
    public Guid UserId { get; private set; }
    public string RefreshTokenHash { get; private set; } = null!;
    public bool IsUsed { get; private set; } = false;
    public DateTime? UsedAt { get; private set; }
    public DateTime RefreshTokenExpiryTime { get; private set; } = DateTime.UtcNow.AddDays(30);

    private UserRefreshToken() { }

    public UserRefreshToken(Guid userId, string refreshTokenHash)
    {
        UserRefreshTokenGuard.ValidateUserId(userId);
        UserRefreshTokenGuard.ValidateRefreshTokenHash(refreshTokenHash);

        UserId = userId;
        RefreshTokenHash = refreshTokenHash;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }
}