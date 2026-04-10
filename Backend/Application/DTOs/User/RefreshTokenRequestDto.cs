namespace Application.DTOs.User;

public record RefreshTokenRequestDto
{
    public Guid UserId { get; init; }
}