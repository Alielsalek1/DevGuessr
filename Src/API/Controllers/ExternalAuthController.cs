using API.Extensions;
using Application.DTOs.ExternalAuth;
using Application.Services.Interfaces;
using Application.Utils;
using Asp.Versioning;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// External Authentication Controller
/// 
/// Manages OAuth2-based authentication flows with external providers.
/// Currently supports Google OAuth2 for seamless social login integration.
/// 
/// **Key Features:**
/// - Single Sign-On (SSO) via Google OAuth2
/// - Automatic user account creation on first login
/// - JWT token generation for authenticated sessions
/// - Secure refresh token management via HttpOnly cookies
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/external-auth")]
public class ExternalAuthController(IExternalAuthService authService) : ControllerBase
{
    private readonly IExternalAuthService _authService = authService;

    /// <summary>
    /// Authenticate with Google OAuth2
    /// </summary>
    [HttpPost("login/google")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<GoogleAuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequestDto authRequest, CancellationToken ct)
    {
        var result = await _authService.GoogleLoginAsync(authRequest, ct);
        if (result.IsSuccess)
        {
            var newRefreshToken = result.Data.Data.RefreshToken;
            this.AddRefreshTokenCookie(newRefreshToken);
        }
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Link a Google account to an existing authenticated user
    /// </summary>
    [HttpPost("link/google")]
    [ProducesResponseType(typeof(SuccessApiResponse<GoogleAuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LinkGoogleAccount([FromBody] GoogleAuthRequestDto authRequest, CancellationToken ct)
    {
        var userIdResult = this.GetAuthenticatedUserId();
        if (!userIdResult.IsSuccess)
        {
            return this.ToActionResult(Result<SuccessApiResponse<GoogleAuthResponseDto>>.Failure(userIdResult.Error));
        }
        var result = await _authService.LinkGoogleAccountAsync(authRequest, userIdResult.Data, ct);
        if (result.IsSuccess)
        {
            var newRefreshToken = result.Data.Data.RefreshToken;
            this.AddRefreshTokenCookie(newRefreshToken);
        }
        return this.ToActionResult(result);
    }
}