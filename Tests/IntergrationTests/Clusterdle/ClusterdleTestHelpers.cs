using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.Clusterdle;

namespace Tests.TechnectionCategories;

public static class TechnectionCategoriesTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static CreateClusterdleRequestDto BuildValidRequest(
        string groupName = "Git Commands",
        int difficultyLevel = 1,
        List<string>? words = null,
        string successMessage = "You got it!") =>
        new()
        {
            GroupName = groupName,
            DifficultyLevel = difficultyLevel,
            Words = words ?? ["commit", "push", "pull", "merge"],
            SuccessMessage = successMessage
        };

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        GetPagedAsync<TResponse>(HttpClient client, int pageNumber = 1, int pageSize = 10)
    {
        var response = await client.GetAsync($"/api/v1/clusterdle?pageNumber={pageNumber}&pageSize={pageSize}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        GetByGroupNameAsync<TResponse>(HttpClient client, string groupName)
    {
        var response = await client.GetAsync($"/api/v1/clusterdle/{groupName}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        CreateAsync<TResponse>(HttpClient client, CreateClusterdleRequestDto request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/clusterdle")
        {
            Content = JsonContent.Create(request)
        };

        var response = await client.SendAsync(message);
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        UpdateAsync<TResponse>(HttpClient client, string groupName, UpdateClusterdleByGroupNameRequestDto request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/clusterdle/{groupName}")
        {
            Content = JsonContent.Create(request)
        };

        var response = await client.SendAsync(message);
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        AddWordAsync<TResponse>(HttpClient client, string groupName, AddClusterdleWordByGroupNameRequestDto request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/clusterdle/{groupName}/words")
        {
            Content = JsonContent.Create(request)
        };

        var response = await client.SendAsync(message);
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        RemoveWordAsync<TResponse>(HttpClient client, string groupName, RemoveClusterdleWordByGroupNameRequestDto request)
    {
        var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/clusterdle/{groupName}/words")
        {
            Content = JsonContent.Create(request)
        };

        var response = await client.SendAsync(message);
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        DeleteAsync<TResponse>(HttpClient client, string groupName)
    {
        var response = await client.DeleteAsync($"/api/v1/clusterdle/{groupName}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }
}
