namespace Application.DTOs.TechnectionCategory;

public record CreateTechnectionCategoryRequestDto
{
    public string GroupName { get; init; } = null!;
    public int DifficultyLevel { get; init; }
    public List<string> Words { get; init; } = [];
    public string SuccessMessage { get; init; } = null!;
}
