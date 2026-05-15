namespace Application.DTOs.Archive;

public record PastLangdleGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
    public string TargetName { get; init; } = string.Empty;
}
