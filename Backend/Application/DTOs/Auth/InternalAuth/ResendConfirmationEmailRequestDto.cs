namespace Application.DTOs.Auth;

public record ResendConfirmationEmailRequestDto
{
    public string Email { get; init; } = default!;
}