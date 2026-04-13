using Application.DTOs.LogodleTarget;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces.LogodleTarget;

public interface ILogodleTargetService
{
    Task<Result<SuccessApiResponse<GetPagedLogodleTargetsResponseDto>>> GetPagedAsync(GetPagedLogodleTargetsRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<GetLogodleTargetByNameResponseDto>>> GetByNameAsync(string name, CancellationToken ct);
    Task<Result<SuccessApiResponse<CreateLogodleTargetResponseDto>>> CreateAsync(CreateLogodleTargetRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse>> DeleteByNameAsync(string name, CancellationToken ct);
}
