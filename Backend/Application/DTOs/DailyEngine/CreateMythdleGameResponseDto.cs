using Application.DTOs.MythdleTarget;

namespace Application.DTOs.MythdlePlayer;

public record CreateMythdleGameResponseDto
{
    public Guid PuzzleId { get; init; }
    public DateOnly PuzzleDate { get; init; }
    public List<MythdleTargetDto> Targets { get; init; } = [];
}
