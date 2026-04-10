namespace Application.DTOs.Auth;

public record ForgetPasswordRequestDto
{
    public string Email { get; init; } = null!;
}