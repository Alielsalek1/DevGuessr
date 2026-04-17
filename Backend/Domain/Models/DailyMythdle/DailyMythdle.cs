namespace Domain.Models.DailyMythdle;

public class DailyMythdle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateOnly PuzzleDate { get; private set; }
    public string MythdleTargetName { get; private set; } = null!;
    public List<string> TargetNames { get; private set; } = [];
    public MythdleTarget.MythdleTarget Target { get; private set; } = null!;

    private DailyMythdle() { }

    public DailyMythdle(DateOnly puzzleDate, string mythdleTargetName, IReadOnlyCollection<string> targetNames)
    {
        PuzzleDate = puzzleDate;
        MythdleTargetName = mythdleTargetName;
        TargetNames = targetNames.ToList();
    }
}
