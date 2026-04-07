using Domain.Models.DailyLogodle;

namespace Application.Repositories.Interfaces;

public interface IDailyLogodleRepository
{
	Task<DailyLogodle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
}