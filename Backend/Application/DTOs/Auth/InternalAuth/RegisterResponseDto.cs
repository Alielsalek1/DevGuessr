namespace Application.DTOs.Auth;
public record RegisterResponseDto
{
    public Guid UserId { get; init; }
}