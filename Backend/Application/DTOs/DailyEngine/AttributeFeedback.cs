using Domain.Enums;

namespace Application.DTOs.LangdlePlayer;

public class AttributeFeedback
{
    public string AttributeName { get; set; } = null!;
    public string GuessedValue { get; set; } = null!;
    public MatchStatus Status { get; set; }
}
