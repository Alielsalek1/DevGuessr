using Domain.Models.Clusterdle;

namespace Application.Repositories.Interfaces;

public interface IClusterdleRepository
{
    Task<(List<Clusterdle> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Clusterdle?> GetByGroupNameAsync(string groupName, CancellationToken cancellationToken);
    Task<bool> ExistsByGroupNameAsync(string groupName, CancellationToken cancellationToken);
    Task AddAsync(Clusterdle category, CancellationToken cancellationToken);
    Task UpdateAsync(Clusterdle category, CancellationToken cancellationToken);
    Task<bool> DeleteByGroupNameAsync(string groupName, CancellationToken cancellationToken);
}
