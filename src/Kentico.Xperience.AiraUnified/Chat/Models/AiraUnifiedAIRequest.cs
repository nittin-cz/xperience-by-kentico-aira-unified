using System.Text.Json.Serialization;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// Represents a request model for the Aira Unified AI chat service.
/// </summary>
internal class AiraUnifiedAIRequest
{
    /// <summary>
    /// Gets or sets the conversation history containing previous chat messages.
    /// </summary>
    [JsonPropertyName("conversation_history")]
    public List<AiraUnifiedChatMessageModel> ConversationHistory { get; set; } = [];

    /// <summary>
    /// Gets or sets the current chat message being sent to the AI service.
    /// </summary>
    [JsonPropertyName("chat_message")]
    public string ChatMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current state of the chat session.
    /// </summary>
    [JsonPropertyName("chat_state")]
    public string ChatState { get; set; } = string.Empty;
}
