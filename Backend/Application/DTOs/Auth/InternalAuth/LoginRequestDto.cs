namespace Application.DTOs.Auth;
public record LoginRequestDto
{
    public string UsernameOrEmail { get; init; } = null!;
    public string Password { get; init; } = null!;
}