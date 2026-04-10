using Microsoft.AspNetCore.Mvc;
using Domain.Shared;
using Application.Utils;
using System.Security.Claims;
using System.Net;
using Application.Constants.ApiErrors;

namespace API.Extensions;

public static class ControllerBaseExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        int statusCode = 0;
        if (result.IsSuccess)
        {
            // The user says T will be the SuccessApiResponse
            // We retrieve the status code from T using dynamic to accommodate the generic type
            statusCode = (result.Data as dynamic)?.StatusCode ?? 200;
            if (statusCode < 100 || statusCode > 599) statusCode = 200;
            return controller.StatusCode(statusCode, result.Data);
        }

        var error = result.Error;
        statusCode = result.Error.statusCode;
        if (statusCode < 100 || statusCode > 599) statusCode = 500;
        var failResponse = new FailApiResponse
        {
            StatusCode = error.statusCode,
            Message = error.message,
            Errors = error.errors ?? [],
            ErrorCode = error.errorCode,
            TraceId = error.traceId
        };

        return controller.StatusCode(statusCode, failResponse);
    }

    public static void AddRefreshTokenCookie(this ControllerBase controller, string refreshToken)
    {
        controller.Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            }
        );
    }

    public static Result<string> GetRefreshTokenCookie(this ControllerBase controller)
    {
        var refreshToken = controller.Request.Cookies["refreshToken"] ?? string.Empty;
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Result<string>.Failure(new Error(
                "AUTH_002",
                "Refresh token cookie is missing.",
                [],
                string.Empty,
                StatusCodes.Status400BadRequest
            ));
        }
        return Result<string>.Success(refreshToken);
    }

    public static Result<Guid> GetDeviceIdCookie(this ControllerBase controller)
    {
        var deviceIdString = controller.Request.Cookies["deviceId"] ?? string.Empty;
        if (string.IsNullOrEmpty(deviceIdString) || !Guid.TryParse(deviceIdString, out var deviceId))
        {
            return Result<Guid>.Failure(new Error(
                "AUTH_004",
                "Device ID cookie is missing or invalid.",
                [],
                string.Empty,
                StatusCodes.Status400BadRequest
            ));
        }
        return Result<Guid>.Success(deviceId);
    }

    public static void AddDeviceIdCookie(this ControllerBase controller, Guid deviceId)
    {
        controller.Response.Cookies.Append(
            "deviceId",
            deviceId.ToString(),
            new CookieOptions
            {
                HttpOnly = true,   // Important: Prevent JS from stealing it
                Secure = true,     // Only send over HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            }
        );
    }

    public static Result<Guid> GetAuthenticatedUserId(this ControllerBase controller)
    {
        var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier) ?? controller.User.FindFirst("sub");
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Result<Guid>.Failure(new Error(
                "AUTH_001",
                "User ID not found in token claims or invalid format.",
                [],
                string.Empty,
                StatusCodes.Status401Unauthorized
            ));
        }
        
        return Result<Guid>.Success(userId);
    }

    public static Result<IPAddress> GetClientIpAddress(this ControllerBase controller)
    {
        var remoteIp = controller.HttpContext.Connection.RemoteIpAddress;
        if (remoteIp is null)
        {
            return Result<IPAddress>.Failure(new Error(
                "AUTH_003",
                "Client IP address could not be determined.",
                [],
                string.Empty,
                StatusCodes.Status400BadRequest
            ));
        }
        return Result<IPAddress>.Success(remoteIp);
    }
}