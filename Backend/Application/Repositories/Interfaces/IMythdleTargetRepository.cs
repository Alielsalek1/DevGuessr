using Domain.Models.MythdleTarget;

namespace Application.Repositories.Interfaces;

public interface IMythdleTargetRepository
{
    Task<IEnumerable<MythdleTarget>> GetPagedAsync(int pageNumber, int pageSize);
    Task<List<MythdleTarget>> GetAllAsync(CancellationToken cancellationToken);
    Task<MythdleTarget?> GetByNameAsync(string name);
    Task AddAsync(MythdleTarget target);
    Task DeleteAsync(MythdleTarget target);
    Task<int> SaveChangesAsync();
}
