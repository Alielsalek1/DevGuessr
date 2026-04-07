using Application.Repositories.Interfaces;
using Domain.Models.DailyTechdle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class DailyTechdleRepository(AppDbContext dbContext) : IDailyTechdleRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<DailyTechdle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        return await _dbContext.DailyTechdles
            .Include(d => d.TargetLanguage)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.PuzzleDate == date, cancellationToken);
    }
    
}
