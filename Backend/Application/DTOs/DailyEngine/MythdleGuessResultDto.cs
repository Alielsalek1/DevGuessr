namespace Application.DTOs.MythdlePlayer;

public record MythdleGuessResultDto
{
    public bool IsCorrect { get; init; }
    public string? TargetName { get; init; }
}
