using System.Text;
using System.Text.Json;

using CMS.Base;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataEngine.Query;

using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Insights;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kentico.Xperience.AiraUnified.Chat;

internal class AiraUnifiedChatService : IAiraUnifiedChatService
{
    private readonly IInfoProvider<AiraUnifiedChatPromptGroupInfo> airaUnifiedChatPromptGroupProvider;
    private readonly IInfoProvider<AiraUnifiedChatPromptInfo> airaUnifiedChatPromptProvider;
    private readonly IInfoProvider<AiraUnifiedChatMessageInfo> airaUnifiedChatMessageProvider;
    private readonly IInfoProvider<AiraUnifiedChatSummaryInfo> airaUnifiedChatSummaryProvider;
    private readonly IInfoProvider<AiraUnifiedChatThreadInfo> airaUnifiedChatThreadProvider;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupProvider;
    private readonly IAiraUnifiedInsightsService airaUnifiedInsightsService;
    private readonly AiraUnifiedOptions airaUnifiedOptions;
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AiraUnifiedChatService(IInfoProvider<AiraUnifiedChatPromptGroupInfo> airaUnifiedChatPromptGroupProvider,
        IInfoProvider<AiraUnifiedChatPromptInfo> airaUnifiedChatPromptProvider,
        IInfoProvider<AiraUnifiedChatThreadInfo> airaUnifiedChatThreadProvider,
        IInfoProvider<ContactGroupInfo> contactGroupProvider,
        IInfoProvider<AiraUnifiedChatMessageInfo> airaUnifiedChatMessageProvider,
        IInfoProvider<AiraUnifiedChatSummaryInfo> airaUnifiedChatSummaryProvider,
        IAiraUnifiedInsightsService airaUnifiedInsightsService,
        IOptions<AiraUnifiedOptions> airaUnifiedOptions,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        this.airaUnifiedChatPromptGroupProvider = airaUnifiedChatPromptGroupProvider;
        this.airaUnifiedChatPromptProvider = airaUnifiedChatPromptProvider;
        this.contactGroupProvider = contactGroupProvider;
        this.airaUnifiedChatMessageProvider = airaUnifiedChatMessageProvider;
        this.airaUnifiedChatThreadProvider = airaUnifiedChatThreadProvider;
        this.airaUnifiedInsightsService = airaUnifiedInsightsService;
        this.airaUnifiedChatSummaryProvider = airaUnifiedChatSummaryProvider;
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.airaUnifiedOptions = airaUnifiedOptions.Value;
    }

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
                Role = x.AiraUnifiedChatMessageRole == AiraUnifiedConstants.AiraUnifiedChatRoleIdentifier ?
                    AiraUnifiedConstants.AiraUnifiedChatRoleName :
                    AiraUnifiedConstants.UserChatRoleName,
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

