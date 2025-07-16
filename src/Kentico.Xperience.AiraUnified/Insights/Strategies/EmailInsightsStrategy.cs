using Kentico.Xperience.AiraUnified.Components.Insights;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kentico.Xperience.AiraUnified.Insights.Strategies;

/// <summary>
/// Strategy for loading email insights.
/// </summary>
internal sealed class EmailInsightsStrategy : InsightsStrategyBase
{
    private readonly IAiraUnifiedInsightsService insightsService;

    public EmailInsightsStrategy(
        IAiraUnifiedInsightsService insightsService,
        IConfiguration configuration,
        ILogger<EmailInsightsStrategy> logger)
        : base(configuration, logger) => this.insightsService = insightsService;

    public override string Category => "email";
    public override Type ComponentType => typeof(EmailInsightsComponent);

    protected override async Task<object> LoadRealDataAsync(InsightsContext context)
    {
        var emailInsights = await insightsService.GetEmailInsights();

        return new EmailInsightsDataModel
        {
            Summary = new EmailSummaryModel
            {
                SentCount = emailInsights.Select(i => i.Metrics).Sum(i => i?.TotalSent ?? 0)
            },
            Campaigns = emailInsights
        };
    }

    public override Task<object> LoadMockDataAsync(InsightsContext context) => Task.FromResult<object>(new EmailInsightsDataModel
    {
        Summary = new EmailSummaryModel
        {
            DraftCount = 5,
            ScheduledCount = 2,
            SentCount = 10,
            TotalCount = 17,
            AverageOpenRate = 24.5,
            AverageClickRate = 3.2
        },
        Campaigns =
            [
                new()
                {
                    Id = "1",
                    Name = "Mock Monthly Newsletter",
                    Type = "regular",
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
                        UniqueClicks = 1000M
                    }
                }
            ]
    });
}
