using Application.DTOs.Auth;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;
public interface IInternalUserVerificationService
{
    Task<Result<SuccessApiResponse<ConfirmEmailResponseDto>>> ConfirmEmailAsync(ConfirmEmailRequestDto confirmEmailRequest, Guid deviceId, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse>> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto resendConfirmationEmailRequest, CancellationToken cancellationToken);
}
