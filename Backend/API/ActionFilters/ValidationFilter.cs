using Application.Constants;
using Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.ActionFilters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var errorSummary = string.Join(", ", errors.Keys);
            var response = new FailApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Validation failed: {errorSummary}",
                Errors = errors,
                ErrorCode = ApiErrorCodes.ValidationErrorCode,
                TraceId = context.HttpContext.TraceIdentifier
            };

            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}
