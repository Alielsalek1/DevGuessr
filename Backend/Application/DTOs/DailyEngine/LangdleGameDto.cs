namespace Application.DTOs.LangdlePlayer;

public record LangdleGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
}