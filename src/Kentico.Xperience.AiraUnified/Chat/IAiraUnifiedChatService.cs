using Kentico.Xperience.AiraUnified.Chat.Models;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Service responsible for managing chat history of a user.
/// </summary>
public interface IAiraUnifiedChatService
{
    /// <summary>
    /// Returns the chat history of a user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>A Task returning a List of <see cref="AiraUnifiedChatMessageViewModel"/> in User's history.</returns>
    Task<List<AiraUnifiedChatMessageViewModel>> GetUserChatHistory(int userId, int threadId);

    /// <summary>
    /// Generates new suggested prompts for a user and saves them in the history.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="suggestions">The prompt suggestions.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>A task returning a <see cref="AiraUnifiedChatMessageViewModel"/> with the generated prompts.</returns>
    Task<AiraUnifiedPromptGroupModel> SaveAiraPrompts(int userId, List<string> suggestions, int threadId);

    /// <summary>
    /// Removes used prompt group.
    /// </summary>
    /// <param name="promptGroupId">Prompt group id.</param>
    void RemoveUsedPrompts(string promptGroupId);

    /// <summary>
    /// Gets a chat thread model of the specified id. If the id is null the latest used thread will be returned. If no thread for the user exists, a new thread for the user will be created.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The desired thread id or null.</param>
    /// <returns>The task containing the desired <see cref="AiraUnifiedChatThreadModel"/>.</returns>
    Task<AiraUnifiedChatThreadModel> GetAiraChatThreadModel(int userId, int? threadId = null);

    /// <summary>
    /// Creates new chat thread for the specified user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <returns></returns>
    Task<AiraUnifiedChatThreadModel> CreateNewChatThread(int userId);

    /// <summary>
    /// Saves a text message in the history.
    /// </summary>
    /// <param name="text">Text of the message.</param>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <param name="role">Role of the chat member.</param>
    Task SaveMessage(string text, int userId, string role, int threadId);

    /// <summary>
    /// Calls the ai endpoint with a message from the user.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <param name="numberOfIncludedHistoryMessages">Number of history messages added to the context.</param>
    /// <param name="userId">The user id.</param>
    /// <returns>A task returning the <see cref="AiraUnifiedAIResponse"/> with the ai response.</returns>
    Task<AiraUnifiedAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId);

    /// <summary>
    /// Updates the chat summary of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="summary">New summary.</param>
    Task UpdateChatSummary(int userId, string summary);
}
