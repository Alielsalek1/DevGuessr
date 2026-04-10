using Application.Constants.Successes;
using Application.DTOs.TechnectionCategory;
using Application.Repositories.Interfaces;
using DevGuessr.Application.Services.Interfaces.TechnectionCategory;
using Application.Utils;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using DevGuessr.Application.Constants.Errors;
using Domain.Models.TechnectionCategory;

namespace Application.Services.Implementations.TechnectionCategory;

public class TechnectionCategoryService(
    ITechnectionCategoryRepository repository,
    ILogger<TechnectionCategoryService> logger) : ITechnectionCategoryService
{
    private readonly ITechnectionCategoryRepository _repository = repository;
    private readonly ILogger<TechnectionCategoryService> _logger = logger;

    public async Task<Result<SuccessApiResponse<GetPagedTechnectionCategoriesResponseDto>>> GetPagedAsync(
        GetPagedTechnectionCategoriesRequestDto request, CancellationToken ct)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.PageNumber, request.PageSize, ct);
        _logger.LogInformation("Fetched {Count} technection categories (Page {PageNumber})", items.Count, request.PageNumber);

        var dtos = items.Select(MapToGetByGroupNameDto).ToList();
        return TechnectionCategorySuccesses.CategoriesFetchedPaged(new GetPagedTechnectionCategoriesResponseDto
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    public async Task<Result<SuccessApiResponse<GetTechnectionCategoryByGroupNameResponseDto>>> GetByGroupNameAsync(
        string groupName, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found", groupName);
            return Result<SuccessApiResponse<GetTechnectionCategoryByGroupNameResponseDto>>.Failure(TechnectionCategoryErrors.CategoryNotFound);
        }

        _logger.LogInformation("Fetched technection category {GroupName}", category.GroupName);
        return TechnectionCategorySuccesses.CategoryFetched(MapToGetByGroupNameDto(category));
    }

    public async Task<Result<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>> CreateAsync(
        CreateTechnectionCategoryRequestDto request, CancellationToken ct)
    {
        var existsByGroupName = await _repository.ExistsByGroupNameAsync(request.GroupName, ct);
        if (existsByGroupName)
        {
            _logger.LogWarning("Technection category with group name {GroupName} already exists", request.GroupName);
            return Result<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>.Failure(TechnectionCategoryErrors.CategoryGroupNameAlreadyExists);
        }

        var category = new Domain.Models.TechnectionCategory.TechnectionCategory(new TechnectionCategoryCreationParams
        {
            GroupName = request.GroupName,
            DifficultyLevel = request.DifficultyLevel,
            Words = request.Words,
            SuccessMessage = request.SuccessMessage
        });

        await _repository.AddAsync(category, ct);
        _logger.LogInformation("Created technection category {GroupName} with Id {Id}", category.GroupName, category.Id);

        return TechnectionCategorySuccesses.CategoryCreated(MapToCreateDto(category));
    }

    public async Task<Result<SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>>> UpdateByGroupNameAsync(
        string groupName, UpdateTechnectionCategoryByGroupNameRequestDto request, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found for update", groupName);
            return Result<SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>>.Failure(TechnectionCategoryErrors.CategoryNotFound);
        }

        // Check group name uniqueness only if it's being changed
        if (request.GroupName is not null && !string.Equals(category.GroupName, request.GroupName, StringComparison.OrdinalIgnoreCase))
        {
            var existsByGroupName = await _repository.ExistsByGroupNameAsync(request.GroupName, ct);
            if (existsByGroupName)
            {
                _logger.LogWarning("Cannot update: technection category with group name {GroupName} already exists", request.GroupName);
                return Result<SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>>.Failure(TechnectionCategoryErrors.CategoryGroupNameAlreadyExists);
            }
        }

        category.Update(request.GroupName, request.DifficultyLevel, request.SuccessMessage);
        await _repository.UpdateAsync(category, ct);
        _logger.LogInformation("Updated technection category {GroupName} (Id: {Id})", category.GroupName, category.Id);

        return TechnectionCategorySuccesses.CategoryUpdated(MapToUpdateDto(category));
    }

    public async Task<Result<SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>>> AddWordByGroupNameAsync(
        string groupName, AddTechnectionCategoryWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found for AddWord", groupName);
            return Result<SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>>.Failure(TechnectionCategoryErrors.CategoryNotFound);
        }

        if (category.Words.Any(w => w.Equals(request.Word, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Word {Word} already exists in technection category {GroupName}", request.Word, groupName);
            return Result<SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>>.Failure(TechnectionCategoryErrors.WordAlreadyExists);
        }

        category.AddWord(request.Word);
        await _repository.UpdateAsync(category, ct);
        _logger.LogInformation("Added word {Word} to technection category {GroupName} (Id: {Id})", request.Word, category.GroupName, category.Id);

        return TechnectionCategorySuccesses.WordAdded(MapToAddWordDto(category));
    }

    public async Task<Result<SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>>> RemoveWordByGroupNameAsync(
        string groupName, RemoveTechnectionCategoryWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found for RemoveWord", groupName);
            return Result<SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>>.Failure(TechnectionCategoryErrors.CategoryNotFound);
        }

        if (!category.Words.Any(w => w.Equals(request.Word, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Word {Word} not found in technection category {GroupName}", request.Word, groupName);
            return Result<SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>>.Failure(TechnectionCategoryErrors.WordNotFound);
        }

        category.RemoveWord(request.Word);
        await _repository.UpdateAsync(category, ct);
        _logger.LogInformation("Removed word {Word} from technection category {GroupName} (Id: {Id})", request.Word, category.GroupName, category.Id);

        return TechnectionCategorySuccesses.WordRemoved(MapToRemoveWordDto(category));
    }

    public async Task<Result<SuccessApiResponse>> DeleteByGroupNameAsync(string groupName, CancellationToken ct)
    {
        var deleted = await _repository.DeleteByGroupNameAsync(groupName, ct);
        if (!deleted)
        {
            _logger.LogWarning("Technection category {GroupName} not found for deletion", groupName);
            return Result<SuccessApiResponse>.Failure(TechnectionCategoryErrors.CategoryNotFound);
        }

        _logger.LogInformation("Deleted technection category {GroupName}", groupName);
        return TechnectionCategorySuccesses.CategoryDeleted();
    }

    private static GetTechnectionCategoryByGroupNameResponseDto MapToGetByGroupNameDto(Domain.Models.TechnectionCategory.TechnectionCategory category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static CreateTechnectionCategoryResponseDto MapToCreateDto(Domain.Models.TechnectionCategory.TechnectionCategory category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static UpdateTechnectionCategoryByGroupNameResponseDto MapToUpdateDto(Domain.Models.TechnectionCategory.TechnectionCategory category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static AddTechnectionCategoryWordByGroupNameResponseDto MapToAddWordDto(Domain.Models.TechnectionCategory.TechnectionCategory category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static RemoveTechnectionCategoryWordByGroupNameResponseDto MapToRemoveWordDto(Domain.Models.TechnectionCategory.TechnectionCategory category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };
}
