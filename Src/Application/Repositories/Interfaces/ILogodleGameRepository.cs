using Domain.Models.DailyLogodle;

namespace Application.Repositories.Interfaces;

public interface ILogodleGameRepository
{
	Task<DailyLogodle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
	Task<DailyLogodle?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken);
	Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken);
	Task<bool> TryAddAsync(DailyLogodle puzzle, CancellationToken cancellationToken);
}