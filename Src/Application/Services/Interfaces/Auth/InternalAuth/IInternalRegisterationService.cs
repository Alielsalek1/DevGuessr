using Application.DTOs.Auth;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IInternalRegisterationService
{
    Task<Result<SuccessApiResponse<RegisterResponseDto>>> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<RegisterResponseDto>>> GuestPromoteAsync(RegisterRequestDto registerRequest, Guid userId, CancellationToken cancellationToken);
}
