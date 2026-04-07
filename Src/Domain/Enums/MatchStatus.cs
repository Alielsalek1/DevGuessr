using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MatchStatus
{
    Match,
    Partial,
    Higher,
    Lower,
    Miss
}
