using System.Net;
using System.Text.Json;
using Application.Constants;
using Application.Utils;
using Domain.Exceptions;

namespace Techdle.API.Middlewares;
public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An exception was thrown while processing the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var FailResponse = new FailApiResponse();
        if (exception is DomainException domainException)
        {
            FailResponse = new FailApiResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = domainException.Message,
                Errors = [],
                ErrorCode = ApiErrorCodes.DomainErrorCode,
                TraceId = context.TraceIdentifier
            };
        }
        else if (exception is DbException dbException)
        {
            FailResponse = new FailApiResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = dbException.Message,
                Errors = [],
                ErrorCode = ApiErrorCodes.DatabaseErrorCode,
                TraceId = context.TraceIdentifier
            };
        }
        else
        {
            FailResponse = new FailApiResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred. Please try again later.",
                Errors = [],
                ErrorCode = ApiErrorCodes.InternalServerErrorCode,
                TraceId = context.TraceIdentifier
            };
        }
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(FailResponse, options);
        return context.Response.WriteAsync(json);
    }
}
