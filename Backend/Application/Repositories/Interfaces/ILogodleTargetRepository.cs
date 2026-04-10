using Domain.Models.LogodleTarget;

namespace Application.Repositories.Interfaces;

public interface ILogodleTargetRepository
{
    Task<(List<LogodleTarget> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<LogodleTarget?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<LogodleTarget?> GetRandomAsync(CancellationToken cancellationToken);
    Task AddAsync(LogodleTarget target, CancellationToken cancellationToken);
    Task<bool> DeleteByNameAsync(string name, CancellationToken cancellationToken);
}
