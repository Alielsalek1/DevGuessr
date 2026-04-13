namespace Application.DTOs.LangdlePlayer;

public class CreateLangdleGameResponseDto
{
    public Guid PuzzleId { get; set; }
    public DateOnly PuzzleDate { get; set; }
    public Guid TargetId { get; set; }
    public string TargetName { get; set; } = null!;
}
