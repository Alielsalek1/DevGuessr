using Domain.Enums;

namespace Domain.Models.User;
public record UserCreationParams
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required Roles Role { get; init; }
    public string? Address { get; init; }
    public string? PhoneNumber { get; init; }
}