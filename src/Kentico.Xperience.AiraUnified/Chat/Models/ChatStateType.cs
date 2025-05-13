namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// State of the chat.
/// </summary>
internal enum ChatStateType
{
    /// <summary>
    /// First time opening a new chat thread.
    /// </summary>
    Initial = 0,

    /// <summary>
    /// Reopening a chat thread.
    /// </summary>
    Returning = 1,

    /// <summary>
    /// Standard message.
    /// </summary>
    Ongoing = 2
}
