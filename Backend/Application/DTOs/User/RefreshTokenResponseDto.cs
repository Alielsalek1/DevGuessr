using System.Text.Json.Serialization;

namespace Application.DTOs.User;

public record RefreshTokenResponseDto
{
    public string AccessToken { get; init; } = null!;
    [JsonIgnore]
    public string RefreshToken { get; init; } = null!;
}