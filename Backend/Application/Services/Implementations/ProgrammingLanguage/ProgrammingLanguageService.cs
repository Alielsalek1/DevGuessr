using Application.Constants.Successes;
using Application.DTOs.ProgrammingLanguage;
using Application.Repositories.Interfaces;
using DevGuessr.Application.Services.Interfaces.ProgrammingLanguage;
using Application.Utils;
using Domain.Enums;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using DevGuessr.Application.Constants.Errors;
using Domain.Models.ProgrammingLanguage;

namespace Application.Services.Implementations.ProgrammingLanguage;

public class ProgrammingLanguageService(
    IProgrammingLanguageRepository repository,
    ILogger<ProgrammingLanguageService> logger) : IProgrammingLanguageService
{
    private readonly IProgrammingLanguageRepository _repository = repository;
    private readonly ILogger<ProgrammingLanguageService> _logger = logger;

    public async Task<Result<SuccessApiResponse<GetPagedProgrammingLanguagesResponseDto>>> GetPagedAsync(GetPagedProgrammingLanguagesRequestDto request, CancellationToken ct)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.PageNumber, request.PageSize, ct);
        _logger.LogInformation("Fetched {Count} programming languages (Page {PageNumber})", items.Count, request.PageNumber);

        var dtos = items.Select(MapToGetByNameDto).ToList();
        return ProgrammingLanguageSuccesses.LanguagesFetchedPaged(new GetPagedProgrammingLanguagesResponseDto
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    public async Task<Result<SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>>> AddTagByNameAsync(string name, AddProgrammingLanguageTagByNameRequestDto request, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found for AddTag", name);
            return Result<SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>>.Failure(ProgrammingLanguageErrors.LanguageNotFound);
        }

        if (language.Tags.Any(t => t.Equals(request.Tag, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Tag {Tag} already exists in programming language {Name}", request.Tag, name);
            return Result<SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>>.Failure(ProgrammingLanguageErrors.TagAlreadyExists);
        }

        language.AddTag(request.Tag);
        await _repository.UpdateAsync(language, ct);
        _logger.LogInformation("Added tag {Tag} to programming language {Name} (Id: {Id})", request.Tag, language.Name, language.Id);

        return ProgrammingLanguageSuccesses.TagAdded(MapToAddTagDto(language));
    }

    public async Task<Result<SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>>> RemoveTagByNameAsync(string name, RemoveProgrammingLanguageTagByNameRequestDto request, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found for RemoveTag", name);
            return Result<SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>>.Failure(ProgrammingLanguageErrors.LanguageNotFound);
        }

        if (!language.Tags.Any(t => t.Equals(request.Tag, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Tag {Tag} not found in programming language {Name}", request.Tag, name);
            return Result<SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>>.Failure(ProgrammingLanguageErrors.TagNotFound);
        }

        language.RemoveTag(request.Tag);
        await _repository.UpdateAsync(language, ct);
        _logger.LogInformation("Removed tag {Tag} from programming language {Name} (Id: {Id})", request.Tag, language.Name, language.Id);

        return ProgrammingLanguageSuccesses.TagRemoved(MapToRemoveTagDto(language));
    }

    public async Task<Result<SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>>> GetByNameAsync(string name, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found", name);
            return Result<SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>>.Failure(ProgrammingLanguageErrors.LanguageNotFound);
        }

        _logger.LogInformation("Fetched programming language {Name}", language.Name);
        return ProgrammingLanguageSuccesses.LanguageFetched(MapToGetByNameDto(language));
    }

    public async Task<Result<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>> CreateAsync(
        CreateProgrammingLanguageRequestDto request, CancellationToken ct)
    {
        var existsByName = await _repository.ExistsByNameAsync(request.Name, ct);
        if (existsByName)
        {
            _logger.LogWarning("Programming language with name {Name} already exists", request.Name);
            return Result<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>.Failure(ProgrammingLanguageErrors.LanguageNameAlreadyExists);
        }

        var language = new Domain.Models.ProgrammingLanguage.ProgrammingLanguage(new ProgrammingLanguageCreationParams
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

        return ProgrammingLanguageSuccesses.LanguageCreated(MapToCreateDto(language));
    }

    public async Task<Result<SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>>> UpdateByNameAsync(
        string name, UpdateProgrammingLanguageByNameRequestDto request, CancellationToken ct)
    {
        var language = await _repository.GetByNameAsync(name, ct);
        if (language is null)
        {
            _logger.LogWarning("Programming language {Name} not found for update", name);
            return Result<SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>>.Failure(ProgrammingLanguageErrors.LanguageNotFound);
        }

        // Check name uniqueness if name is being changed
        if (request.Name is not null && !string.Equals(language.Name, request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var existsByName = await _repository.ExistsByNameAsync(request.Name, ct);
            if (existsByName)
            {
                _logger.LogWarning("Cannot update: programming language with name {Name} already exists", request.Name);
                return Result<SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>>.Failure(ProgrammingLanguageErrors.LanguageNameAlreadyExists);
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

        return ProgrammingLanguageSuccesses.LanguageUpdated(MapToUpdateDto(language));
    }

    public async Task<Result<SuccessApiResponse>> DeleteByNameAsync(string name, CancellationToken ct)
    {
        var deleted = await _repository.DeleteByNameAsync(name, ct);
        if (!deleted)
        {
            _logger.LogWarning("Programming language {Name} not found for deletion", name);
            return Result<SuccessApiResponse>.Failure(ProgrammingLanguageErrors.LanguageNotFound);
        }

        _logger.LogInformation("Deleted programming language {Name}", name);
        return ProgrammingLanguageSuccesses.LanguageDeleted();
    }

    private static GetProgrammingLanguageByNameResponseDto MapToGetByNameDto(Domain.Models.ProgrammingLanguage.ProgrammingLanguage language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static CreateProgrammingLanguageResponseDto MapToCreateDto(Domain.Models.ProgrammingLanguage.ProgrammingLanguage language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static UpdateProgrammingLanguageByNameResponseDto MapToUpdateDto(Domain.Models.ProgrammingLanguage.ProgrammingLanguage language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static AddProgrammingLanguageTagByNameResponseDto MapToAddTagDto(Domain.Models.ProgrammingLanguage.ProgrammingLanguage language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };

    private static RemoveProgrammingLanguageTagByNameResponseDto MapToRemoveTagDto(Domain.Models.ProgrammingLanguage.ProgrammingLanguage language) =>
        new() { Id = language.Id, Name = language.Name, YearFirstAppeared = language.YearFirstAppeared, TypingDiscipline = language.TypingDiscipline.ToString(), TypeStrength = language.TypeStrength.ToString(), ExecutionModel = language.ExecutionModel.ToString(), MemoryManagement = language.MemoryManagement.ToString(), Tags = language.Tags };
}
