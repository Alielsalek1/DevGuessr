using Application.DTOs.Auth;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IInternalPasswordResetService
{
    Task<Result<SuccessApiResponse>> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest, CancellationToken cancellationToken);
}
