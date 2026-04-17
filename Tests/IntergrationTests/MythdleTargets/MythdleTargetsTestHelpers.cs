using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.MythdleTarget;

namespace Tests.MythdleTargets;

public static class MythdleTargetsTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        GetPagedAsync<TResponse>(HttpClient client, int pageNumber = 1, int pageSize = 10)
    {
        var response = await client.GetAsync($"/api/v1/mythdle-targets?pageNumber={pageNumber}&pageSize={pageSize}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }

        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        CreateAsync<TResponse>(HttpClient client, CreateMythdleTargetDto request)
    {
        var response = await client.PostAsJsonAsync("/api/v1/mythdle-targets", request);
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }

        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        DeleteAsync<TResponse>(HttpClient client, string name)
    {
        var response = await client.DeleteAsync($"/api/v1/mythdle-targets/{name}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }

        return (response, content, json);
    }
}