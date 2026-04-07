using Application.DTOs.TechdlePlayer;
using Application.Repositories.Interfaces;
using Domain.Models.ProgrammingLanguage;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class ProgrammingLanguageRepository(AppDbContext dbContext) : IProgrammingLanguageRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<(List<ProgrammingLanguage> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.ProgrammingLanguages.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(pl => pl.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<ProgrammingLanguage?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.ProgrammingLanguages
            .AsNoTracking()
            .FirstOrDefaultAsync(pl => pl.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.ProgrammingLanguages
            .AsNoTracking()
            .AnyAsync(pl => pl.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task AddAsync(ProgrammingLanguage language, CancellationToken cancellationToken)
    {
        await _dbContext.ProgrammingLanguages.AddAsync(language, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProgrammingLanguage language, CancellationToken cancellationToken)
    {
        _dbContext.ProgrammingLanguages.Update(language);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteByNameAsync(string name, CancellationToken cancellationToken)
    {
        var rowsAffected = await _dbContext.ProgrammingLanguages
            .Where(pl => pl.Name.ToLower() == name.ToLower())
            .ExecuteDeleteAsync(cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<ProgrammingLanguage?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.ProgrammingLanguages
            .AsNoTracking()
            .FirstOrDefaultAsync(pl => pl.Id == id, cancellationToken);
    }

    public async Task<ProgrammingLanguage?> GetRandomAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ProgrammingLanguages
            .AsNoTracking()
            .OrderBy(pl => EF.Functions.Random())
            .FirstOrDefaultAsync(cancellationToken);
    }
}
