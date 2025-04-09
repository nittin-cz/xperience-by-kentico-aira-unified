namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// The chat thread model.
/// </summary>
public sealed class AiraUnifiedChatThreadModel
{
    /// <summary>
    /// The name of the thread.
    /// </summary>
    public string ThreadName { get; set; } = string.Empty;

    /// <summary>
    /// The id of the thread.
    /// </summary>
    public int ThreadId { get; set; }

    public DateTime? LastUsed { get; set; }

    public string LastMessage { get; set; } = string.Empty;
}
