using Domain.Models.DailyLangdle;

namespace Application.Repositories.Interfaces;

public interface ILangdleGameRepository
{
    Task<DailyLangdle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
    Task<DailyLangdle?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken);
    Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken);
    Task<bool> TryAddAsync(DailyLangdle puzzle, CancellationToken cancellationToken);
    Task<bool> TryAddRangeAsync(IReadOnlyCollection<DailyLangdle> puzzles, CancellationToken cancellationToken);
}
