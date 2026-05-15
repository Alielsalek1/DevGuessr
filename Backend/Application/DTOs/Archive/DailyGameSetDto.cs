namespace Application.DTOs.Archive;

public record DailyGameSetDto
{
    public DateOnly PuzzleDate { get; init; }
    public PastLangdleGameDto? Langdle { get; init; }
    public PastLogodleGameDto? Logodle { get; init; }
    public PastMythdleGameDto? Mythdle { get; init; }
}
