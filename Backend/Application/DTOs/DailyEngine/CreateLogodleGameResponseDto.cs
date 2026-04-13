namespace Application.DTOs.LogodlePlayer;

public class CreateLogodleGameResponseDto
{
    public Guid PuzzleId { get; set; }
    public DateOnly PuzzleDate { get; set; }
    public Guid TargetId { get; set; }
    public string TargetName { get; set; } = null!;
}
