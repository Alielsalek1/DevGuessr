namespace Application.DTOs.LogodlePlayer;

public record LogodleGuessRequestDto
{
	public Guid PuzzleId { get; init; }
	public string GuessedTargetName { get; init; } = string.Empty;
	public int AttemptNumber { get; init; }
}