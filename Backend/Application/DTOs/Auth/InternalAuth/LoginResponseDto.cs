using System.Text.Json.Serialization;

namespace Application.DTOs.Auth;
public record LoginResponseDto
{
    public Guid UserId { get; init; }
    [JsonIgnore]
    public Guid DeviceId { get; init; }
    public string AccessToken { get; init; } = null!;
    [JsonIgnore]
    public string RefreshToken { get; init; } = null!;
}