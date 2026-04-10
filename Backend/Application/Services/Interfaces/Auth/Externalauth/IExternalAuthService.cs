using Application.DTOs.ExternalAuth;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IExternalAuthService
{
    public Task<Result<SuccessApiResponse<GoogleAuthResponseDto>>> GoogleLoginAsync(GoogleAuthRequestDto authRequest, CancellationToken ct);
    public Task<Result<SuccessApiResponse<GoogleAuthResponseDto>>> LinkGoogleAccountAsync(GoogleAuthRequestDto authRequest, Guid userId, CancellationToken ct);
}