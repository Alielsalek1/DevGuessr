namespace Application.Utils;

public record SuccessApiResponse<T>
{
    public bool Success { get; init; } = true;
    public int StatusCode { get; init; }
    public string Message { get; init; } = null!;
    public T Data { get; init; } = default!;
    public string TraceId { get; init; } = null!;
}

public record SuccessApiResponse
{
    public bool Success { get; init; } = true;
    public int StatusCode { get; init; }
    public string Message { get; init; } = null!;
    public string TraceId { get; init; } = null!;
}