namespace Domain.Models.MythdleTarget;

public record MythdleTargetCreationParams
{
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required bool IsFake { get; init; }
    public required string Description { get; init; }
}
