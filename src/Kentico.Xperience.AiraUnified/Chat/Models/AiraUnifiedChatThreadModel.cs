namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// The chat thread model.
/// </summary>
internal sealed class AiraUnifiedChatThreadModel
{
    /// <summary>
    /// The name of the thread.
    /// </summary>
    public string ThreadName { get; set; } = string.Empty;


    /// <summary>
    /// The id of the thread.
    /// </summary>
    public int ThreadId { get; set; }


    /// <summary>
    /// The timestamp when the thread was last used.
    /// </summary>
    public DateTime? LastUsed { get; set; }


    /// <summary>
    /// The content of the last message in the thread.
    /// </summary>
    public string LastMessage { get; set; } = string.Empty;
}
