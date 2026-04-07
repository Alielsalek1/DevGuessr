using Application.Repositories.Interfaces;
using Domain.Models.DailyLogodle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class DailyLogodleRepository(AppDbContext dbContext) : IDailyLogodleRepository
{
	private readonly AppDbContext _dbContext = dbContext;

	public async Task<DailyLogodle?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken)
	{
		return await _dbContext.DailyLogodles
			.Include(d => d.Target)
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.PuzzleDate == date, cancellationToken);
	}
}