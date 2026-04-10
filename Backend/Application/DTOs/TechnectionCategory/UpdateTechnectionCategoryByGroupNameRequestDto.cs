namespace Application.DTOs.TechnectionCategory;

/// <summary>
/// All fields are optional. Words are NOT included — use the dedicated
/// word endpoints (POST/DELETE /{groupName}/words) to modify words.
/// </summary>
public record UpdateTechnectionCategoryByGroupNameRequestDto
{
    public string? GroupName { get; init; }
    public int? DifficultyLevel { get; init; }
    public string? SuccessMessage { get; init; }
}
