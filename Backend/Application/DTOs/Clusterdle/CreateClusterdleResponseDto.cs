namespace Application.DTOs.Clusterdle;

public record CreateClusterdleResponseDto
{
    public Guid Id { get; init; }
    public string GroupName { get; init; } = null!;
    public int DifficultyLevel { get; init; }
    public List<string> Words { get; init; } = [];
    public string SuccessMessage { get; init; } = null!;
}
