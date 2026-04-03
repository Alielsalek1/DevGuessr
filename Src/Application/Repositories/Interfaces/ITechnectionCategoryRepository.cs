using Domain.Models.TechnectionCategory;

namespace Application.Repositories.Interfaces;

public interface ITechnectionCategoryRepository
{
    Task<(List<TechnectionCategory> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<TechnectionCategory?> GetByGroupNameAsync(string groupName, CancellationToken cancellationToken);
    Task<bool> ExistsByGroupNameAsync(string groupName, CancellationToken cancellationToken);
    Task AddAsync(TechnectionCategory category, CancellationToken cancellationToken);
    Task UpdateAsync(TechnectionCategory category, CancellationToken cancellationToken);
    Task<bool> DeleteByGroupNameAsync(string groupName, CancellationToken cancellationToken);
}
