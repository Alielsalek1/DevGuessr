using Domain.Enums;

namespace Application.DTOs.MythdleTarget;

public record MythdleTargetDto
{
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required bool IsFake { get; init; }
    public required string Description { get; init; }
    public required MythdleDifficulty Difficulty { get; init; }
}
