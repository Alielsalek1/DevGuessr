using Application.Repositories.Interfaces;
using Domain.Models.DailyTechdle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories.Implementations;

public class TechdleGameRepository(AppDbContext dbContext) : ITechdleGameRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<DailyTechdle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        return await _dbContext.DailyTechdles
            .Include(d => d.TargetLanguage)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.PuzzleDate == date, cancellationToken);
    }

    public async Task<DailyTechdle?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken)
    {
        return await _dbContext.DailyTechdles
            .Include(d => d.TargetLanguage)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == puzzleId, cancellationToken);
    }

    public async Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.DailyTechdles
            .AsNoTracking()
            .OrderByDescending(d => d.PuzzleDate)
            .Select(d => (DateOnly?)d.PuzzleDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> TryAddAsync(DailyTechdle puzzle, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.DailyTechdles.AddAsync(puzzle, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException &&
                                           postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }
    }
}
