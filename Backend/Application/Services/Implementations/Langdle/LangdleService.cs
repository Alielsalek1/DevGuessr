using Application.Constants.Successes;
using Application.DTOs.Langdle;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces.Langdle;
using Application.Utils;
using Domain.Enums;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using Application.Constants.Errors;
using Domain.Models.Langdle;

namespace Application.Services.Implementations.Langdle;

public class LangdleService(
    ILangdleRepository repository,
    ILogger<LangdleService> logger) : ILangdleService
{
    private readonly ILangdleRepository _repository = repository;
    private readonly ILogger<LangdleService> _logger = logger;

    public async Task<Result<SuccessApiResponse<GetPagedLangdleResponseDto>>> GetPagedAsync(GetPagedLangdleRequestDto request, CancellationToken ct)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.PageNumber, request.PageSize, ct);
        _logger.LogInformation("Fetched {Count} programming languages (Page {PageNumber})", items.Count, request.PageNumber);

        var dtos = items.Select(MapToGetByNameDto).ToList();
        return LangdleSuccesses.LanguagesFetchedPaged(new GetPagedLangdleResponseDto
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    public async Task<Result<SuccessApiResponse<AddLangdleTagByNameResponseDto>>> AddTagByNameAsync(string name, AddLangdleTagByNameRequestDto request, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found for AddTag", name);
            return Result<SuccessApiResponse<AddLangdleTagByNameResponseDto>>.Failure(LangdleErrors.LanguageNotFound);
        }

        if (language.Tags.Any(t => t.Equals(request.Tag, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Tag {Tag} already exists in programming language {Name}", request.Tag, name);
            return Result<SuccessApiResponse<AddLangdleTagByNameResponseDto>>.Failure(LangdleErrors.TagAlreadyExists);
        }

        language.AddTag(request.Tag);
        await _repository.UpdateAsync(language, ct);
        _logger.LogInformation("Added tag {Tag} to programming language {Name} (Id: {Id})", request.Tag, language.Name, language.Id);

        return LangdleSuccesses.TagAdded(MapToAddTagDto(language));
    }

    public async Task<Result<SuccessApiResponse<RemoveLangdleTagByNameResponseDto>>> RemoveTagByNameAsync(string name, RemoveLangdleTagByNameRequestDto request, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found for RemoveTag", name);
            return Result<SuccessApiResponse<RemoveLangdleTagByNameResponseDto>>.Failure(LangdleErrors.LanguageNotFound);
        }

        if (!language.Tags.Any(t => t.Equals(request.Tag, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Tag {Tag} not found in programming language {Name}", request.Tag, name);
            return Result<SuccessApiResponse<RemoveLangdleTagByNameResponseDto>>.Failure(LangdleErrors.TagNotFound);
        }

        language.RemoveTag(request.Tag);
        await _repository.UpdateAsync(language, ct);
        _logger.LogInformation("Removed tag {Tag} from programming language {Name} (Id: {Id})", request.Tag, language.Name, language.Id);

        return LangdleSuccesses.TagRemoved(MapToRemoveTagDto(language));
    }

    public async Task<Result<SuccessApiResponse<GetLangdleByNameResponseDto>>> GetByNameAsync(string name, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found", name);
            return Result<SuccessApiResponse<GetLangdleByNameResponseDto>>.Failure(LangdleErrors.LanguageNotFound);
        }

        _logger.LogInformation("Fetched programming language {Name}", language.Name);
        return LangdleSuccesses.LanguageFetched(MapToGetByNameDto(language));
    }

    public async Task<Result<SuccessApiResponse<CreateLangdleResponseDto>>> CreateAsync(
        CreateLangdleRequestDto request, CancellationToken ct)
    {
        var existsByName = await _repository.ExistsByNameAsync(request.Name, ct);
        if (existsByName)
        {
            _logger.LogWarning("Programming language with name {Name} already exists", request.Name);
            return Result<SuccessApiResponse<CreateLangdleResponseDto>>.Failure(LangdleErrors.LanguageNameAlreadyExists);
        }

        var language = new Domain.Models.Langdle.Langdle(new LangdleCreationParams
        {
            Name = request.Name,
            YearFirstAppeared = request.YearFirstAppeared,
            TypingDiscipline = Enum.Parse<TypingDiscipline>(request.TypingDiscipline, true),
            TypeStrength = Enum.Parse<TypeStrength>(request.TypeStrength, true),
            ExecutionModel = Enum.Parse<ExecutionModel>(request.ExecutionModel, true),
            MemoryManagement = Enum.Parse<MemoryManagement>(request.MemoryManagement, true),
            Tags = request.Tags
        });

        await _repository.AddAsync(language, ct);
        _logger.LogInformation("Created programming language {Name} with Id {Id}", language.Name, language.Id);

        return LangdleSuccesses.LanguageCreated(MapToCreateDto(language));
    }

    public async Task<Result<SuccessApiResponse<UpdateLangdleByNameResponseDto>>> UpdateByNameAsync(
        string name, UpdateLangdleByNameRequestDto request, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found for update", name);
            return Result<SuccessApiResponse<UpdateLangdleByNameResponseDto>>.Failure(LangdleErrors.LanguageNotFound);
        }

        // Check name uniqueness if name is being changed
        if (request.Name is not null && !string.Equals(language.Name, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var existsByName = await _repository.ExistsByNameAsync(request.Name, ct);
            if (existsByName)
            {
                _logger.LogWarning("Cannot update: programming language with name {Name} already exists", request.Name);
                return Result<SuccessApiResponse<UpdateLangdleByNameResponseDto>>.Failure(LangdleErrors.LanguageNameAlreadyExists);
            }
        }

        language.Update(
            request.Name,
            request.YearFirstAppeared,
            request.TypingDiscipline != null ? Enum.Parse<TypingDiscipline>(request.TypingDiscipline, true) : null,
            request.TypeStrength != null ? Enum.Parse<TypeStrength>(request.TypeStrength, true) : null,
            request.ExecutionModel != null ? Enum.Parse<ExecutionModel>(request.ExecutionModel, true) : null,
            request.MemoryManagement != null ? Enum.Parse<MemoryManagement>(request.MemoryManagement, true) : null
        );

        await _repository.UpdateAsync(language, ct);
        _logger.LogInformation("Updated programming language {Name} (Id: {Id})", language.Name, language.Id);

        return LangdleSuccesses.LanguageUpdated(MapToUpdateDto(language));
    }

    public async Task<Result<SuccessApiResponse>> DeleteByNameAsync(string name, CancellationToken ct)
    {
        var deleted = await _repository.DeleteByNameAsync(name, ct);
        if (!deleted)
        {
            _logger.LogWarning("Programming language {Name} not found for deletion", name);
            return Result<SuccessApiResponse>.Failure(LangdleErrors.LanguageNotFound);
        }

        _logger.LogInformation("Deleted programming language {Name}", name);
        return LangdleSuccesses.LanguageDeleted();
    }

    private static GetLangdleByNameResponseDto MapToGetByNameDto(Domain.Models.Langdle.Langdle language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static CreateLangdleResponseDto MapToCreateDto(Domain.Models.Langdle.Langdle language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static UpdateLangdleByNameResponseDto MapToUpdateDto(Domain.Models.Langdle.Langdle language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static AddLangdleTagByNameResponseDto MapToAddTagDto(Domain.Models.Langdle.Langdle language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static RemoveLangdleTagByNameResponseDto MapToRemoveTagDto(Domain.Models.Langdle.Langdle language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };
}
