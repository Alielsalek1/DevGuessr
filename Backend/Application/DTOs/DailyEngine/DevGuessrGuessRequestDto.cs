namespace Application.DTOs.DevGuessrPlayer;

public record DevGuessrGuessRequestDto
{
    public Guid PuzzleId { get; init; }
    public string GuessedLanguageName { get; init; } = string.Empty;
}
