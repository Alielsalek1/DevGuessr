using Application.Repositories.Interfaces;
using Domain.Models.LogodleTarget;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class LogodleTargetRepository(AppDbContext dbContext) : ILogodleTargetRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<(List<LogodleTarget> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.LogodleTargets.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(lt => lt.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<LogodleTarget?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.LogodleTargets
            .AsNoTracking()
            .FirstOrDefaultAsync(lt => lt.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.LogodleTargets
            .AsNoTracking()
            .AnyAsync(lt => lt.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task AddAsync(LogodleTarget target, CancellationToken cancellationToken)
    {
        await _dbContext.LogodleTargets.AddAsync(target, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteByNameAsync(string name, CancellationToken cancellationToken)
    {
        var rowsAffected = await _dbContext.LogodleTargets
            .Where(lt => lt.Name.ToLower() == name.ToLower())
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }
}
