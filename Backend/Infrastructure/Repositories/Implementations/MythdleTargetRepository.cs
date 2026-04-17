using Application.Repositories.Interfaces;
using Domain.Models.MythdleTarget;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class MythdleTargetRepository : IMythdleTargetRepository
{
    private readonly AppDbContext _context;

    public MythdleTargetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MythdleTarget>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.MythdleTargets
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<MythdleTarget>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.MythdleTargets
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<MythdleTarget?> GetByNameAsync(string name)
    {
        return await _context.MythdleTargets
            .FirstOrDefaultAsync(m => m.Name == name);
    }

    public async Task AddAsync(MythdleTarget target)
    {
        await _context.MythdleTargets.AddAsync(target);
    }

    public Task DeleteAsync(MythdleTarget target)
    {
        _context.MythdleTargets.Remove(target);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