    public async Task<AiraUnifiedChatThreadInfo?> GetAiraUnifiedThreadInfoOrNull(int userId, int threadId)
    => (await airaUnifiedChatThreadProvider
    .Get()
    .WhereEquals(nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadId), threadId)
    .TopN(1)
    .GetEnumerableTypedResultAsync())
    .FirstOrDefault();


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

    public async Task SaveMessage(string text, int userId, string role, AiraUnifiedChatThreadInfo thread)
    {
        var message = new AiraUnifiedChatMessageInfo
        {
            AiraUnifiedChatMessageCreatedWhen = DateTime.UtcNow,
            AiraUnifiedChatMessageThreadId = thread.AiraUnifiedChatThreadId,
            AiraUnifiedChatMessageText = text,
            AiraUnifiedChatMessageUserId = userId,
            AiraUnifiedChatMessageRole = role == AiraUnifiedConstants.AiraUnifiedChatRoleName ?
                AiraUnifiedConstants.AiraUnifiedChatRoleIdentifier :
                AiraUnifiedConstants.UserChatRoleIdentifier
        };

        await airaUnifiedChatMessageProvider.SetAsync(message);

        thread.AiraUnifiedChatThreadLastMessageId = message.AiraUnifiedChatMessageId;
        await airaUnifiedChatThreadProvider.SetAsync(thread);
    }

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

    public async Task<AiraUnifiedAIResponse?> GetAIResponseOrNull(string message, int numberOfIncludedHistoryMessages, int userId)
    {
        var textMessageHistory = (await airaUnifiedChatMessageProvider.Get()
            .WhereEquals(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageUserId), userId)
            .OrderByDescending(nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageCreatedWhen))
            .TopN(numberOfIncludedHistoryMessages)
            .GetEnumerableTypedResultAsync())
            .Select(x => new AiraUnifiedChatMessageModel
            {
                Role = x.AiraUnifiedChatMessageRole == AiraUnifiedConstants.AiraUnifiedChatRoleIdentifier ?
                    AiraUnifiedConstants.AIRequestAssistantRoleName :
                    AiraUnifiedConstants.AIRequestUserRoleName,
                Content = x.AiraUnifiedChatMessageText
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
            //AppInsights = await GenerateInsights(userId),
            ChatState = nameof(ChatStateType.ongoing)
        };

        var aiResponse = await GetAiResponse(request);

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
        }

        return aiResponse;
    }

    private async Task<AiraUnifiedAIResponse?> GetAiResponse(AiraUnifiedAIRequest request)
    {
        var httpRequest = httpContextAccessor.HttpContext?.Request;
        
        var isFake = !string.IsNullOrEmpty(httpRequest?.Query["is_fake"]);

        if (isFake)
        {
            var isInsights = !string.IsNullOrEmpty(httpRequest?.Query["is_insights"]);
            var insightsCategory = httpRequest?.Query["insights_category"];

            return new AiraUnifiedAIResponse()
            {
                Insights = new InsightsResponseModel()
                {
                    IsInsightsQuery = isInsights,
                    Category = insightsCategory
                }
            };
        }

        return await ExecuteAIRequest(request);
    }

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

    private async Task<object?> GetEmailInsights()
    {
        var emailInsights = await airaUnifiedInsightsService.GetEmailInsights();

        return new EmailInsightsDataModel()
        {
            Summary = new EmailSummaryModel()
            {
                SentCount = emailInsights.Select(i=> i.Metrics).Sum(i => i?.TotalSent ?? 0)
            },
            Campaigns = emailInsights
        };
    }

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

        return await ExecuteAIRequest(request);
    }

    private async Task<AiraUnifiedAIResponse?> ExecuteAIRequest(AiraUnifiedAIRequest request)
    {
        var jsonRequest = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        content.Headers.Add("Ocp-Apim-Subscription-Key", airaUnifiedOptions.AiraUnifiedApiSubscriptionKey);

        var response = await httpClient.PostAsync(AiraUnifiedConstants.AiraUnifiedAIEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AiraUnifiedAIResponse>(jsonResponse);
    }

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

    // private async Task<Dictionary<string, string>> GenerateInsights(int userId)
    // {
    //     var emailInsights = await airaUnifiedInsightsService.GetEmailInsights();
    //
    //     // var reusableDraftContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Reusable, userId, AiraUnifiedConstants.InsightsDraftIdentifier);
    //     // var reusableScheduledContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Reusable, userId, AiraUnifiedConstants.InsightsScheduledIdentifier);
    //     // var websiteDraftContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Website, userId, AiraUnifiedConstants.InsightsDraftIdentifier);
    //     // var websiteScheduledContentInsights = await airaUnifiedInsightsService.GetContentInsights(ContentType.Website, userId, AiraUnifiedConstants.InsightsScheduledIdentifier);
    //
    //     // var contactGroups = await contactGroupProvider.Get().GetEnumerableTypedResultAsync();
    //     // var contactGroupNames = contactGroups.Select(x => x.ContactGroupDisplayName).ToArray();
    //     //
    //     // var contactGroupInsights = airaUnifiedInsightsService.GetContactGroupInsights(contactGroupNames);
    //
    //     var separator = '_';
    //
    //     var resultInsights = new Dictionary<string, string>
    //     {
    //         // { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), reusableDraftContentInsights.Items.Count.ToString()},
    //         // { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), reusableScheduledContentInsights.Items.Count.ToString()},
    //         // { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), websiteDraftContentInsights.Items.Count.ToString()},
    //         // { string.Join(separator, AiraUnifiedConstants.InsightsContentIdentifier, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), websiteScheduledContentInsights.Items.Count.ToString()},
    //
    //         //{ string.Join(separator, AiraUnifiedConstants.InsightsAllAccountsIdentifier, AiraUnifiedConstants.InsightsCountIdentifier), contactGroupInsights.AllCount.ToString() }
    //     };
    //
    //     // foreach (var contactGroup in contactGroupInsights.Groups)
    //     // {
    //     //     resultInsights.Add(
    //     //         string.Join(separator, AiraUnifiedConstants.InsightsContactGroupIdentifier, contactGroup.Name, AiraUnifiedConstants.InsightsCountIdentifier), contactGroup.Count.ToString()
    //     //     );
    //     //
    //     //     resultInsights.Add(
    //     //         string.Join(separator, AiraUnifiedConstants.InsightsContactGroupIdentifier, contactGroup.Name, AiraUnifiedConstants.InsightsRatioToAllContactsIdentifier), ((decimal)contactGroup.Count / contactGroupInsights.AllCount).ToString()
    //     //     );
    //     // }
    //
    //     foreach (var emailInsight in emailInsights)
    //     {
    //         AddEmailInsight(nameof(EmailInsightsModel.EmailsSent), emailInsight.EmailConfigurationName, emailInsight.EmailsSent.ToString());
    //         AddEmailInsight(nameof(EmailInsightsModel.EmailsDelivered), emailInsight.EmailConfigurationName, emailInsight.EmailsDelivered.ToString());
    //         AddEmailInsight(nameof(EmailInsightsModel.EmailsOpened), emailInsight.EmailConfigurationName, emailInsight.EmailsOpened.ToString());
    //         AddEmailInsight(nameof(EmailInsightsModel.LinksClicked), emailInsight.EmailConfigurationName, emailInsight.LinksClicked.ToString());
    //         AddEmailInsight(nameof(EmailInsightsModel.UnsubscribeRate), emailInsight.EmailConfigurationName, emailInsight.UnsubscribeRate.ToString());
    //         AddEmailInsight(nameof(EmailInsightsModel.SpamReports), emailInsight.EmailConfigurationName, emailInsight.SpamReports.ToString());
    //     }
    //
    //     // AddContentItemsToInsights(reusableDraftContentInsights.Items, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier);
    //     // AddContentItemsToInsights(reusableScheduledContentInsights.Items, AiraUnifiedConstants.InsightsReusableIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier);
    //     // AddContentItemsToInsights(websiteDraftContentInsights.Items, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInDraftIdentifier);
    //     // AddContentItemsToInsights(websiteScheduledContentInsights.Items, AiraUnifiedConstants.InsightsWebsiteIdentifier, AiraUnifiedConstants.InsightsInScheduledIdentifier);
    //
    //     void AddEmailInsight(string statisticsParameter, string emailConfigurationName, string value) =>
    //         resultInsights.Add(
    //             string.Join(separator, AiraUnifiedConstants.InsightsEmailIdentifier, statisticsParameter, emailConfigurationName),
    //             value
    //         );
    //
    //     // void AddContentItemsToInsights(List<ContentItemInsightsModel> items, string contentTypePrefix, string releaseStatusPrefix)
    //     // {
    //     //     foreach (var contentItem in items)
    //     //     {
    //     //         resultInsights.Add(
    //     //             string.Join(separator, AiraUnifiedConstants.InsightsEmailIdentifier, contentTypePrefix, releaseStatusPrefix, contentItem.DisplayName),
    //     //             ""
    //     //         );
    //     //     }
    //     // }
    //
    //     return resultInsights;
    // }

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

    public void RemoveUsedPrompts(string promptGroupId)
    {
        if (int.TryParse(promptGroupId, out var id))
        {
            airaUnifiedChatPromptGroupProvider.BulkDelete(new WhereCondition($"{nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId)} = {id}"));
            airaUnifiedChatPromptProvider.BulkDelete(new WhereCondition($"{nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptChatPromptGroupId)} = {id}"));
        }
    }
}
