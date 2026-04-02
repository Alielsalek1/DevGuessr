using System.Net.Http.Headers;
using System.Text.Json;
using Application.DTOs.LogodleTarget;
using SixLabors.ImageSharp;

namespace Tests.LogodleTargets;

public static class LogodleTargetsTestHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    // A dynamically generated valid 100x100 PNG image to pass all ImageSharp processing
    public static readonly byte[] ValidPngImage = GetValidImageBytes();

    private static byte[] GetValidImageBytes()
    {
        using var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(100, 100);
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }


    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        GetPagedAsync<TResponse>(HttpClient client, int pageNumber = 1, int pageSize = 10)
    {
        var response = await client.GetAsync($"/api/v1/logodle-targets?pageNumber={pageNumber}&pageSize={pageSize}");
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
        var response = await client.GetAsync($"/api/v1/logodle-targets/{name}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }

    public static async Task<(HttpResponseMessage Response, TResponse? Content, string Json)>
        CreateAsync<TResponse>(HttpClient client, string name, byte[]? imageBytes = null)
    {
        using var multipartContent = new MultipartFormDataContent();
        
        if (!string.IsNullOrEmpty(name))
        {
            multipartContent.Add(new StringContent(name), "Name");
        }

        if (imageBytes != null)
        {
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            multipartContent.Add(imageContent, "Image", "test-image.png");
        }

        using var message = new HttpRequestMessage(HttpMethod.Post, "/api/v1/logodle-targets")
        {
            Content = multipartContent
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
        var response = await client.DeleteAsync($"/api/v1/logodle-targets/{name}");
        var json = await response.Content.ReadAsStringAsync();
        TResponse? content = default;
        if (!string.IsNullOrWhiteSpace(json))
        {
            content = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        return (response, content, json);
    }
}
