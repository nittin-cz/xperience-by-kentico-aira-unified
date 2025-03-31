using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Insights;
using CMS.ContentEngine;

using static Kentico.Xperience.AiraUnified.Chat.Models.ChatStateType;

namespace Kentico.Xperience.AiraUnified.Chat;

/// <summary>
/// Mock implementation of HTTP client communication with AI service for local development.
/// </summary>
internal class MockAiHttpClient : IAiHttpClient
{
    /// <inheritdoc />
    public Task<AiraUnifiedAIResponse?> SendRequestAsync(AiraUnifiedAIRequest request)
    {
        // Simulate network delay
        Thread.Sleep(500);

        return request.ChatState switch
        {
            nameof(initial) => GetInitialMessageResponse(),
            nameof(returning) => GetReturningMessageResponse(),
            nameof(ongoing) => GetOngoingMessageResponse(request),
            _ => Task.FromResult<AiraUnifiedAIResponse?>(null)
        };
    }

    private static Task<AiraUnifiedAIResponse?> GetInitialMessageResponse() => Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
    {
        Responses =
        [
            new ResponseMessageModel { Content = "MOCK: Hello! I'm your AI assistant. How can I help you today?", ContentType = "text" }
        ],
        SuggestedQuestions =
        [
            "What can you help me with?",
            "How do I create content?",
            "How do I manage contacts?"
        ]
    });

    private static Task<AiraUnifiedAIResponse?> GetReturningMessageResponse() => Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
    {
        Responses =
        [
            new ResponseMessageModel { Content = "MOCK: Welcome back! I'm here to help you. What would you like to know?", ContentType = "text" }
        ],
        SuggestedQuestions =
        [
            "Continue previous conversation",
            "Start new topic",
            "Show me my content"
        ]
    });

    private static Task<AiraUnifiedAIResponse?> GetOngoingMessageResponse(AiraUnifiedAIRequest request)
    {
        // Check if the message contains any insights-related keywords
        var isInsightsQuery = request.ChatMessage.Contains("insights", StringComparison.OrdinalIgnoreCase);
        var category = isInsightsQuery ? GetInsightsCategory(request.ChatMessage) : null;

        return Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
        {
            Summary = $"This is a mock response to: {request.ChatMessage}",
            Responses =
            [
                new ResponseMessageModel { Content = $"I understand you're asking about: {request.ChatMessage}", ContentType = "text" }
            ],
            Insights = isInsightsQuery ? new InsightsResponseModel
            {
                IsInsightsQuery = true,
                Category = category,
                InsightsData = category?.ToLowerInvariant() switch
                {
                    "content" => new ContentInsightsDataModel
                    {
                        Summary = new ContentSummaryModel
                        {
                            DraftCount = 5,
                            ScheduledCount = 3,
                            PublishedCount = 12,
                            TotalCount = 20
                        },
                        ReusableContent = new ContentCategoryModel
                        {
                            DraftCount = 2,
                            ScheduledCount = 1,
                            Items = [
                                new() { Id = 1, Name = "Newsletter Template", DisplayName = "Newsletter Template", ContentTypeId = 1, ContentTypeName = "Email", VersionStatus = VersionStatus.InitialDraft, LanguageId = 1 },
                                new() { Id = 2, Name = "Product Catalog", DisplayName = "Product Catalog", ContentTypeId = 2, ContentTypeName = "Catalog", VersionStatus = VersionStatus.Published, LanguageId = 1 }
                            ]
                        },
                        WebsiteContent = new ContentCategoryModel
                        {
                            DraftCount = 3,
                            ScheduledCount = 2,
                            Items = [
                                new() { Id = 3, Name = "Home Page", DisplayName = "Home Page", ContentTypeId = 3, ContentTypeName = "Page", VersionStatus = VersionStatus.InitialDraft, LanguageId = 1 },
                                new() { Id = 4, Name = "About Us", DisplayName = "About Us", ContentTypeId = 3, ContentTypeName = "Page", VersionStatus = VersionStatus.Published, LanguageId = 1 }
                            ]
                        }
                    },
                    "marketing" => new MarketingInsightsDataModel
                    {
                        Contacts = new ContactsSummaryModel
                        {
                            TotalCount = 1000,
                            ActiveCount = 850,
                            InactiveCount = 150
                        },
                        ContactGroups = [
                            new() { Name = "Newsletter Subscribers", ContactCount = 500, RatioPercentage = 50 },
                            new() { Name = "Active Customers", ContactCount = 300, RatioPercentage = 30 },
                            new() { Name = "Prospects", ContactCount = 200, RatioPercentage = 20 }
                        ]
                    },
                    "email" => new EmailInsightsDataModel
                    {
                        Summary = new EmailSummaryModel
                        {
                            DraftCount = 2,
                            ScheduledCount = 1,
                            SentCount = 15000,
                            TotalCount = 15003,
                            AverageOpenRate = (double)45.5M,
                            AverageClickRate = (double)12.3M
                        },
                        Campaigns = [
                            new()
                            {
                                Id = "1",
                                Name = "Monthly Newsletter",
                                Type = "reusable",
                                Status = "sent",
                                LastModified = DateTime.UtcNow.AddDays(-5),
                                SentDate = DateTime.UtcNow.AddDays(-4),
                                Metrics = new EmailMetricsModel
                                {
                                    TotalSent = 10000,
                                    Delivered = 9850,
                                    Opened = 4500,
                                    OpenRate = 45.7M,
                                    Clicks = 1200M,
                                    UniqueClicks = 1000M,
                                    UnsubscribeRate = 0.5M,
                                    SpamReports = 2,
                                    SoftBounces = 50,
                                    HardBounces = 100
                                }
                            },
                            new()
                            {
                                Id = "2",
                                Name = "Product Launch",
                                Type = "website",
                                Status = "scheduled",
                                LastModified = DateTime.UtcNow.AddDays(-1),
                                Metrics = new EmailMetricsModel
                                {
                                    TotalSent = 0,
                                    Delivered = 0,
                                    Opened = 0,
                                    OpenRate = 0M,
                                    Clicks = 0M,
                                    UniqueClicks = 0M,
                                    UnsubscribeRate = 0M,
                                    SpamReports = 0,
                                    SoftBounces = 0,
                                    HardBounces = 0
                                }
                            }
                        ]
                    },
                    _ => null
                }
            } : new InsightsResponseModel { IsInsightsQuery = false }
        });
    }

    private static string? GetInsightsCategory(string message)
    {
        message = message.ToLowerInvariant();
        return message switch
        {
            var m when m.Contains("content") => "content",
            var m when m.Contains("email") => "email",
            var m when m.Contains("marketing") => "marketing",
            _ => null
        };
    }
}
