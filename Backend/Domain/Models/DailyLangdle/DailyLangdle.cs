using Domain.Models.Langdle;

namespace Domain.Models.DailyLangdle;

public class DailyLangdle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateOnly PuzzleDate { get; private set; }
    public string LangdleName { get; private set; } = null!;
    public Domain.Models.Langdle.Langdle TargetLanguage { get; private set; } = null!;

    // EF Core constructor
    private DailyLangdle() { }

    public DailyLangdle(DateOnly puzzleDate, string langdleName)
    {
        PuzzleDate = puzzleDate;
        LangdleName = langdleName;
    }
}
