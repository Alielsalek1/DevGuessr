using Application.Repositories.Interfaces;
using Domain.Models.DailyLogodle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories.Implementations;

public class LogodleGameRepository(AppDbContext dbContext) : ILogodleGameRepository
{
	private readonly AppDbContext _dbContext = dbContext;

	public async Task<DailyLogodle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken)
	{
		return await _dbContext.DailyLogodles
			.Include(d => d.Target)
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.PuzzleDate == date, cancellationToken);
	}

	public async Task<DailyLogodle?> GetByIdAsync(Guid puzzleId, CancellationToken cancellationToken)
	{
		return await _dbContext.DailyLogodles
			.Include(d => d.Target)
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == puzzleId, cancellationToken);
	}

	public async Task<DateOnly?> GetLatestPuzzleDateAsync(CancellationToken cancellationToken)
	{
		return await _dbContext.DailyLogodles
			.AsNoTracking()
			.OrderByDescending(d => d.PuzzleDate)
			.Select(d => (DateOnly?)d.PuzzleDate)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<(List<DailyLogodle> Items, int TotalCount)> GetPastPuzzlesAsync(int skip, int take, CancellationToken cancellationToken)
	{
		var today = DateOnly.FromDateTime(DateTime.UtcNow);
		var query = _dbContext.DailyLogodles
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

	public async Task<bool> TryAddAsync(DailyLogodle puzzle, CancellationToken cancellationToken)
	{
		try
		{
			await _dbContext.DailyLogodles.AddAsync(puzzle, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return true;
		}
		catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException &&
		                                   postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
		{
			return false;
		}
	}

	public async Task<bool> TryAddRangeAsync(IReadOnlyCollection<DailyLogodle> puzzles, CancellationToken cancellationToken)
	{
		if (puzzles.Count == 0)
		{
			return true;
		}

		try
		{
			await _dbContext.DailyLogodles.AddRangeAsync(puzzles, cancellationToken);
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