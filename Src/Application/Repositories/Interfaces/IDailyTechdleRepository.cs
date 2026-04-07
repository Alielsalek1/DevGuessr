using Domain.Models.DailyTechdle;

namespace Application.Repositories.Interfaces;

public interface IDailyTechdleRepository
{
    Task<DailyTechdle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
}
