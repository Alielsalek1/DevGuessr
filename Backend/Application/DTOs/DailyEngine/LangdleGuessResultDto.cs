namespace Application.DTOs.LangdlePlayer;

public class LangdleGuessResultDto
{
    public bool IsCorrect { get; set; }
    public List<AttributeFeedback> AttributeFeedback { get; set; } = [];
}
