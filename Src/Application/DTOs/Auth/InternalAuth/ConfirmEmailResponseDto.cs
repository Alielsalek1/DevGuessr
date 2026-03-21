using System.Text.Json.Serialization;

namespace Application.DTOs.Auth;

public record ConfirmEmailResponseDto
{
    [JsonIgnore]
    public Guid DeviceId { get; init; }
}

