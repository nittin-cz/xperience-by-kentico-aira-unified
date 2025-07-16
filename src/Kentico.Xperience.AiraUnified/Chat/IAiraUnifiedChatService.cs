using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Chat.Models;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Service responsible for managing chat history of a user.
/// </summary>
internal interface IAiraUnifiedChatService
{
    /// <summary>
    /// Returns the chat history of a user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>A task returning a List of <see cref="AiraUnifiedChatMessageViewModel"/> in User's history.</returns>
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
    /// Gets the chat thread information for a specific user and thread.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>The chat thread information or null if not found.</returns>
    Task<AiraUnifiedChatThreadInfo?> GetAiraUnifiedThreadInfoOrNull(int userId, int threadId);


    /// <summary>
    /// Gets a chat thread model of the specified id. If the id is null the latest used thread will be returned. If no thread for the user exists, a new thread for the user will be created.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="setAsLastUsed">Whether to update the last used timestamp.</param>
    /// <param name="threadId">The desired thread id or null.</param>
    /// <returns>The task containing the desired <see cref="AiraUnifiedChatThreadModel"/>.</returns>
    Task<AiraUnifiedChatThreadModel> GetAiraChatThreadModel(int userId, bool setAsLastUsed, int? threadId = null);


    /// <summary>
    /// Creates new chat thread for the specified user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <returns>A task containing the newly created <see cref="AiraUnifiedChatThreadModel"/>.</returns>
    Task<AiraUnifiedChatThreadModel> CreateNewChatThread(int userId);


    /// <summary>
    /// Gets an enumerable of user's threads ordered from first to last used.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <returns>IEnumerable of user's <see cref="AiraUnifiedChatThreadModel"/>s.</returns>
    Task<List<AiraUnifiedChatThreadModel>> GetThreads(int userId);


    /// <summary>
    /// Saves a text message in the history.
    /// </summary>
    /// <param name="text">Text of the message.</param>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="role">Role of the chat member.</param>
    /// <param name="thread">The chat thread information.</param>
    /// <returns>A task containing the saved <see cref="AiraUnifiedChatMessageInfo"/>.</returns>
    Task<AiraUnifiedChatMessageInfo> SaveMessage(string text, int userId, ChatRoleType role, AiraUnifiedChatThreadInfo thread);


    /// <summary>
    /// Calls the AI endpoint with a message from the user.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <param name="numberOfIncludedHistoryMessages">Number of history messages added to the context.</param>
    /// <param name="userId">The user id.</param>
    /// <returns>A task returning the <see cref="AiraUnifiedAIResponse"/> with the AI response.</returns>
    Task<AiraUnifiedAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId);


    /// <summary>
    /// Gets an initial AI message displayed in a new thread or when returning to a thread.
    /// </summary>
    /// <param name="chatState">The <see cref="ChatStateType"/> of the chat context.</param>
    /// <returns>A task containing the initial AI response.</returns>
    Task<AiraUnifiedAIResponse?> GetInitialAIMessage(ChatStateType chatState);


    /// <summary>
    /// Updates the chat summary of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="summary">New summary.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateChatSummary(int userId, string summary);

    /// <summary>
    /// Gets chat history for a specific thread with enhanced insights parsing.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>A task returning enhanced chat history with insights data.</returns>
    Task<List<AiraUnifiedChatMessageViewModel>> GetChatHistoryAsync(int userId, int threadId);

    /// <summary>
    /// Sends a message and returns the AI response with enhanced insights processing.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The chat thread id.</param>
    /// <returns>A task returning the AI response with insights data.</returns>
    Task<AiraUnifiedChatMessageViewModel?> SendMessageAsync(string message, int userId, int threadId);

    /// <summary>
    /// Gets or creates a chat thread for the user.
    /// </summary>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="threadId">The optional thread id.</param>
    /// <returns>A task returning the chat thread model.</returns>
    Task<AiraUnifiedChatThreadModel> GetOrCreateThreadAsync(int userId, int? threadId = null);

    /// <summary>
    /// Removes used prompt group.
    /// </summary>
    /// <param name="promptGroupId">Prompt group id.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveUsedPromptsAsync(string promptGroupId);

    /// <summary>
    /// Saves AI response messages with enhanced insights processing.
    /// </summary>
    /// <param name="aiResponse">The AI response containing messages and insights.</param>
    /// <param name="userId">Admin application user id.</param>
    /// <param name="thread">The chat thread information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveMessages(AiraUnifiedAIResponse aiResponse, int userId, AiraUnifiedChatThreadInfo thread);
}
