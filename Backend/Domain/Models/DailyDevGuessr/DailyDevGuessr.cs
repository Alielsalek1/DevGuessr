using Domain.Models.ProgrammingLanguage;

namespace Domain.Models.DailyDevGuessr;

public class DailyDevGuessr
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateOnly PuzzleDate { get; private set; }
    public Guid ProgrammingLanguageId { get; private set; }
    public Domain.Models.ProgrammingLanguage.ProgrammingLanguage TargetLanguage { get; private set; } = null!;

    // EF Core constructor
    private DailyDevGuessr() { }

    public DailyDevGuessr(DateOnly puzzleDate, Guid programmingLanguageId)
    {
        PuzzleDate = puzzleDate;
        ProgrammingLanguageId = programmingLanguageId;
    }
}
