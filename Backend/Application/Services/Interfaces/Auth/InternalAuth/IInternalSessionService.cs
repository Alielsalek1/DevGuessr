using System.Net;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IInternalSessionService
{
    Task<Result<SuccessApiResponse<LoginResponseDto>>> LoginAsync(LoginRequestDto loginRequest, IPAddress ipAddress, Guid deviceId, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<GuestLoginResponseDto>>> GuestLoginAsync(CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<LoginResponseDto>>> ConfirmLoginForNewDeviceAsync(ConfirmLoginRequestDto confirmLoginRequest, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<RefreshTokenResponseDto>>> RefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);
}