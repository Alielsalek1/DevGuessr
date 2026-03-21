using System.Net.Http.Json;

namespace Tests.MailHog;

public class MailhogClient
{
    private readonly HttpClient _httpClient;

    public MailhogClient(string mailhogUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(mailhogUrl) };
    }

    public async Task<MailhogMessagesResponse> GetMessagesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<MailhogMessagesResponse>("/api/v2/messages");
        return response ?? new MailhogMessagesResponse { Total = 0, Count = 0, Items = [] };
    }

    public async Task<MailhogMessagesResponse> SearchMessagesByRecipientAsync(string email)
    {
        var response = await _httpClient.GetFromJsonAsync<MailhogMessagesResponse>($"/api/v2/search?kind=to&query={email}");
        return response ?? new MailhogMessagesResponse { Total = 0, Count = 0, Items = [] };
    }

    public async Task DeleteAllMessagesAsync()
    {
        await _httpClient.DeleteAsync("/api/v1/messages");
    }

    public async Task<MailhogMessage?> GetMessageByIdAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MailhogMessage>($"/api/v1/messages/{id}");
        }
        catch
        {
            return null;
        }
    }
}
