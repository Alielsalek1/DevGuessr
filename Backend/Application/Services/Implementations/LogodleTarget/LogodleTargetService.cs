using Application.Constants.Successes;
using Application.DTOs.LogodleTarget;
using Application.Repositories.Interfaces;
using DevGuessr.Application.Services.Interfaces.LogodleTarget;
using Application.Utils;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using DevGuessr.Application.Constants.Errors;
using Domain.Models.LogodleTarget;
using Application.Services.Interfaces.Misc;

namespace Application.Services.Implementations.LogodleTarget;

public class LogodleTargetService(
    ILogodleTargetRepository repository,
    IFileStorageService fileStorageService,
    ILogger<LogodleTargetService> logger) : ILogodleTargetService
{
    private readonly ILogodleTargetRepository _repository = repository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly ILogger<LogodleTargetService> _logger = logger;

    public async Task<Result<SuccessApiResponse<GetPagedLogodleTargetsResponseDto>>> GetPagedAsync(GetPagedLogodleTargetsRequestDto request, CancellationToken ct)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.PageNumber, request.PageSize, ct);
        _logger.LogInformation("Fetched {Count} logodle targets (Page {PageNumber})", items.Count, request.PageNumber);

        var dtos = items.Select(MapToGetByNameDto).ToList();
        return LogodleTargetSuccesses.TargetsFetchedPaged(new GetPagedLogodleTargetsResponseDto
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    public async Task<Result<SuccessApiResponse<GetLogodleTargetByNameResponseDto>>> GetByNameAsync(string name, CancellationToken ct)
    {
        var target = await _repository.GetByNameAsync(name, ct);
        if (target is null)
        {
            _logger.LogWarning("Logodle target {Name} not found", name);
            return Result<SuccessApiResponse<GetLogodleTargetByNameResponseDto>>.Failure(LogodleTargetErrors.TargetNotFound);
        }

        _logger.LogInformation("Fetched logodle target {Name}", target.Name);
        return LogodleTargetSuccesses.TargetFetched(MapToGetByNameDto(target));
    }

    public async Task<Result<SuccessApiResponse<CreateLogodleTargetResponseDto>>> CreateAsync(
        CreateLogodleTargetRequestDto request, CancellationToken ct)
    {
        var existsByName = await _repository.ExistsByNameAsync(request.Name, ct);
        if (existsByName)
        {
            _logger.LogWarning("Logodle target with name {Name} already exists", request.Name);
            return Result<SuccessApiResponse<CreateLogodleTargetResponseDto>>.Failure(LogodleTargetErrors.TargetNameAlreadyExists);
        }

        var imagePath = await _fileStorageService.SaveOriginalImageAsync(request.Image, request.Name, ct);
        var ext = Path.GetExtension(request.Image.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".png";
        var blurredImageUrls = await _fileStorageService.GenerateBlurredImagesAsync(request.Name, ext, ct);

        var target = new Domain.Models.LogodleTarget.LogodleTarget(new LogodleTargetCreationParams
        {
            Name = request.Name,
            ImagePath = imagePath,
            BlurredImageUrls = blurredImageUrls
        });

        await _repository.AddAsync(target, ct);
        _logger.LogInformation("Created logodle target {Name} with Id {Id}", target.Name, target.Id);

        return LogodleTargetSuccesses.TargetCreated(MapToCreateDto(target));
    }

    public async Task<Result<SuccessApiResponse>> DeleteByNameAsync(string name, CancellationToken ct)
    {
        var deleted = await _repository.DeleteByNameAsync(name, ct);
        if (!deleted)
        {
            _logger.LogWarning("Logodle target {Name} not found for deletion", name);
            return Result<SuccessApiResponse>.Failure(LogodleTargetErrors.TargetNotFound);
        }

        await _fileStorageService.DeleteImagesAsync(name, ct);

        _logger.LogInformation("Deleted logodle target {Name}", name);
        return LogodleTargetSuccesses.TargetDeleted();
    }

    private static GetLogodleTargetByNameResponseDto MapToGetByNameDto(Domain.Models.LogodleTarget.LogodleTarget target) =>
        new() { Id = target.Id, Name = target.Name, ImagePath = target.ImagePath, BlurredImageUrls = target.BlurredImageUrls };

    private static CreateLogodleTargetResponseDto MapToCreateDto(Domain.Models.LogodleTarget.LogodleTarget target) =>
        new() { Id = target.Id, Name = target.Name, ImagePath = target.ImagePath, BlurredImageUrls = target.BlurredImageUrls };

}
