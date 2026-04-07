using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.TechdlePlayer;

namespace Tests.IntergrationTests.TechdlePlayer;

public static class TechdlePlayerTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        PostGuessAsync<TResponse>(HttpClient client, TechdleGuessRequestDto request)
    {
        var response = await client.PostAsJsonAsync("/api/v1/techdle/guess", request);
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }

        return (response, content, json);
    }
}
