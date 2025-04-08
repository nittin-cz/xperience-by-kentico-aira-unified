using CMS.Base;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataEngine.Query;

using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Insights;
using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Service responsible for managing chat history of a user.
/// </summary>
internal sealed class AiraUnifiedChatService : IAiraUnifiedChatService
{
    private readonly IInfoProvider<AiraUnifiedChatPromptGroupInfo> airaUnifiedChatPromptGroupProvider;
    private readonly IInfoProvider<AiraUnifiedChatPromptInfo> airaUnifiedChatPromptProvider;
    private readonly IInfoProvider<AiraUnifiedChatMessageInfo> airaUnifiedChatMessageProvider;
    private readonly IInfoProvider<AiraUnifiedChatSummaryInfo> airaUnifiedChatSummaryProvider;
    private readonly IInfoProvider<AiraUnifiedChatThreadInfo> airaUnifiedChatThreadProvider;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupProvider;
    private readonly IAiraUnifiedInsightsService airaUnifiedInsightsService;
    private readonly IAiHttpClient aiHttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AiraUnifiedChatService"/> class.
    /// </summary>
    /// <param name="airaUnifiedChatPromptGroupProvider">Provider for chat prompt groups.</param>
    /// <param name="airaUnifiedChatPromptProvider">Provider for chat prompts.</param>
    /// <param name="airaUnifiedChatThreadProvider">Provider for chat threads.</param>
    /// <param name="contactGroupProvider">Provider for contact groups.</param>
    /// <param name="airaUnifiedChatMessageProvider">Provider for chat messages.</param>
    /// <param name="airaUnifiedChatSummaryProvider">Provider for chat summaries.</param>
    /// <param name="airaUnifiedInsightsService">Service for insights data.</param>
    /// <param name="aiHttpClient">Client for AI service communication.</param>
    public AiraUnifiedChatService(IInfoProvider<AiraUnifiedChatPromptGroupInfo> airaUnifiedChatPromptGroupProvider,
        IInfoProvider<AiraUnifiedChatPromptInfo> airaUnifiedChatPromptProvider,
        IInfoProvider<AiraUnifiedChatThreadInfo> airaUnifiedChatThreadProvider,
        IInfoProvider<ContactGroupInfo> contactGroupProvider,
        IInfoProvider<AiraUnifiedChatMessageInfo> airaUnifiedChatMessageProvider,
        IInfoProvider<AiraUnifiedChatSummaryInfo> airaUnifiedChatSummaryProvider,
        IAiraUnifiedInsightsService airaUnifiedInsightsService,
        IAiHttpClient aiHttpClient)
    {
        this.airaUnifiedChatPromptGroupProvider = airaUnifiedChatPromptGroupProvider;
        this.airaUnifiedChatPromptProvider = airaUnifiedChatPromptProvider;
        this.contactGroupProvider = contactGroupProvider;
        this.airaUnifiedChatMessageProvider = airaUnifiedChatMessageProvider;
        this.airaUnifiedChatThreadProvider = airaUnifiedChatThreadProvider;
        this.airaUnifiedInsightsService = airaUnifiedInsightsService;
        this.airaUnifiedChatSummaryProvider = airaUnifiedChatSummaryProvider;
        this.aiHttpClient = aiHttpClient;
    }

