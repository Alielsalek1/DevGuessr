namespace Domain.Models.DailyLogodle;

public class DailyLogodle
{
	public Guid Id { get; private set; } = Guid.NewGuid();
	public DateOnly PuzzleDate { get; private set; }
	public Guid LogodleTargetId { get; private set; }
	public LogodleTarget.LogodleTarget Target { get; private set; } = null!;

	private DailyLogodle() { }

	public DailyLogodle(DateOnly puzzleDate, Guid logodleTargetId)
	{
		PuzzleDate = puzzleDate;
		LogodleTargetId = logodleTargetId;
	}
}