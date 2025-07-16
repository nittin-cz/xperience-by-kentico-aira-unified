using CMS.ContactManagement;
using CMS.DataEngine;

using Kentico.Xperience.AiraUnified.Components.Insights;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kentico.Xperience.AiraUnified.Insights.Strategies;

/// <summary>
/// Strategy for loading marketing insights.
/// </summary>
internal sealed class MarketingInsightsStrategy : InsightsStrategyBase
{
    private readonly IInfoProvider<ContactGroupInfo> contactGroupProvider;
    private readonly IAiraUnifiedInsightsService insightsService;

    public MarketingInsightsStrategy(
        IAiraUnifiedInsightsService insightsService,
        IInfoProvider<ContactGroupInfo> contactGroupProvider,
        IConfiguration configuration,
        ILogger<MarketingInsightsStrategy> logger)
        : base(configuration, logger)
    {
        this.insightsService = insightsService;
        this.contactGroupProvider = contactGroupProvider;
    }

    /// <inheritdoc />
    public override string Category => "marketing";


    /// <inheritdoc />
    public override Type ComponentType => typeof(MarketingInsightsComponent);


    /// <inheritdoc />
    protected override async Task<object> LoadRealDataAsync(InsightsContext context)
    {
        var groups = await contactGroupProvider.Get().GetEnumerableTypedResultAsync();
        var contactGroupNames = groups.Where(x => !x.ContactGroupIsRecipientList)
            .Select(x => x.ContactGroupDisplayName).ToArray();
        var recipientListGroupNames = groups.Where(x => x.ContactGroupIsRecipientList)
            .Select(x => x.ContactGroupDisplayName).ToArray();

        var contactGroupInsights = insightsService.GetContactGroupInsights(contactGroupNames);
        var recipientListGroupInsights = insightsService.GetContactGroupInsights(recipientListGroupNames);

        return new MarketingInsightsDataModel
        {
            Contacts = new ContactsSummaryModel { TotalCount = contactGroupInsights.AllCount },
            ContactGroups = contactGroupInsights.Groups.Select(item => new ContactGroupModel
            {
                Name = item.Name,
                ContactCount = item.Count,
                RatioPercentage = (decimal)item.Count / contactGroupInsights.AllCount * 100
            }).ToList(),
            RecipientLists = recipientListGroupInsights.Groups.Select(item => new ContactGroupModel
            {
                Name = item.Name,
                ContactCount = item.Count,
                RatioPercentage = (decimal)item.Count / contactGroupInsights.AllCount * 100
            }).ToList()
        };
    }


    /// <inheritdoc />
    public override Task<object> LoadMockDataAsync(InsightsContext context) => Task.FromResult<object>(
        new MarketingInsightsDataModel
        {
            Contacts = new ContactsSummaryModel { TotalCount = 1500, ActiveCount = 1200, InactiveCount = 300 },
            ContactGroups =
            [
                new ContactGroupModel { Name = "Newsletter Subscribers", ContactCount = 800, RatioPercentage = 53.3M },
                new ContactGroupModel { Name = "Active Customers", ContactCount = 450, RatioPercentage = 30.0M }
            ],
            RecipientLists =
            [
                new ContactGroupModel { Name = "VIP Customers", ContactCount = 150, RatioPercentage = 10.0M },
                new ContactGroupModel { Name = "Trial Users", ContactCount = 100, RatioPercentage = 6.7M }
            ]
        });
}
