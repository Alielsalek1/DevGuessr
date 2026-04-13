using Application.Constants.Successes;
using Application.DTOs.Clusterdle;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces.Clusterdle;
using Application.Utils;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using Application.Constants.Errors;
using Domain.Models.Clusterdle;

namespace Application.Services.Implementations.Clusterdle;

public class ClusterdleService(
    IClusterdleRepository repository,
    ILogger<ClusterdleService> logger) : IClusterdleService
{
    private readonly IClusterdleRepository _repository = repository;
    private readonly ILogger<ClusterdleService> _logger = logger;

    public async Task<Result<SuccessApiResponse<GetPagedClusterdleResponseDto>>> GetPagedAsync(
        GetPagedClusterdleRequestDto request, CancellationToken ct)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.PageNumber, request.PageSize, ct);
        _logger.LogInformation("Fetched {Count} technection categories (Page {PageNumber})", items.Count, request.PageNumber);

        var dtos = items.Select(MapToGetByGroupNameDto).ToList();
        return ClusterdleSuccesses.CategoriesFetchedPaged(new GetPagedClusterdleResponseDto
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    public async Task<Result<SuccessApiResponse<GetClusterdleByGroupNameResponseDto>>> GetByGroupNameAsync(
        string groupName, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found", groupName);
            return Result<SuccessApiResponse<GetClusterdleByGroupNameResponseDto>>.Failure(ClusterdleErrors.CategoryNotFound);
        }

        _logger.LogInformation("Fetched technection category {GroupName}", category.GroupName);
        return ClusterdleSuccesses.CategoryFetched(MapToGetByGroupNameDto(category));
    }

    public async Task<Result<SuccessApiResponse<CreateClusterdleResponseDto>>> CreateAsync(
        CreateClusterdleRequestDto request, CancellationToken ct)
    {
        var existsByGroupName = await _repository.ExistsByGroupNameAsync(request.GroupName, ct);
        if (existsByGroupName)
        {
            _logger.LogWarning("Technection category with group name {GroupName} already exists", request.GroupName);
            return Result<SuccessApiResponse<CreateClusterdleResponseDto>>.Failure(ClusterdleErrors.CategoryGroupNameAlreadyExists);
        }

        var category = new Domain.Models.Clusterdle.Clusterdle(new ClusterdleCreationParams
        {
            GroupName = request.GroupName,
            DifficultyLevel = request.DifficultyLevel,
            Words = request.Words,
            SuccessMessage = request.SuccessMessage
        });

        await _repository.AddAsync(category, ct);
        _logger.LogInformation("Created technection category {GroupName} with Id {Id}", category.GroupName, category.Id);

        return ClusterdleSuccesses.CategoryCreated(MapToCreateDto(category));
    }

    public async Task<Result<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>>> UpdateByGroupNameAsync(
        string groupName, UpdateClusterdleByGroupNameRequestDto request, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found for update", groupName);
            return Result<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>>.Failure(ClusterdleErrors.CategoryNotFound);
        }

        // Check group name uniqueness only if it's being changed
        if (request.GroupName is not null && !string.Equals(category.GroupName, request.GroupName, StringComparison.OrdinalIgnoreCase))
        {
            var existsByGroupName = await _repository.ExistsByGroupNameAsync(request.GroupName, ct);
            if (existsByGroupName)
            {
                _logger.LogWarning("Cannot update: technection category with group name {GroupName} already exists", request.GroupName);
                return Result<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>>.Failure(ClusterdleErrors.CategoryGroupNameAlreadyExists);
            }
        }

        category.Update(request.GroupName, request.DifficultyLevel, request.SuccessMessage);
        await _repository.UpdateAsync(category, ct);
        _logger.LogInformation("Updated technection category {GroupName} (Id: {Id})", category.GroupName, category.Id);

        return ClusterdleSuccesses.CategoryUpdated(MapToUpdateDto(category));
    }

    public async Task<Result<SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>>> AddWordByGroupNameAsync(
        string groupName, AddClusterdleWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found for AddWord", groupName);
            return Result<SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>>.Failure(ClusterdleErrors.CategoryNotFound);
        }

        if (category.Words.Any(w => w.Equals(request.Word, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Word {Word} already exists in technection category {GroupName}", request.Word, groupName);
            return Result<SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>>.Failure(ClusterdleErrors.WordAlreadyExists);
        }

        category.AddWord(request.Word);
        await _repository.UpdateAsync(category, ct);
        _logger.LogInformation("Added word {Word} to technection category {GroupName} (Id: {Id})", request.Word, category.GroupName, category.Id);

        return ClusterdleSuccesses.WordAdded(MapToAddWordDto(category));
    }

    public async Task<Result<SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>>> RemoveWordByGroupNameAsync(
        string groupName, RemoveClusterdleWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var category = await _repository.GetByGroupNameAsync(groupName, ct);
        if (category is null)
        {
            _logger.LogWarning("Technection category {GroupName} not found for RemoveWord", groupName);
            return Result<SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>>.Failure(ClusterdleErrors.CategoryNotFound);
        }

        if (!category.Words.Any(w => w.Equals(request.Word, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Word {Word} not found in technection category {GroupName}", request.Word, groupName);
            return Result<SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>>.Failure(ClusterdleErrors.WordNotFound);
        }

        category.RemoveWord(request.Word);
        await _repository.UpdateAsync(category, ct);
        _logger.LogInformation("Removed word {Word} from technection category {GroupName} (Id: {Id})", request.Word, category.GroupName, category.Id);

        return ClusterdleSuccesses.WordRemoved(MapToRemoveWordDto(category));
    }

    public async Task<Result<SuccessApiResponse>> DeleteByGroupNameAsync(string groupName, CancellationToken ct)
    {
        var deleted = await _repository.DeleteByGroupNameAsync(groupName, ct);
        if (!deleted)
        {
            _logger.LogWarning("Technection category {GroupName} not found for deletion", groupName);
            return Result<SuccessApiResponse>.Failure(ClusterdleErrors.CategoryNotFound);
        }

        _logger.LogInformation("Deleted technection category {GroupName}", groupName);
        return ClusterdleSuccesses.CategoryDeleted();
    }

    private static GetClusterdleByGroupNameResponseDto MapToGetByGroupNameDto(Domain.Models.Clusterdle.Clusterdle category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static CreateClusterdleResponseDto MapToCreateDto(Domain.Models.Clusterdle.Clusterdle category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static UpdateClusterdleByGroupNameResponseDto MapToUpdateDto(Domain.Models.Clusterdle.Clusterdle category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static AddClusterdleWordByGroupNameResponseDto MapToAddWordDto(Domain.Models.Clusterdle.Clusterdle category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };

    private static RemoveClusterdleWordByGroupNameResponseDto MapToRemoveWordDto(Domain.Models.Clusterdle.Clusterdle category) =>
        new() { Id = category.Id, GroupName = category.GroupName, DifficultyLevel = category.DifficultyLevel, Words = category.Words, SuccessMessage = category.SuccessMessage };
}
