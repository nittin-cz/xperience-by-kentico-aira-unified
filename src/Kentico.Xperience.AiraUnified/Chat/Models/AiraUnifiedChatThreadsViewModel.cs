namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// View model for displaying a collection of chat threads.
/// </summary>
internal sealed class AiraUnifiedChatThreadsViewModel
{
    /// <summary>
    /// Gets or sets the list of chat threads.
    /// </summary>
    public List<AiraUnifiedChatThreadModel> ChatThreads { get; set; } = [];
}
