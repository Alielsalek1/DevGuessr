namespace Application.DTOs.Archive;

public record PastMythdleGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
    public List<string> TargetNames { get; init; } = [];
}
