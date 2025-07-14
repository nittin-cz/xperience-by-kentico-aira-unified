using CMS.ContentEngine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Models;
using Kentico.Xperience.AiraUnified.Components.Insights;
using Kentico.Xperience.AiraUnified.Admin;

namespace Kentico.Xperience.AiraUnified.Insights.Strategies;

/// <summary>
/// Strategy for loading content insights.
/// </summary>
internal sealed class ContentInsightsStrategy : InsightsStrategyBase
{
    private readonly IAiraUnifiedInsightsService insightsService;
    
    public ContentInsightsStrategy(
        IAiraUnifiedInsightsService insightsService,
        IConfiguration configuration,
        ILogger<ContentInsightsStrategy> logger) 
        : base(configuration, logger) =>
        this.insightsService = insightsService;

    public override string Category => "content";
    public override Type ComponentType => typeof(ContentInsightsComponent);

    protected override async Task<object> LoadRealDataAsync(InsightsContext context)
    {
        var reusableDraftContent = await insightsService.GetContentInsights(
            ContentType.Reusable, context.UserId, AiraUnifiedConstants.InsightsDraftIdentifier);
        var reusableScheduledContent = await insightsService.GetContentInsights(
            ContentType.Reusable, context.UserId, AiraUnifiedConstants.InsightsScheduledIdentifier);
        var websiteDraftContent = await insightsService.GetContentInsights(
            ContentType.Website, context.UserId, AiraUnifiedConstants.InsightsDraftIdentifier);
        var websiteScheduledContent = await insightsService.GetContentInsights(
            ContentType.Website, context.UserId, AiraUnifiedConstants.InsightsScheduledIdentifier);

        return new ContentInsightsDataModel
        {
            Summary = new ContentSummaryModel
            {
                DraftCount = reusableDraftContent.Count + websiteDraftContent.Count,
                ScheduledCount = reusableScheduledContent.Count + websiteScheduledContent.Count
            },
            ReusableContent = new ContentCategoryModel
            {
                DraftCount = reusableDraftContent.Count,
                ScheduledCount = reusableScheduledContent.Count,
                Items = reusableDraftContent.Concat(reusableScheduledContent).ToList()
            },
            WebsiteContent = new ContentCategoryModel
            {
                DraftCount = websiteDraftContent.Count,
                ScheduledCount = websiteScheduledContent.Count,
                Items = websiteScheduledContent.Concat(websiteDraftContent).ToList()
            }
        };
    }
    
    public override Task<object> LoadMockDataAsync(InsightsContext context) =>
        Task.FromResult<object>(new ContentInsightsDataModel
        {
            Summary = new ContentSummaryModel
            {
                DraftCount = 15,
                ScheduledCount = 5
            },
            ReusableContent = new ContentCategoryModel
            {
                DraftCount = 8,
                ScheduledCount = 3,
                Items =
                [
                    new ContentItemModel
                    {
                        Id = 1,
                        DisplayName = "Mock Newsletter Template",
                        ContentTypeName = "Email",
                        VersionStatus = VersionStatus.Draft
                    },
                    new ContentItemModel
                    {
                        Id = 2,
                        DisplayName = "Mock Product Catalog",
                        ContentTypeName = "Catalog",
                        VersionStatus = VersionStatus.Published
                    }
                ]
            },
            WebsiteContent = new ContentCategoryModel
            {
                DraftCount = 7,
                ScheduledCount = 2,
                Items =
                [
                    new ContentItemModel
                    {
                        Id = 3,
                        DisplayName = "Mock Home Page",
                        ContentTypeName = "Page",
                        VersionStatus = VersionStatus.Draft
                    },
                    new ContentItemModel
                    {
                        Id = 4,
                        DisplayName = "Mock About Us",
                        ContentTypeName = "Page",
                        VersionStatus = VersionStatus.Published
                    }
                ]
            }
        });
}
