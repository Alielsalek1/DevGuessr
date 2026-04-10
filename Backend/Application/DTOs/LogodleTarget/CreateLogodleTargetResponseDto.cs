namespace Application.DTOs.LogodleTarget;

public record CreateLogodleTargetResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string ImagePath { get; init; } = null!;
    public List<string> BlurredImageUrls { get; init; } = [];
}
