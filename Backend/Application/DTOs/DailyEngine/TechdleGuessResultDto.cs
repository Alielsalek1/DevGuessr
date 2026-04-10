namespace Application.DTOs.TechdlePlayer;

public class TechdleGuessResultDto
{
    public bool IsCorrect { get; set; }
    public List<AttributeFeedback> AttributeFeedback { get; set; } = [];
}
