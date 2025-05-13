namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// Represents a chat message in the Aira Unified system.
/// </summary>
internal sealed class AiraUnifiedChatMessageModel
{
    /// <summary>
    /// Gets or sets the role of the message sender (e.g., "user", "assistant", "system").
    /// </summary>
    public string Role { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets the content of the chat message.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
