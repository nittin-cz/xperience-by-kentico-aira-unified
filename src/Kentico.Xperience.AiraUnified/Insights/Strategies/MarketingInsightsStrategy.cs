using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CMS.ContactManagement;
using CMS.DataEngine;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Models;
using Kentico.Xperience.AiraUnified.Components.Insights;

namespace Kentico.Xperience.AiraUnified.Insights.Strategies;

/// <summary>
/// Strategy for loading marketing insights.
/// </summary>
internal sealed class MarketingInsightsStrategy : InsightsStrategyBase
{
    private readonly IAiraUnifiedInsightsService insightsService;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupProvider;
    
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
    
    public override string Category => "marketing";
    public override Type ComponentType => typeof(MarketingInsightsComponent);
    
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
    
    public override Task<object> LoadMockDataAsync(InsightsContext context)
    {
        return Task.FromResult<object>(new MarketingInsightsDataModel
        {
            Contacts = new ContactsSummaryModel 
            { 
                TotalCount = 1500,
                ActiveCount = 1200,
                InactiveCount = 300
            },
            ContactGroups = new List<ContactGroupModel>
            {
                new() { Name = "Newsletter Subscribers", ContactCount = 800, RatioPercentage = 53.3M },
                new() { Name = "Active Customers", ContactCount = 450, RatioPercentage = 30.0M }
            },
            RecipientLists = new List<ContactGroupModel>
            {
                new() { Name = "VIP Customers", ContactCount = 150, RatioPercentage = 10.0M },
                new() { Name = "Trial Users", ContactCount = 100, RatioPercentage = 6.7M }
            }
        });
    }
}