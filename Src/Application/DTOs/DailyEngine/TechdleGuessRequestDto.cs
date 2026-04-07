namespace Application.DTOs.TechdlePlayer;

public record TechdleGuessRequestDto
{
    public Guid PuzzleId { get; init; }
    public Guid GuessedLanguageId { get; init; }
}
