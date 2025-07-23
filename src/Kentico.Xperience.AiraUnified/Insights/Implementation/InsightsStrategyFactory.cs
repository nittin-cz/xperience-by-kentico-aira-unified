using Kentico.Xperience.AiraUnified.Insights.Abstractions;

using Microsoft.Extensions.Logging;

namespace Kentico.Xperience.AiraUnified.Insights.Implementation;

/// <summary>
/// Implementation of factory for insights strategies.
/// </summary>
internal sealed class InsightsStrategyFactory : IInsightsStrategyFactory
{
    private readonly ILogger<InsightsStrategyFactory> logger;
    private readonly IEnumerable<IInsightsStrategy> strategies;
    private readonly Dictionary<string, IInsightsStrategy> strategyLookup;

    public InsightsStrategyFactory(
        IEnumerable<IInsightsStrategy> strategies,
        ILogger<InsightsStrategyFactory> logger)
    {
        this.strategies = strategies;
        this.logger = logger;
        strategyLookup = BuildStrategyLookup();
    }

    /// <inheritdoc />
    public IInsightsStrategy? GetStrategy(string category) => string.IsNullOrWhiteSpace(category)
        ? null
        : strategyLookup.GetValueOrDefault(category.ToLowerInvariant());


    /// <inheritdoc />
    public IEnumerable<IInsightsStrategy> GetAllStrategies()
        => strategyLookup.Values;


    private Dictionary<string, IInsightsStrategy> BuildStrategyLookup()
    {
        var lookup = new Dictionary<string, IInsightsStrategy>();

        foreach (var strategy in strategies)
        {
            var category = strategy.Category.ToLowerInvariant();

            if (lookup.TryGetValue(category, out var value))
            {
                logger.LogError(
                    "Duplicate insights strategy found for category '{Category}'. " +
                    "Existing: {ExistingType}, New: {NewType}",
                    category, value.GetType().Name, strategy.GetType().Name);
                continue;
            }

            lookup[category] = strategy;
            logger.LogDebug("Registered insights strategy '{StrategyType}' for category '{Category}'",
                strategy.GetType().Name, category);
        }

        return lookup;
    }
}
