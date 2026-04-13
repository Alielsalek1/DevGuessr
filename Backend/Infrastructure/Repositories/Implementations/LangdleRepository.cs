using Application.DTOs.Langdle;
using Application.Repositories.Interfaces;
using LangdleModel = Domain.Models.Langdle.Langdle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class LangdleRepository(AppDbContext dbContext) : ILangdleRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<(List<LangdleModel> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.LangdleTargets.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(pl => pl.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<LangdleModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.LangdleTargets
            .AsNoTracking()
            .OrderBy(pl => pl.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<LangdleModel?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.LangdleTargets
            .AsNoTracking()
            .FirstOrDefaultAsync(pl => pl.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.LangdleTargets
            .AsNoTracking()
            .AnyAsync(pl => pl.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task AddAsync(LangdleModel language, CancellationToken cancellationToken)
    {
        await _dbContext.LangdleTargets.AddAsync(language, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(LangdleModel language, CancellationToken cancellationToken)
    {
        _dbContext.LangdleTargets.Update(language);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteByNameAsync(string name, CancellationToken cancellationToken)
    {
        var rowsAffected = await _dbContext.LangdleTargets
            .Where(pl => pl.Name.ToLower() == name.ToLower())
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<LangdleModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.LangdleTargets
            .AsNoTracking()
            .FirstOrDefaultAsync(pl => pl.Id == id, cancellationToken);
    }

    public async Task<LangdleModel?> GetRandomAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.LangdleTargets
            .AsNoTracking()
            .OrderBy(pl => EF.Functions.Random())
            .FirstOrDefaultAsync(cancellationToken);
    }
}