    /// <inheritdoc />
    public async Task<List<AiraUnifiedChatMessageViewModel>> GetUserChatHistory(int userId, int threadId)
    {
        var chatPrompts = (await airaUnifiedChatPromptProvider
            .Get()
            .Source(x => x.InnerJoin<AiraUnifiedChatPromptGroupInfo>(
                nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptChatPromptGroupId),
                nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId)
            ))
            .WhereEquals(nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupUserId), userId)
            .WhereEquals(nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupThreadId), threadId)
            .Columns(nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupCreatedWhen),
                nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId),
                nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptText))
            .OrderBy(nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupCreatedWhen))
            .GetDataContainerResultAsync())
            .GroupBy(x =>
                new
                {
                    PromptGroupId = x[nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId)] as int?
                }
            );

        var textMessages = (await airaUnifiedChatMessageProvider.Get()
            .WhereEquals(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageUserId), userId)
            .WhereEquals(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageThreadId), threadId)
            .GetEnumerableTypedResultAsync())
            .Select(x => new AiraUnifiedChatMessageViewModel
            {
                Role = GetChatRole(x).ToLowerInvariant(),
                CreatedWhen = x.AiraUnifiedChatMessageCreatedWhen,
                Message = x.AiraUnifiedChatMessageText
            });

        return chatPrompts.Select(x =>
        {
            var prompts = x.AsEnumerable();

            return new AiraUnifiedChatMessageViewModel
            {
                QuickPrompts = prompts.Select(x => (string)x[nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptText)]).ToList(),
                Role = AiraUnifiedConstants.AiraUnifiedChatRoleName,
                QuickPromptsGroupId = x.Key.PromptGroupId!.ToString()!,
                CreatedWhen = (DateTime)prompts.First()[nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupCreatedWhen)]
            };
        })
        .Union(textMessages)
        .OrderBy(x => x.CreatedWhen)
        .ToList();
    }

    /// <inheritdoc />
    public async Task<AiraUnifiedChatThreadModel> GetAiraChatThreadModel(int userId, bool setAsLastUsed, int? threadId = null)
    {
        if (threadId is null)
        {
            var numberOfThreads = await airaUnifiedChatThreadProvider
                .Get()
                .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadUserId), userId)
                .GetCountAsync();

            if (numberOfThreads == 0)
            {
                return await CreateNewChatThread(userId);
            }

            var latestUsedThread = airaUnifiedChatThreadProvider
                .Get()
                .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadUserId), userId)
                .OrderByDescending(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadLastUsedWhen))
                .TopN(1)
                .SingleOrDefault() ?? throw new InvalidOperationException($"No thread exists for the user with id {userId}.");

            if (setAsLastUsed)
            {
                latestUsedThread.AiraUnifiedChatThreadLastUsedWhen = DateTime.UtcNow;
                await airaUnifiedChatThreadProvider.SetAsync(latestUsedThread);
            }

            return new AiraUnifiedChatThreadModel
            {
                ThreadName = latestUsedThread.AiraUnifiedChatThreadName,
                ThreadId = latestUsedThread.AiraUnifiedChatThreadId,
            };
        }

        var chatThread = airaUnifiedChatThreadProvider
            .Get()
            .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadUserId), userId)
            .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadId), threadId.Value)
            .FirstOrDefault() ?? throw new InvalidOperationException($"The specified thread with id {threadId} for the specified user with id {userId} does not exist.");

        if (setAsLastUsed)
        {
            chatThread.AiraUnifiedChatThreadLastUsedWhen = DateTime.UtcNow;
            await airaUnifiedChatThreadProvider.SetAsync(chatThread);
        }

        return new AiraUnifiedChatThreadModel
        {
            ThreadName = chatThread.AiraUnifiedChatThreadName,
            ThreadId = chatThread.AiraUnifiedChatThreadId
        };
    }

    /// <inheritdoc />
    public async Task<AiraUnifiedChatThreadInfo?> GetAiraUnifiedThreadInfoOrNull(int userId, int threadId)
    => (await airaUnifiedChatThreadProvider
    .Get()
    .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadId), threadId)
    .TopN(1)
    .GetEnumerableTypedResultAsync())
    .FirstOrDefault();


    /// <inheritdoc />
    public async Task<List<AiraUnifiedChatThreadModel>> GetThreads(int userId)
    => (await airaUnifiedChatThreadProvider
    .Get()
    .Source(x => x.LeftJoin<AiraUnifiedChatMessageInfo>(
        nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadLastMessageId),
        nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageId)
    ))
    .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadUserId), userId)
    .Columns(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadId),
        nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageCreatedWhen),
        nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageText),
        nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadName)
    )
    .OrderByDescending(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadLastUsedWhen))
    .GetEnumerableTypedResultAsync(x =>
    {
        var dataContainer = new DataRecordContainer(x);

        var threadModel = new AiraUnifiedChatThreadModel
        {
            ThreadName = (string)dataContainer[nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadName)],
            ThreadId = (int)dataContainer[nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadId)]
        };

        if (dataContainer.TryGetValue(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageText), out var message) && message is string messageValue)
        {
            threadModel.LastMessage = messageValue;
        }
        if (dataContainer.TryGetValue(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageCreatedWhen), out var lastUsed) && lastUsed is DateTime lastUsedValue)
        {
            threadModel.LastUsed = lastUsedValue;
        }

        return threadModel;
    }))
    .ToList();

    /// <inheritdoc />
    public async Task<AiraUnifiedChatMessageInfo> SaveMessage(string text, int userId, ChatRoleType role, AiraUnifiedChatThreadInfo thread)
    {
        var message = new AiraUnifiedChatMessageInfo
        {
            AiraUnifiedChatMessageText = text,
            AiraUnifiedChatMessageCreatedWhen = DateTime.Now,
            AiraUnifiedChatMessageThreadId = thread.AiraUnifiedChatThreadId,
            AiraUnifiedChatMessageUserId = userId,
            AiraUnifiedChatMessageRole = (int)role
        };

        await airaUnifiedChatMessageProvider.SetAsync(message);

        thread.AiraUnifiedChatThreadLastMessageId = message.AiraUnifiedChatMessageId;
        await airaUnifiedChatThreadProvider.SetAsync(thread);

        return message;
    }

    /// <inheritdoc />
    public async Task<bool> ValidateUserThread(int userId, int threadId)
    {
        var thread = (await airaUnifiedChatThreadProvider
            .Get()
            .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadId), threadId)
            .TopN(1)
            .GetEnumerableTypedResultAsync())
            .FirstOrDefault();

        return thread is not null && thread.AiraUnifiedChatThreadUserId == userId;
    }

    /// <inheritdoc />
    public async Task<AiraUnifiedAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId)
    {
        var textMessageHistory = (await airaUnifiedChatMessageProvider.Get()
            .WhereEquals(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageUserId), userId)
            .OrderByDescending(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageCreatedWhen))
            .TopN(numberOfIncludedHistoryMessages)
            .GetEnumerableTypedResultAsync())
            .Select(item => new AiraUnifiedChatMessageModel
            {
                Role = GetChatRole(item),
                Content = item.AiraUnifiedChatMessageText
            })
            .ToList();

        var conversationSummary = airaUnifiedChatSummaryProvider.Get()
            .WhereEquals(nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryUserId), userId)
            .FirstOrDefault();

        if (conversationSummary is not null)
        {
            textMessageHistory.Add(new AiraUnifiedChatMessageModel
            {
                Role = AiraUnifiedConstants.AIRequestAssistantRoleName,
                Content = conversationSummary.AiraUnifiedChatSummaryContent
            });
        }

        var request = new AiraUnifiedAIRequest
        {
            ChatMessage = message,
            ConversationHistory = textMessageHistory,
            ChatState = nameof(ChatStateType.ongoing)
        };

        var aiResponse = await aiHttpClient.SendRequestAsync(request);

        await AddInsightsData(userId, aiResponse);

        return aiResponse;
    }

    /// <inheritdoc />
    public async Task<AiraUnifiedAIResponse?> GetInitialAIMessage(ChatStateType chatState)
    {
        var request = new AiraUnifiedAIRequest
        {
            ChatState = chatState switch
            {
                ChatStateType.initial => nameof(ChatStateType.initial),
                ChatStateType.returning => nameof(ChatStateType.returning),
                ChatStateType.ongoing => nameof(ChatStateType.ongoing),
                _ => throw new NotImplementedException($"The {chatState} is missing implementation in {nameof(GetAIResponseOrNull)}")
            }
        };

        return await aiHttpClient.SendRequestAsync(request);
    }

    /// <inheritdoc />
    public async Task<AiraUnifiedPromptGroupModel> SaveAiraPrompts(int userId, List<string> suggestions, int threadId)
    {
        var chatPromptGroup = new AiraUnifiedChatPromptGroupInfo
        {
            AiraUnifiedChatPromptGroupCreatedWhen = DateTime.UtcNow,
            AiraUnifiedChatPromptGroupUserId = userId,
            AiraUnifiedChatPromptGroupThreadId = threadId
        };

        await airaUnifiedChatPromptGroupProvider.SetAsync(chatPromptGroup);

        var messages = new List<AiraUnifiedChatPromptInfo>();

        foreach (var suggestion in suggestions)
        {
            var prompt = new AiraUnifiedChatPromptInfo
            {
                AiraUnifiedChatPromptText = suggestion,
                AiraUnifiedChatPromptChatPromptGroupId = chatPromptGroup.AiraUnifiedChatPromptGroupId
            };

            await airaUnifiedChatPromptProvider.SetAsync(prompt);

            messages.Add(prompt);
        }

        return new AiraUnifiedPromptGroupModel
        {
            QuickPromptsGroupId = chatPromptGroup.AiraUnifiedChatPromptGroupId,
            QuickPrompts = messages.Select(x => x.AiraUnifiedChatPromptText),
        };
    }

    /// <inheritdoc />
    public async Task UpdateChatSummary(int userId, string summary)
    {
        var summaryInfo = airaUnifiedChatSummaryProvider
            .Get()
            .WhereEquals(nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryUserId), userId)
            .FirstOrDefault()
            ??
            new AiraUnifiedChatSummaryInfo
            {
                AiraUnifiedChatSummaryUserId = userId
            };

        summaryInfo.AiraUnifiedChatSummaryContent = summary;

        await airaUnifiedChatSummaryProvider.SetAsync(summaryInfo);
    }

    /// <inheritdoc />
    public async Task<AiraUnifiedChatThreadModel> CreateNewChatThread(int userId)
    {
        var countOfThreads = await airaUnifiedChatThreadProvider
            .Get()
            .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadUserId), userId)
            .GetCountAsync();

        var newChatThread = new AiraUnifiedChatThreadInfo
        {
            AiraUnifiedChatThreadUserId = userId,
            AiraUnifiedChatThreadLastUsedWhen = DateTime.UtcNow,
            AiraUnifiedChatThreadName = $"Chat {countOfThreads + 1}"
        };

        await airaUnifiedChatThreadProvider.SetAsync(newChatThread);

        return new AiraUnifiedChatThreadModel
        {
            ThreadId = newChatThread.AiraUnifiedChatThreadId,
            ThreadName = newChatThread.AiraUnifiedChatThreadName
        };
    }

    /// <inheritdoc />
    public void RemoveUsedPrompts(string promptGroupId)
    {
        if (int.TryParse(promptGroupId, out var id))
        {
            airaUnifiedChatPromptGroupProvider.BulkDelete(new WhereCondition($"{nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId)} = {id}"));
            airaUnifiedChatPromptProvider.BulkDelete(new WhereCondition($"{nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptChatPromptGroupId)} = {id}"));
        }
    }

    /// <summary>
    /// Adds insights data to the AI response based on the category.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="aiResponse">The AI response to enrich with insights.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddInsightsData(int userId, AiraUnifiedAIResponse? aiResponse)
    {
        if (aiResponse?.Insights is { IsInsightsQuery: true })
        {
            var category = aiResponse.Insights.Category ?? string.Empty;

            switch (category.ToLowerInvariant())
            {
                case "content":
                    aiResponse.Insights.InsightsData = await GetContentInsights(userId);
                    break;
                case "email":
                    aiResponse.Insights.InsightsData = await GetEmailInsights();
                    break;
                case "marketing":
                    aiResponse.Insights.InsightsData = await GetMarketingInsights();
                    break;
            }

            if (aiResponse.Insights.InsightsData != null)
            {
                aiResponse.Insights.Metadata = new InsightsMetadataModel()
                {
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }

    /// <summary>
    /// Gets marketing insights data.
    /// </summary>
    /// <returns>A task containing the marketing insights data.</returns>
    private async Task<object?> GetMarketingInsights()
    {
        var contactGroups = await contactGroupProvider.Get().GetEnumerableTypedResultAsync();
        var contactGroupNames = contactGroups.Select(x => x.ContactGroupDisplayName).ToArray();

        var contactGroupInsights = airaUnifiedInsightsService.GetContactGroupInsights(contactGroupNames);

        return new MarketingInsightsDataModel
        {
            Contacts = new ContactsSummaryModel() { TotalCount = contactGroupInsights.AllCount },
            ContactGroups = contactGroupInsights.Groups.Select(item => new ContactGroupModel()
            {
                Name = item.Name,
                ContactCount = item.Count,
                RatioPercentage = (decimal)item.Count / contactGroupInsights.AllCount * 100
            }).ToList()
        };
    }

    /// <summary>
    /// Gets email insights data.
    /// </summary>
    /// <returns>A task containing the email insights data.</returns>
    private async Task<object?> GetEmailInsights()
    {
        var emailInsights = await airaUnifiedInsightsService.GetEmailInsights();

        return new EmailInsightsDataModel()
        {
            Summary = new EmailSummaryModel()
            {
                SentCount = emailInsights.Select(i => i.Metrics).Sum(i => i?.TotalSent ?? 0)
            },
            Campaigns = emailInsights
        };
    }

    /// <summary>
    /// Gets content insights data for a specific user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>A task containing the content insights data.</returns>
    private async Task<ContentInsightsDataModel> GetContentInsights(int userId)
    {
        var reusableDraftContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Reusable, userId, AiraUnifiedConstants.InsightsDraftIdentifier);
        var reusableScheduledContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Reusable, userId, AiraUnifiedConstants.InsightsScheduledIdentifier);
        var websiteDraftContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Website, userId, AiraUnifiedConstants.InsightsDraftIdentifier);
        var websiteScheduledContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Website, userId, AiraUnifiedConstants.InsightsScheduledIdentifier);

        return new ContentInsightsDataModel
        {
            Summary = new ContentSummaryModel()
            {
                DraftCount = reusableDraftContentInsights.Count + websiteDraftContentInsights.Count,
                ScheduledCount = reusableScheduledContentInsights.Count + websiteScheduledContentInsights.Count,
                // PublishedCount = 43,
                // TotalCount = 123
            },
            ReusableContent = new ContentCategoryModel()
            {
                DraftCount = reusableDraftContentInsights.Count,
                ScheduledCount = reusableScheduledContentInsights.Count,
                Items = reusableDraftContentInsights.Concat(reusableScheduledContentInsights).ToList()
            },
            WebsiteContent = new ContentCategoryModel()
            {
                DraftCount = websiteDraftContentInsights.Count,
                ScheduledCount = websiteScheduledContentInsights.Count,
                Items = websiteScheduledContentInsights.Concat(websiteDraftContentInsights).ToList()
            }
        };
    }
    
    /// <summary>
    /// Gets the chat role string based on the message info.
    /// </summary>
    /// <param name="x">The chat message info.</param>
    /// <returns>The role string.</returns>
    private static string GetChatRole(AiraUnifiedChatMessageInfo x) =>
        (ChatRoleType)x.AiraUnifiedChatMessageRole switch
        {
            ChatRoleType.AI => AiraUnifiedConstants.AIRequestAssistantRoleName,
            ChatRoleType.User => AiraUnifiedConstants.AIRequestUserRoleName,
            ChatRoleType.System => AiraUnifiedConstants.AiraUnifiedSystemRoleName,
            _ => AiraUnifiedConstants.AIRequestUserRoleName
        };
}
