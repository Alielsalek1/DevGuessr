using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.DevGuessrPlayer;

namespace Tests.IntergrationTests.DevGuessrPlayer;

public static class DevGuessrPlayerTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        PostGuessAsync<TResponse>(HttpClient client, DevGuessrGuessRequestDto request)
    {
        var response = await client.PostAsJsonAsync("/api/v1/devguessr/guess", request);
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }

        return (response, content, json);
    }
}
