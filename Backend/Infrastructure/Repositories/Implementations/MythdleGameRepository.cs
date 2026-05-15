using Application.Repositories.Interfaces;
using Domain.Models.DailyMythdle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories.Implementations;

public class MythdleGameRepository(AppDbContext dbContext) : IMythdleGameRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<DailyMythdle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        return await _dbContext.DailyMythdles
            .Include(d => d.Target)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.PuzzleDate == date, cancellationToken);
    }

    public async Task<DailyMythdle?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken)
    {
        return await _dbContext.DailyMythdles
            .Include(d => d.Target)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == puzzleId, cancellationToken);
    }

    public async Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.DailyMythdles
            .AsNoTracking()
            .OrderByDescending(d => d.PuzzleDate)
            .Select(d => (DateOnly?)d.PuzzleDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(List<DailyMythdle> Items, int TotalCount)> GetPastPuzzlesAsync(int skip, int take, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = _dbContext.DailyMythdles
            .Where(d => d.PuzzleDate < today)
            .Include(d => d.Target)
            .AsNoTracking();
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(d => d.PuzzleDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    public async Task<bool> TryAddAsync(DailyMythdle puzzle, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.DailyMythdles.AddAsync(puzzle, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException &&
                                           postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }
    }

    public async Task<bool> TryAddRangeAsync(IReadOnlyCollection<DailyMythdle> puzzles, CancellationToken cancellationToken)
    {
        if (puzzles.Count == 0)
        {
            return true;
        }

        try
        {
            await _dbContext.DailyMythdles.AddRangeAsync(puzzles, cancellationToken);
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
