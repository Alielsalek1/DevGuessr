namespace Application.DTOs.Auth;

public record ResetPasswordRequestDto
{
    public string Otp { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
}