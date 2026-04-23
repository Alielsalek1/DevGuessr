namespace Application.DTOs.MythdlePlayer;

public record MythdlePlayerTargetDto
{
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required string Description { get; init; }
}
