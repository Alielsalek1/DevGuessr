namespace Application.DTOs.Archive;

public record PastLogodleGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
    public string TargetName { get; init; } = string.Empty;
    public string InitialImageUrl { get; init; } = string.Empty;
}
