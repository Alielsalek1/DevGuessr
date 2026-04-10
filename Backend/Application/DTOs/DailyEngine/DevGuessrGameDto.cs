namespace Application.DTOs.DevGuessrPlayer;

public record DevGuessrGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
}