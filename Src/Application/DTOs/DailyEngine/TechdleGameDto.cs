namespace Application.DTOs.TechdlePlayer;

public record TechdleGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
}