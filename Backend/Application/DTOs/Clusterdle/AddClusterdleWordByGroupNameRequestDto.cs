namespace Application.DTOs.Clusterdle;

public record AddClusterdleWordByGroupNameRequestDto
{
    public string Word { get; init; } = null!;
}
