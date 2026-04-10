namespace Application.DTOs.User;

public record GetUserProfileResponseDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Address { get; init; } = null!;
}