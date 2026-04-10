namespace Application.DTOs.DevGuessrPlayer;

public class CreateDevGuessrGameResponseDto
{
    public Guid PuzzleId { get; set; }
    public Guid TargetId { get; set; }
    public string TargetName { get; set; } = null!;
}
