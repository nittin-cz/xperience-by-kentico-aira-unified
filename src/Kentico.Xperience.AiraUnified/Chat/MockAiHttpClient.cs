using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Insights.Models;

using static Kentico.Xperience.AiraUnified.Chat.Models.ChatStateType;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Mock implementation of HTTP client communication with AI service for local development.
/// </summary>
internal sealed class MockAiHttpClient : IAiHttpClient
{
    /// <inheritdoc />
    public Task<AiraUnifiedAIResponse?> SendRequestAsync(AiraUnifiedAIRequest request)
    {
        // Simulate network delay
        Thread.Sleep(500);

        return request.ChatState switch
        {
            nameof(initial) => GetInitialMessageResponse(),
            nameof(returning) => GetReturningMessageResponse(),
            nameof(ongoing) => GetOngoingMessageResponse(request),
            _ => Task.FromResult<AiraUnifiedAIResponse?>(null)
        };
    }


    private static Task<AiraUnifiedAIResponse?> GetInitialMessageResponse() => Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
    {
        Responses =
        [
            new ResponseMessageModel { Content = "MOCK: Hello! I'm your AI assistant. How can I help you today?", ContentType = "text" }
        ],
        QuickOptions =
        [
            "What can you help me with?",
            "How do I create content?",
            "How do I manage contacts?"
        ]
    });


    private static Task<AiraUnifiedAIResponse?> GetReturningMessageResponse() => Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
    {
        Responses =
        [
            new ResponseMessageModel { Content = "MOCK: Welcome back! I'm here to help you. What would you like to know?", ContentType = "text" }
        ],
        QuickOptions =
        [
            "Continue previous conversation",
            "Start new topic",
            "Show me my content"
        ]
    });


    private static Task<AiraUnifiedAIResponse?> GetOngoingMessageResponse(AiraUnifiedAIRequest request)
    {
        // Check if the message contains any insights-related keywords
        var isInsightsQuery = request.ChatMessage.Contains("insights", StringComparison.OrdinalIgnoreCase);
        var category = isInsightsQuery ? GetInsightsCategory(request.ChatMessage) : null;

        return Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
        {
            Summary = $"This is a mock response to: {request.ChatMessage}",
            Responses =
            [
                new ResponseMessageModel { Content = $"I understand you're asking about: {request.ChatMessage}", ContentType = "text" }
            ],
            Insights = isInsightsQuery ? new InsightsResponseModel
            {
                IsInsightsQuery = true,
                Category = category,
                Metadata = new InsightsMetadataModel
                {
                    Timestamp = DateTime.UtcNow
                }
            } : new InsightsResponseModel { IsInsightsQuery = false }
        });
    }


    private static string? GetInsightsCategory(string message)
    {
        message = message.ToLowerInvariant();
        return message switch
        {
            var m when m.Contains("content") => "content",
            var m when m.Contains("email") => "email",
            var m when m.Contains("marketing") => "marketing",
            _ => null
        };
    }
}
