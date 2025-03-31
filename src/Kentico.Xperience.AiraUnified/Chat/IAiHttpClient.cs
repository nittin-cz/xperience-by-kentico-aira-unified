using Kentico.Xperience.AiraUnified.Chat.Models;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Interface for HTTP client communication with AI service.
/// </summary>
internal interface IAiHttpClient
{
    /// <summary>
    /// Sends a request to the AI service and returns the response.
    /// </summary>
    /// <param name="request">The request to send to the AI service.</param>
    /// <returns>The AI service response.</returns>
    Task<AiraUnifiedAIResponse?> SendRequestAsync(AiraUnifiedAIRequest request);
}
