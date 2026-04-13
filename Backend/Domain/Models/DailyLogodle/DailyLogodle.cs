namespace Domain.Models.DailyLogodle;

public class DailyLogodle
{
	public Guid Id { get; private set; } = Guid.NewGuid();
	public DateOnly PuzzleDate { get; private set; }
	public string LogodleTargetName { get; private set; } = null!;
	public LogodleTarget.LogodleTarget Target { get; private set; } = null!;

	private DailyLogodle() { }

	public DailyLogodle(DateOnly puzzleDate, string logodleTargetName)
	{
		PuzzleDate = puzzleDate;
		LogodleTargetName = logodleTargetName;
	}
}