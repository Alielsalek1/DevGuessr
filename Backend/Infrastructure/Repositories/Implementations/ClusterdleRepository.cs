using Application.Repositories.Interfaces;
using Domain.Models.Clusterdle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class ClusterdleRepository(AppDbContext dbContext) : IClusterdleRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<(List<Clusterdle> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.ClusterdleTargets.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(tc => tc.GroupName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Clusterdle?> GetByGroupNameAsync(string groupName, CancellationToken cancellationToken)
    {
        return await _dbContext.ClusterdleTargets
            .FirstOrDefaultAsync(tc => tc.GroupName.ToLower() == groupName.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByGroupNameAsync(string groupName, CancellationToken cancellationToken)
    {
        return await _dbContext.ClusterdleTargets
            .AsNoTracking()
            .AnyAsync(tc => tc.GroupName.ToLower() == groupName.ToLower(), cancellationToken);
    }

    public async Task AddAsync(Clusterdle category, CancellationToken cancellationToken)
    {
        await _dbContext.ClusterdleTargets.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Clusterdle category, CancellationToken cancellationToken)
    {
        _dbContext.ClusterdleTargets.Update(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteByGroupNameAsync(string groupName, CancellationToken cancellationToken)
    {
        var rowsAffected = await _dbContext.ClusterdleTargets
            .Where(tc => tc.GroupName.ToLower() == groupName.ToLower())
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }
}
