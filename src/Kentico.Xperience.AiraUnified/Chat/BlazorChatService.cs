using Kentico.Membership;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Chat.Models;

using Microsoft.AspNetCore.Components.Authorization;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Blazor-specific chat service implementation
/// </summary>
internal sealed class BlazorChatService : IBlazorChatService
{
    private readonly IAiraUnifiedChatService airaUnifiedChatService;
    private readonly AdminUserManager adminUserManager;
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public BlazorChatService(
        IAiraUnifiedChatService airaUnifiedChatService,
        AdminUserManager adminUserManager,
        AuthenticationStateProvider authenticationStateProvider)
    {
        this.airaUnifiedChatService = airaUnifiedChatService;
        this.adminUserManager = adminUserManager;
        this.authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<List<AiraUnifiedChatMessageViewModel>> GetChatHistoryAsync(int userId, int threadId)
    {
        return await airaUnifiedChatService.GetUserChatHistory(userId, threadId);
    }

    public async Task<AiraUnifiedChatMessageViewModel?> SendMessageAsync(string message, int userId, int threadId)
    {
        var thread = await airaUnifiedChatService.GetAiraUnifiedThreadInfoOrNull(userId, threadId);
        if (thread == null)
        {
            return null;
        }

        // Save user message
        await airaUnifiedChatService.SaveMessage(message, userId, ChatRoleType.User, thread);

        // Get AI response
        var aiResponse = await airaUnifiedChatService.GetAIResponseOrNull(message, 5, userId);
        if (aiResponse == null)
        {
            return new AiraUnifiedChatMessageViewModel
            {
                ServiceUnavailable = true,
                Role = AiraUnifiedConstants.AiraUnifiedFrontEndChatComponentAIAssistantRoleName
            };
        }

        // Save AI response
        await airaUnifiedChatService.SaveMessage(aiResponse.Responses[0].Content, userId, ChatRoleType.AIAssistant, thread);

        // Update chat summary
        await airaUnifiedChatService.UpdateChatSummary(userId, message);

        var result = new AiraUnifiedChatMessageViewModel
        {
            Role = AiraUnifiedConstants.AiraUnifiedFrontEndChatComponentAIAssistantRoleName,
            Message = aiResponse.Responses[0].Content,
            Insights = aiResponse.Insights
        };

        // Handle quick prompts if available
        if (aiResponse.QuickOptions != null)
        {
            var promptGroup = await airaUnifiedChatService.SaveAiraPrompts(userId, aiResponse.QuickOptions, threadId);
            result.QuickPrompts = promptGroup.QuickPrompts;
            result.QuickPromptsGroupId = promptGroup.QuickPromptsGroupId.ToString();
        }

        return result;
    }

    public async Task<AiraUnifiedChatThreadModel> GetOrCreateThreadAsync(int userId, int? threadId = null)
    {
        return await airaUnifiedChatService.GetAiraChatThreadModel(userId, setAsLastUsed: true, threadId);
    }

    public async Task<List<AiraUnifiedChatThreadModel>> GetThreadsAsync(int userId)
    {
        return await airaUnifiedChatService.GetThreads(userId);
    }

    public async Task<AiraUnifiedChatThreadModel> CreateNewThreadAsync(int userId)
    {
        return await airaUnifiedChatService.CreateNewChatThread(userId);
    }

    public async Task RemoveUsedPromptsAsync(string promptGroupId)
    {
        airaUnifiedChatService.RemoveUsedPrompts(promptGroupId);
        await Task.CompletedTask;
    }

    private async Task<int> GetCurrentUserIdAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = await adminUserManager.GetUserAsync(authState.User);
        return user?.UserID ?? 0;
    }
}
