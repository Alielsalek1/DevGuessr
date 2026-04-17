namespace Application.DTOs.MythdleTarget;

public record CreateMythdleTargetDto
{
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required bool IsFake { get; init; }
    public required string Description { get; init; }
}
