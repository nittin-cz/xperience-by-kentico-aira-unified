using System.Text;
using System.Text.Json;

using Kentico.Xperience.AiraUnified.Chat.Models;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Implementation of HTTP client communication with AI service.
/// </summary>
internal sealed class AiHttpClient(IHttpClientFactory httpClientFactory, IOptions<AiraUnifiedOptions> airaUnifiedOptions)
    : IAiHttpClient
{
    private readonly AiraUnifiedOptions airaUnifiedOptions = airaUnifiedOptions.Value;

    /// <inheritdoc />
    public async Task<AiraUnifiedAIResponse?> SendRequestAsync(AiraUnifiedAIRequest request)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        content.Headers.Add("Ocp-Apim-Subscription-Key", airaUnifiedOptions.AiraUnifiedApiSubscriptionKey);

        var endpoint = airaUnifiedOptions.AiraUnifiedAIEndpoint;
        var response = await httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AiraUnifiedAIResponse>(jsonResponse);
    }
}
