namespace Application.DTOs.LogodlePlayer;

public record LogodleGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
    public string InitialImageUrl { get; init; } = string.Empty;
}