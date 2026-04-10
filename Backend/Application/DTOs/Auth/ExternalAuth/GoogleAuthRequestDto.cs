namespace Application.DTOs.ExternalAuth;

public record GoogleAuthRequestDto
{
    public string IdToken { get; init; } = null!;
}