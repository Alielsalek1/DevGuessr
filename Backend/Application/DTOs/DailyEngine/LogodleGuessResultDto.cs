namespace Application.DTOs.LogodlePlayer;

public record LogodleGuessResultDto
{
	public bool IsCorrect { get; init; }
	public bool IsGameOver { get; init; }
	public int AttemptNumber { get; init; }
	public string RevealedImageUrl { get; init; } = string.Empty;
	public string? TargetName { get; init; }
}