using Domain.Models.DailyMythdle;

namespace Application.Repositories.Interfaces;

public interface IMythdleGameRepository
{
    Task<DailyMythdle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
    Task<DailyMythdle?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken);
    Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken);
    Task<(List<DailyMythdle> Items, int TotalCount)> GetPastPuzzlesAsync(int skip, int take, CancellationToken cancellationToken);
    Task<bool> TryAddAsync(DailyMythdle puzzle, CancellationToken cancellationToken);
    Task<bool> TryAddRangeAsync(IReadOnlyCollection<DailyMythdle> puzzles, CancellationToken cancellationToken);
}
