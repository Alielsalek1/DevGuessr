namespace Application.DTOs.Auth;
public record ConfirmEmailRequestDto
{
    public string Otp { get; init; } = default!;
}