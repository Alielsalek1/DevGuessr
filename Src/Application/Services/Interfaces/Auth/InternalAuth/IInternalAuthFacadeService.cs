using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Utils;
using Domain.Shared;
using System.Net;

namespace Application.Services.Interfaces;

public interface IInternalAuthFacadeService
{
    Task<Result<SuccessApiResponse<RegisterResponseDto>>> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<RegisterResponseDto>>> GuestPromoteAsync(RegisterRequestDto registerRequest, Guid userId, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<LoginResponseDto>>> LoginAsync(LoginRequestDto loginRequest, IPAddress ipAddress, Guid deviceId, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<LoginResponseDto>>> ConfirmLoginAsync(ConfirmLoginRequestDto confirmLoginRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<ConfirmEmailResponseDto>>> ConfirmEmailAsync(ConfirmEmailRequestDto confirmEmailRequest, Guid deviceId, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse>> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto resendConfirmationEmailRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse>> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<RefreshTokenResponseDto>>> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest, string refreshTokenCookie, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<GuestLoginResponseDto>>> GuestLoginAsync(CancellationToken cancellationToken);
}