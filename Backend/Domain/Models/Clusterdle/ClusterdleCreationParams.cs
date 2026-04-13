namespace Domain.Models.Clusterdle;

public record ClusterdleCreationParams
{
    public required string GroupName { get; init; }
    public required int DifficultyLevel { get; init; }
    public required List<string> Words { get; init; }
    public required string SuccessMessage { get; init; }
}
