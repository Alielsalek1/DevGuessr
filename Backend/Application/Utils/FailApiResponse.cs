namespace Application.Utils;

public record FailApiResponse
{
    public bool Success { get; init; } = false;
    public int StatusCode { get; init; }
    public string Message { get; init; } = null!;
    public Dictionary<string, string[]> Errors { get; init; } = [];
    public string ErrorCode { get; init; } = null!;
    public string TraceId { get; init; } = null!;
}