using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Shared;
using System.Net;

namespace Application.Services.Implementations;

public class InternalAuthFacadeService(
    IInternalRegisterationService internalAuthService,
    IInternalUserVerificationService userConfirmationService,
    IInternalPasswordResetService passwordResetService,
    IInternalSessionService internalIdentityService) : IInternalAuthFacadeService
{
    private readonly IInternalRegisterationService _internalAuthService = internalAuthService;
    private readonly IInternalUserVerificationService _userConfirmationService = userConfirmationService;
    private readonly IInternalPasswordResetService _passwordResetService = passwordResetService;
    private readonly IInternalSessionService _internalIdentityService = internalIdentityService;

    public Task<Result<SuccessApiResponse<RegisterResponseDto>>> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken)
        => _internalAuthService.RegisterAsync(registerRequest, cancellationToken);

    public Task<Result<SuccessApiResponse<LoginResponseDto>>> LoginAsync(LoginRequestDto loginRequest, IPAddress ipAddress, Guid deviceId, CancellationToken cancellationToken)
        => _internalIdentityService.LoginAsync(loginRequest, ipAddress, deviceId, cancellationToken);

    public Task<Result<SuccessApiResponse<LoginResponseDto>>> ConfirmLoginAsync(ConfirmLoginRequestDto confirmLoginRequest, CancellationToken cancellationToken)
        => _internalIdentityService.ConfirmLoginForNewDeviceAsync(confirmLoginRequest, cancellationToken);

    public Task<Result<SuccessApiResponse<ConfirmEmailResponseDto>>> ConfirmEmailAsync(ConfirmEmailRequestDto confirmEmailRequest, Guid deviceId, CancellationToken cancellationToken)
        => _userConfirmationService.ConfirmEmailAsync(confirmEmailRequest, deviceId, cancellationToken);

    public Task<Result<SuccessApiResponse>> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto resendConfirmationEmailRequest, CancellationToken cancellationToken)
        => _userConfirmationService.ResendConfirmationEmailAsync(resendConfirmationEmailRequest, cancellationToken);

    public Task<Result<SuccessApiResponse>> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequest, CancellationToken cancellationToken)
        => _passwordResetService.ForgetPasswordAsync(forgetPasswordRequest, cancellationToken);

    public Task<Result<SuccessApiResponse>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest, CancellationToken cancellationToken)
        => _passwordResetService.ResetPasswordAsync(resetPasswordRequest, cancellationToken);

    public Task<Result<SuccessApiResponse<RefreshTokenResponseDto>>> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest, string refreshTokenCookie, CancellationToken cancellationToken)
        => _internalIdentityService.RefreshTokenAsync(refreshTokenRequest.UserId, refreshTokenCookie, cancellationToken);

    public Task<Result<SuccessApiResponse<RegisterResponseDto>>> GuestPromoteAsync(RegisterRequestDto registerRequest, Guid userId, CancellationToken cancellationToken)
        => _internalAuthService.GuestPromoteAsync(registerRequest, userId, cancellationToken);

    public Task<Result<SuccessApiResponse<GuestLoginResponseDto>>> GuestLoginAsync(CancellationToken cancellationToken)
        => _internalIdentityService.GuestLoginAsync(cancellationToken);
}