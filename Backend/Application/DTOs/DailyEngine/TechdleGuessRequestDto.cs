namespace Application.DTOs.TechdlePlayer;

public record TechdleGuessRequestDto
{
    public Guid PuzzleId { get; init; }
    public string GuessedLanguageName { get; init; } = string.Empty;
}
