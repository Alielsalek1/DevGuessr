using Domain.Models.DailyTechdle;

namespace Application.Repositories.Interfaces;

public interface ITechdleGameRepository
{
    Task<DailyTechdle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
    Task<DailyTechdle?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken);
    Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken);
    Task<bool> TryAddAsync(DailyTechdle puzzle, CancellationToken cancellationToken);
}
