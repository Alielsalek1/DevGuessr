namespace Application.DTOs.MythdlePlayer;

public record MythdleGuessRequestDto
{
    public Guid PuzzleId { get; init; }
    public string GuessedTargetName { get; init; } = string.Empty;
}
