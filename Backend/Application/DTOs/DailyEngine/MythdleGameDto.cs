namespace Application.DTOs.MythdlePlayer;

public record MythdleGameDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
    public List<MythdlePlayerTargetDto> Targets { get; init; } = [];
}
