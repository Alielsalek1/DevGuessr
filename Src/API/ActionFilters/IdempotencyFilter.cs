using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace API.ActionFilters;

public class IdempotencyFilter(IDistributedCache cache, ILogger<IdempotencyFilter> logger, IOptions<JsonOptions> jsonOptions) : IAsyncActionFilter
{
    private readonly IDistributedCache cache = cache;
    private readonly ILogger<IdempotencyFilter> logger = logger;
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions.Value.JsonSerializerOptions;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogInformation("Executing IdempotencyFilter for {Path}", context.HttpContext.Request.Path);
        // 1. Check for Header
        if (!context.HttpContext.Request.Headers.TryGetValue("Idempotency-Key", out var keyValues) || string.IsNullOrWhiteSpace(keyValues.ToString()))
        {
            logger.LogWarning("Idempotency key missing for request to {Path}", context.HttpContext.Request.Path);
            var response = new Application.Utils.FailApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Idempotency-Key header is missing or empty",
                ErrorCode = Application.Constants.ApiErrorCodes.ValidationErrorCode,
                TraceId = context.HttpContext.TraceIdentifier
            };
            context.Result = new BadRequestObjectResult(response);
            return;
        }

        string key = keyValues.ToString();
        string cacheKey = $"idempotency:{key}";

        // 2. Hash the Request Body (Safety Check)
        string requestHash = await ComputeBodyHashAsync(context.HttpContext.Request);

        // 3. Check Cache
        var cachedData = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            var record = JsonSerializer.Deserialize<IdempotencyRecord>(cachedData, _jsonOptions);

            // VALIDATION: Ensure the key isn't being reused for a different request
            if (record!.RequestHash != requestHash)
            {
                logger.LogWarning("Idempotency key {Key} reused for different request content at {Path}", key, context.HttpContext.Request.Path);
                context.Result = new ConflictObjectResult(new { error = "Idempotency key reused for different request parameters" });
                return;
            }

            logger.LogInformation("Idempotency key {Key} hit for {Path}. Returning cached response.", key, context.HttpContext.Request.Path);

            // SUCCESS: Return cached response immediately
            context.Result = new ObjectResult(record.ResponseBody) { StatusCode = record.StatusCode };
            return;
        }

        logger.LogInformation("Idempotency key miss for {Path}. Executing action.", context.HttpContext.Request.Path);

        // 4. Execute Controller
        var executedContext = await next();

        // 5. Cache the Result (Only if successful)
        if (executedContext.Result is ObjectResult result && result.StatusCode >= 200 && result.StatusCode < 300)
        {
            logger.LogInformation("Caching successful response for idempotency key {Key}", key);
            var record = new IdempotencyRecord
            (
                requestHash,
                result.StatusCode ?? 200,
                result.Value ?? new { }
            );

            await cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(record, _jsonOptions),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }
            );
        }
    }

    private static async Task<string> ComputeBodyHashAsync(HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(body)));
    }

    private record IdempotencyRecord(string RequestHash, int StatusCode, object ResponseBody);

}