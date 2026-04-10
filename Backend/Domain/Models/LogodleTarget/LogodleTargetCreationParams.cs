namespace Domain.Models.LogodleTarget;

public record LogodleTargetCreationParams
{
    public required string Name { get; init; }
    public required string ImagePath { get; init; }
    public required List<string> BlurredImageUrls { get; init; }
}
