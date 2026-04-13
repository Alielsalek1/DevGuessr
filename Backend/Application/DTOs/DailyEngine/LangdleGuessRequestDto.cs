namespace Application.DTOs.LangdlePlayer;

public record LangdleGuessRequestDto
{
    public Guid PuzzleId { get; init; }
    public string GuessedLanguageName { get; init; } = string.Empty;
}
