using Domain.Models.ProgrammingLanguage;

namespace Domain.Models.DailyTechdle;

public class DailyTechdle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateOnly PuzzleDate { get; private set; }
    public Guid ProgrammingLanguageId { get; private set; }
    public Domain.Models.ProgrammingLanguage.ProgrammingLanguage TargetLanguage { get; private set; } = null!;

    // EF Core constructor
    private DailyTechdle() { }

    public DailyTechdle(DateOnly puzzleDate, Guid programmingLanguageId)
    {
        PuzzleDate = puzzleDate;
        ProgrammingLanguageId = programmingLanguageId;
    }
}
