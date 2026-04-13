using Application.DTOs.Clusterdle;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces.Clusterdle;

public interface IClusterdleService
{
    Task<Result<SuccessApiResponse<GetPagedClusterdleResponseDto>>> GetPagedAsync(GetPagedClusterdleRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<GetClusterdleByGroupNameResponseDto>>> GetByGroupNameAsync(string groupName, CancellationToken ct);
    Task<Result<SuccessApiResponse<CreateClusterdleResponseDto>>> CreateAsync(CreateClusterdleRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>>> UpdateByGroupNameAsync(string groupName, UpdateClusterdleByGroupNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>>> AddWordByGroupNameAsync(string groupName, AddClusterdleWordByGroupNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>>> RemoveWordByGroupNameAsync(string groupName, RemoveClusterdleWordByGroupNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse>> DeleteByGroupNameAsync(string groupName, CancellationToken ct);
}
