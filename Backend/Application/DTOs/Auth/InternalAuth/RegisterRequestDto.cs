namespace Application.DTOs.Auth;
public record RegisterRequestDto
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
}