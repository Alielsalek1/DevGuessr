namespace Application.DTOs.TechdlePlayer;

public class CreateTechdleGameResponseDto
{
    public Guid PuzzleId { get; set; }
    public Guid TargetId { get; set; }
    public string TargetName { get; set; } = null!;
}
