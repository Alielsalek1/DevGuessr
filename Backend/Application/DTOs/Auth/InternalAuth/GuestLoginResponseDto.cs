using System.Text.Json.Serialization;

namespace Application.DTOs.Auth;

public class GuestLoginResponseDto
{
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = null!;
    [JsonIgnore]
    public string RefreshToken { get; set; } = null!;
}