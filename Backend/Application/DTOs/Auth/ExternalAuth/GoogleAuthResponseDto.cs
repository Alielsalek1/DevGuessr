using System.Text.Json.Serialization;

namespace Application.DTOs.ExternalAuth;

public record GoogleAuthResponseDto
{
    public Guid UserId { get; init; }
    public string AccessToken { get; init; } = null!;
    [JsonIgnore]
    public string RefreshToken { get; init; } = null!;
}