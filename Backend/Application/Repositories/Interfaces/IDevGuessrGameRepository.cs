using Domain.Models.DailyDevGuessr;

namespace Application.Repositories.Interfaces;

public interface IDevGuessrGameRepository
{
    Task<DailyDevGuessr?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
    Task<DailyDevGuessr?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken);
    Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken);
    Task<bool> TryAddAsync(DailyDevGuessr puzzle, CancellationToken cancellationToken);
}
