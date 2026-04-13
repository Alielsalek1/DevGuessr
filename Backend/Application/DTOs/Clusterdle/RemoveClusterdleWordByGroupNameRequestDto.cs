namespace Application.DTOs.Clusterdle;

public record RemoveClusterdleWordByGroupNameRequestDto
{
    public string Word { get; init; } = null!;
}
