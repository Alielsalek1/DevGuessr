using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Auth;
using Application.Services.Interfaces;
using Application.DTOs.User;
using Application.Utils;
using Asp.Versioning;
using API.Extensions;
using API.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Domain.Shared;

namespace API.Controllers;

/// <summary>
/// Internal Authentication Controller
/// Manages user registration, login, email confirmation, password management, and token refresh.
/// This controller handles all authentication flows for users registering with email and password.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/internal-auth")]
public class InternalAuthController(IInternalAuthFacadeService authFacade) : ControllerBase
{
    private readonly IInternalAuthFacadeService _authFacade = authFacade;

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [Idempotent]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<RegisterResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto registerRequest,
        CancellationToken cancellationToken)
    {
        var result = await _authFacade.RegisterAsync(registerRequest, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Promote a guest user to a registered user
    /// </summary>
    [HttpPost("promote/guest")]
    [Idempotent]
    [ProducesResponseType(typeof(SuccessApiResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GuestPromote(
        [FromBody] RegisterRequestDto registerRequest,
        CancellationToken cancellationToken)
    {
        var userIdResult = this.GetAuthenticatedUserId();
        if (!userIdResult.IsSuccess)
        {
            return this.ToActionResult(Result<SuccessApiResponse<RegisterResponseDto>>.Failure(userIdResult.Error));
        }

        var result = await _authFacade.GuestPromoteAsync(registerRequest, userIdResult.Data, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Login as a guest user
    /// </summary>
    [HttpPost("login/guest")]
    [Idempotent]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<GuestLoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GuestLogin(CancellationToken cancellationToken)
    {
        var result = await _authFacade.GuestLoginAsync(cancellationToken);
        if (result.IsSuccess)
        {
            var refreshToken = result.Data.Data.RefreshToken;
            this.AddRefreshTokenCookie(refreshToken);
        }
        return this.ToActionResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto loginRequest,
        CancellationToken cancellationToken)
    {
        var ipAddressResult = this.GetClientIpAddress();
        if (!ipAddressResult.IsSuccess)
        {
            return this.ToActionResult(Result<SuccessApiResponse<LoginResponseDto>>.Failure(ipAddressResult.Error));
        }

        var deviceIdResult = this.GetDeviceIdCookie();
        var deviceId = deviceIdResult.IsSuccess ? deviceIdResult.Data : Guid.NewGuid();

        var result = await _authFacade.LoginAsync(loginRequest, ipAddressResult.Data, deviceId, cancellationToken);
        if (result.IsSuccess && result.Data.Data != null)
        {
            var refreshToken = result.Data.Data.RefreshToken;
            this.AddRefreshTokenCookie(refreshToken);
            this.AddDeviceIdCookie(deviceId);
        }
        return this.ToActionResult(result);
    }

    [HttpPost("confirm-login")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmLogin(
        [FromBody] ConfirmLoginRequestDto confirmLoginRequest,
        CancellationToken cancellationToken)
    {
        var result = await _authFacade.ConfirmLoginAsync(confirmLoginRequest, cancellationToken);
        if (result.IsSuccess && result.Data.Data != null)
        {
            var refreshToken = result.Data.Data.RefreshToken;
            this.AddRefreshTokenCookie(refreshToken);
            this.AddDeviceIdCookie(result.Data.Data.DeviceId);
        }
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Confirm user email address
    /// </summary>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<ConfirmEmailResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequestDto confirmEmailRequest,
        CancellationToken cancellationToken)
    {
        var deviceIdResult = this.GetDeviceIdCookie();
        var deviceId = deviceIdResult.IsSuccess ? deviceIdResult.Data : Guid.NewGuid();

        var result = await _authFacade.ConfirmEmailAsync(confirmEmailRequest, deviceId, cancellationToken);
        if (result.IsSuccess && result.Data.Data != null)
        {
            this.AddDeviceIdCookie(result.Data.Data.DeviceId);
        }
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Resend confirmation email
    /// </summary>
    [HttpPost("resend-confirmation-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmationEmail(
        [FromBody] ResendConfirmationEmailRequestDto resendConfirmationEmailRequest,
        CancellationToken cancellationToken)
    {
        var result = await _authFacade.ResendConfirmationEmailAsync(resendConfirmationEmailRequest, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forget-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto forgetPasswordRequest, CancellationToken cancellationToken)
    {
        var result = await _authFacade.ForgetPasswordAsync(forgetPasswordRequest, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequest, CancellationToken cancellationToken)
    {
        var result = await _authFacade.ResetPasswordAsync(resetPasswordRequest, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<RefreshTokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest, CancellationToken cancellationToken)
    {
        var refreshTokenResult = this.GetRefreshTokenCookie();
        if (!refreshTokenResult.IsSuccess)
        {
            return this.ToActionResult(Result<SuccessApiResponse<RefreshTokenResponseDto>>.Failure(refreshTokenResult.Error));
        }
        var result = await _authFacade.RefreshTokenAsync(refreshTokenRequest, refreshTokenResult.Data, cancellationToken);
        if (result.IsSuccess)
        {
            var newRefreshToken = result.Data.Data.RefreshToken;
            this.AddRefreshTokenCookie(newRefreshToken);
        }
        return this.ToActionResult(result);
    }
}