using Kentico.Xperience.AiraUnified.Models;

namespace Kentico.Xperience.AiraUnified.Services;

public class ChatService : IChatService
{
    private static readonly Dictionary<string, List<ChatMessage>> _chatHistory = new();

    public async Task<string> SendMessageAsync(string message, string sessionId)
    {
        // Simulace AI odpovědi - nahraďte skutečnou AI integrací
        await Task.Delay(500);
            
        var userMessage = new ChatMessage
        {
            Content = message,
            Author = "User",
            Type = ChatMessageType.User
        };

        if (!_chatHistory.ContainsKey(sessionId))
            _chatHistory[sessionId] = new List<ChatMessage>();

        _chatHistory[sessionId].Add(userMessage);

        // Simulace AI odpovědi
        var aiResponse = $"Rozumím vaší otázce: '{message}'. Jak vám mohu pomoci?";
            
        var aiMessage = new ChatMessage
        {
            Content = aiResponse,
            Author = "AI Assistant",
            Type = ChatMessageType.AI
        };

        _chatHistory[sessionId].Add(aiMessage);

        return aiResponse;
    }

    public async Task<List<ChatMessage>> GetChatHistoryAsync(string sessionId)
    {
        await Task.CompletedTask;
        return _chatHistory.ContainsKey(sessionId) ? _chatHistory[sessionId] : new List<ChatMessage>();
    }
}
