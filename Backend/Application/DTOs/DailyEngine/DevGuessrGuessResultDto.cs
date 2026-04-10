namespace Application.DTOs.DevGuessrPlayer;

public class DevGuessrGuessResultDto
{
    public bool IsCorrect { get; set; }
    public List<AttributeFeedback> AttributeFeedback { get; set; } = [];
}
