using Kentico.Xperience.AiraUnified.Models;

namespace Kentico.Xperience.AiraUnified.Services;

public interface IChatService
{
    Task<string> SendMessageAsync(string message, string sessionId);
    Task<List<ChatMessage>> GetChatHistoryAsync(string sessionId);
}
