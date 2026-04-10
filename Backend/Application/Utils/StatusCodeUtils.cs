namespace Application.Utils;
public static class StatusCodeUtils
{
    public static bool IsSuccessStatusCode(int statusCode) => statusCode >= 200 && statusCode <= 299;
    public static bool IsFailureStatusCode(int statusCode) => !IsSuccessStatusCode(statusCode);

    public static bool IsNoBodyStatusCode(int statusCode) =>
        (statusCode >= 100 && statusCode <= 199) ||
        statusCode == 204 ||
        statusCode == 205 ||
        statusCode == 304;
}