using Application.DTOs.Langdle;
using LangdleModel = Domain.Models.Langdle.Langdle;

namespace Application.Repositories.Interfaces;

public interface ILangdleRepository
{
    Task<(List<LangdleModel> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<List<LangdleModel>> GetAllAsync(CancellationToken cancellationToken);
    Task<LangdleModel?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(LangdleModel language, CancellationToken cancellationToken);
    Task UpdateAsync(LangdleModel language, CancellationToken cancellationToken);
    Task<bool> DeleteByNameAsync(string name, CancellationToken cancellationToken);
    Task<LangdleModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<LangdleModel?> GetRandomAsync(CancellationToken cancellationToken);
}
