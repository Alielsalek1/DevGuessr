using System.Text.Json;

namespace Tests.Auth;

public static class GuestLoginTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json, string? RefreshTokenCookie)>
        PostGuestLoginAsync<TResponse>(HttpClient client)
    {
        // Create request with idempotency key (required for this endpoint)
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal-auth/login/guest");
        requestMessage.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await client.SendAsync(requestMessage);
        var json = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);

        // Extract the refresh token cookie from the response
        string? refreshTokenCookie = null;
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var refreshTokenCookieHeader = cookies.FirstOrDefault(c => c.StartsWith("refreshToken="));
            if (refreshTokenCookieHeader != null)
            {
                // Extract just the value (before the first semicolon)
                var cookieValue = refreshTokenCookieHeader.Split(';')[0].Replace("refreshToken=", "");
                refreshTokenCookie = cookieValue;
            }
        }

        return (response, content, json, refreshTokenCookie);
    }
}
