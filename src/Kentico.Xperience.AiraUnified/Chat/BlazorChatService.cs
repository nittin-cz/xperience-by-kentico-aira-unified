using Kentico.Membership;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Chat.Services;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Models;

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
    private readonly IInsightsStrategyFactory insightsStrategyFactory;
    private readonly EnhancedInsightsParser enhancedInsightsParser;

    public BlazorChatService(
        IAiraUnifiedChatService airaUnifiedChatService,
        AdminUserManager adminUserManager,
        AuthenticationStateProvider authenticationStateProvider,
        IInsightsStrategyFactory insightsStrategyFactory,
        EnhancedInsightsParser enhancedInsightsParser)
    {
        this.airaUnifiedChatService = airaUnifiedChatService;
        this.adminUserManager = adminUserManager;
        this.authenticationStateProvider = authenticationStateProvider;
        this.insightsStrategyFactory = insightsStrategyFactory;
        this.enhancedInsightsParser = enhancedInsightsParser;
    }

    public async Task<List<AiraUnifiedChatMessageViewModel>> GetChatHistoryAsync(int userId, int threadId)
    {
        var historyMessages = await airaUnifiedChatService.GetUserChatHistory(userId, threadId);
        
        foreach (var message in historyMessages)
        {
            if (message.IsInsightsMessage)
            {
                var (category, data, timestamp, componentType) = enhancedInsightsParser.ParseSystemMessage(message.Message!);
                message.InsightsCategory = category;
                message.InsightsData = data;
                message.InsightsTimestamp = timestamp;
                message.ComponentType = componentType;
            }
        }
        
        return historyMessages;
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

        // Save AI response (včetně insights, pokud existují)
        await SaveMessages(aiResponse, userId, thread);

        // Update chat summary
        await airaUnifiedChatService.UpdateChatSummary(userId, message);

        var result = new AiraUnifiedChatMessageViewModel
        {
            Role = AiraUnifiedConstants.AiraUnifiedFrontEndChatComponentAIAssistantRoleName,
            Message = aiResponse.Responses[0].Content,
            Insights = aiResponse.Insights
        };

        // === NOVÁ LOGIKA: Nastavení insights dat pro okamžité renderování ===
        if (aiResponse.Insights?.IsInsightsQuery == true)
        {
            result.InsightsCategory = aiResponse.Insights.Category;
            result.InsightsData = aiResponse.Insights.InsightsData;
            result.InsightsTimestamp = aiResponse.Insights.Metadata?.Timestamp;
            
            // Set component type based on insights category
            if (!string.IsNullOrEmpty(aiResponse.Insights.Category))
            {
                var strategy = insightsStrategyFactory.GetStrategy(aiResponse.Insights.Category);
                result.ComponentType = strategy?.ComponentType;
            }
        }

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
    
    private async Task SaveMessages(AiraUnifiedAIResponse aiResponse, int userId, AiraUnifiedChatThreadInfo thread)
    {
        if (!aiResponse.Insights?.IsInsightsQuery ?? true)
        {
            foreach (var response in aiResponse.Responses)
            {
                await airaUnifiedChatService.SaveMessage(response.Content, userId,
                    ChatRoleType.AIAssistant, thread);
            }
        }
        else
        {
            // Use enhanced serialization format with type information
            var insightsJson = CreateEnhancedInsightsJson(aiResponse.Insights!);
            await airaUnifiedChatService.SaveMessage(insightsJson, userId,
                ChatRoleType.System, thread);
        }
    }
    
    /// <summary>
    /// Creates enhanced JSON format with type information for proper deserialization
    /// </summary>
    private string CreateEnhancedInsightsJson(InsightsResponseModel insights)
    {
        try
        {
            // Get strategy to determine data and component types
            Type? dataType = null;
            Type? componentType = null;
            
            if (!string.IsNullOrEmpty(insights.Category))
            {
                var strategy = insightsStrategyFactory.GetStrategy(insights.Category);
                if (strategy != null)
                {
                    componentType = strategy.ComponentType;
                    
                    // Try to determine data type from the actual insights data
                    if (insights.InsightsData != null)
                    {
                        dataType = insights.InsightsData.GetType();
                    }
                }
            }
            
            // Create enhanced serialization model
            var enhancedModel = InsightsSerializationModel.FromInsightsResponse(insights, dataType, componentType);
            
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            
            return System.Text.Json.JsonSerializer.Serialize(enhancedModel, options);
        }
        catch (Exception)
        {
            // Fallback to legacy format if enhancement fails
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            
            return System.Text.Json.JsonSerializer.Serialize(insights, options);
        }
    }
}
