using Application.DTOs.DevGuessrPlayer;
using Domain.Models.ProgrammingLanguage;

namespace Application.Repositories.Interfaces;

public interface IProgrammingLanguageRepository
{
    Task<(List<ProgrammingLanguage> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<ProgrammingLanguage?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(ProgrammingLanguage language, CancellationToken cancellationToken);
    Task UpdateAsync(ProgrammingLanguage language, CancellationToken cancellationToken);
    Task<bool> DeleteByNameAsync(string name, CancellationToken cancellationToken);
    Task<ProgrammingLanguage?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProgrammingLanguage?> GetRandomAsync(CancellationToken cancellationToken);
}
