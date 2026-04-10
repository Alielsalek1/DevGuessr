using Application.Repositories.Interfaces;
using Domain.Models.TechnectionCategory;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class TechnectionCategoryRepository(AppDbContext dbContext) : ITechnectionCategoryRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<(List<TechnectionCategory> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.TechnectionCategories.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(tc => tc.GroupName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<TechnectionCategory?> GetByGroupNameAsync(string groupName, CancellationToken cancellationToken)
    {
        return await _dbContext.TechnectionCategories
            .FirstOrDefaultAsync(tc => tc.GroupName.ToLower() == groupName.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByGroupNameAsync(string groupName, CancellationToken cancellationToken)
    {
        return await _dbContext.TechnectionCategories
            .AsNoTracking()
            .AnyAsync(tc => tc.GroupName.ToLower() == groupName.ToLower(), cancellationToken);
    }

    public async Task AddAsync(TechnectionCategory category, CancellationToken cancellationToken)
    {
        await _dbContext.TechnectionCategories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TechnectionCategory category, CancellationToken cancellationToken)
    {
        _dbContext.TechnectionCategories.Update(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteByGroupNameAsync(string groupName, CancellationToken cancellationToken)
    {
        var rowsAffected = await _dbContext.TechnectionCategories
            .Where(tc => tc.GroupName.ToLower() == groupName.ToLower())
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }
}
