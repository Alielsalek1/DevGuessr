using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.ProgrammingLanguage;

namespace Tests.ProgrammingLanguages;

public static class ProgrammingLanguagesTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        GetPagedAsync<TResponse>(HttpClient client, int pageNumber = 1, int pageSize = 10)
    {
        var response = await client.GetAsync($"/api/v1/programming-languages?pageNumber={pageNumber}&pageSize={pageSize}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        GetByNameAsync<TResponse>(HttpClient client, string name)
    {
        var response = await client.GetAsync($"/api/v1/programming-languages/{name}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        CreateAsync<TResponse>(HttpClient client, CreateProgrammingLanguageRequestDto request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/programming-languages")
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
        UpdateAsync<TResponse>(HttpClient client, string name, UpdateProgrammingLanguageByNameRequestDto request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/programming-languages/{name}")
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
        AddTagAsync<TResponse>(HttpClient client, string name, AddProgrammingLanguageTagByNameRequestDto request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/programming-languages/{name}/tags")
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
        RemoveTagAsync<TResponse>(HttpClient client, string name, RemoveProgrammingLanguageTagByNameRequestDto request)
    {
        var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/programming-languages/{name}/tags")
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
        DeleteAsync<TResponse>(HttpClient client, string name)
    {
        var response = await client.DeleteAsync($"/api/v1/programming-languages/{name}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }
}
