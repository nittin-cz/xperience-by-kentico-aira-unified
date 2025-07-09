using Kentico.Xperience.AiraUnified.Chat.Models;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Blazor-specific chat service that wraps the existing chat functionality
/// </summary>
internal interface IBlazorChatService
{
    /// <summary>
    /// Gets chat history for a specific thread
    /// </summary>
    Task<List<AiraUnifiedChatMessageViewModel>> GetChatHistoryAsync(int userId, int threadId);

    /// <summary>
    /// Sends a message and returns the AI response
    /// </summary>
    Task<AiraUnifiedChatMessageViewModel?> SendMessageAsync(string message, int userId, int threadId);

    /// <summary>
    /// Gets or creates a chat thread for the user
    /// </summary>
    Task<AiraUnifiedChatThreadModel> GetOrCreateThreadAsync(int userId, int? threadId = null);

    /// <summary>
    /// Gets all threads for a user
    /// </summary>
    Task<List<AiraUnifiedChatThreadModel>> GetThreadsAsync(int userId);

    /// <summary>
    /// Creates a new chat thread
    /// </summary>
    Task<AiraUnifiedChatThreadModel> CreateNewThreadAsync(int userId);

    /// <summary>
    /// Removes used prompt group
    /// </summary>
    Task RemoveUsedPromptsAsync(string promptGroupId);
}
