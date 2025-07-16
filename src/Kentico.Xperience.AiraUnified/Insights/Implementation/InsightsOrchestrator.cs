using Kentico.Xperience.AiraUnified.Insights.Abstractions;

using Microsoft.Extensions.Logging;

namespace Kentico.Xperience.AiraUnified.Insights.Implementation;

/// <summary>
/// Implementation of orchestrator for insights.
/// </summary>
internal sealed class InsightsOrchestrator : IInsightsOrchestrator
{
    private readonly IInsightsStrategyFactory strategyFactory;
    private readonly ILogger<InsightsOrchestrator> logger;

    public InsightsOrchestrator(
        IInsightsStrategyFactory strategyFactory,
        ILogger<InsightsOrchestrator> logger)
    {
        this.strategyFactory = strategyFactory;
        this.logger = logger;
    }

    public async Task<InsightsResult> ProcessInsightsAsync(InsightsRequest request)
    {
        try
        {
            logger.LogDebug("Processing insights request for category '{Category}', UserId: {UserId}",
                request.Category, request.UserId);

            var strategy = strategyFactory.GetStrategy(request.Category);
            if (strategy == null)
            {
                logger.LogWarning("No strategy found for category: {Category}", request.Category);
                return InsightsResult.NotFound(request.Category);
            }

            var context = request.ToContext();
            var data = await strategy.LoadDataAsync(context);

            var metadata = new InsightsMetadata
            {
                Timestamp = DateTime.UtcNow,
                Category = request.Category,
                UseMockData = strategy.UseMockData,
                Version = GetType().Assembly.GetName().Version?.ToString()
            };

            logger.LogDebug("Successfully processed insights for category '{Category}'", request.Category);

            return InsightsResult.CreateSuccess(data, metadata, strategy.ComponentType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process insights for category: {Category}", request.Category);
            return InsightsResult.CreateError($"Internal error: {ex.Message}");
        }
    }
}
