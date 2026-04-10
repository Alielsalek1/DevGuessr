namespace Application.Constants.ApiErrors;
public sealed record Error(string errorCode, string message, Dictionary<string, string[]> errors, string traceId, int statusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, [], string.Empty, 0);
}