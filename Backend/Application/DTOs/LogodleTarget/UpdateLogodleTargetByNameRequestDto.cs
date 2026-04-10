namespace Application.DTOs.LogodleTarget;

public record UpdateLogodleTargetByNameRequestDto
{
    public string? Name { get; init; }
    public string? ImagePath { get; init; }
    public List<string>? BlurredImageUrls { get; init; }
}
