using Microsoft.Extensions.Logging;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;

namespace Kentico.Xperience.AiraUnified.Insights.Implementation;

/// <summary>
/// Implementation of factory for insights strategies.
/// </summary>
internal sealed class InsightsStrategyFactory : IInsightsStrategyFactory
{
    private readonly IEnumerable<IInsightsStrategy> strategies;
    private readonly ILogger<InsightsStrategyFactory> logger;
    private readonly Dictionary<string, IInsightsStrategy> strategyLookup;
    
    public InsightsStrategyFactory(
        IEnumerable<IInsightsStrategy> strategies, 
        ILogger<InsightsStrategyFactory> logger)
    {
        this.strategies = strategies;
        this.logger = logger;
        this.strategyLookup = BuildStrategyLookup();
    }
    
    public IInsightsStrategy? GetStrategy(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return null;
            
        return strategyLookup.TryGetValue(category.ToLowerInvariant(), out var strategy) 
            ? strategy 
            : null;
    }
    
    public IEnumerable<string> GetAvailableCategories()
        => strategyLookup.Keys;
    
    public bool HasStrategy(string category)
        => !string.IsNullOrWhiteSpace(category) && 
           strategyLookup.ContainsKey(category.ToLowerInvariant());
    
    private Dictionary<string, IInsightsStrategy> BuildStrategyLookup()
    {
        var lookup = new Dictionary<string, IInsightsStrategy>();
        
        foreach (var strategy in strategies)
        {
            var category = strategy.Category.ToLowerInvariant();
            
            if (lookup.ContainsKey(category))
            {
                logger.LogError(
                    "Duplicate insights strategy found for category '{Category}'. " +
                    "Existing: {ExistingType}, New: {NewType}", 
                    category, lookup[category].GetType().Name, strategy.GetType().Name);
                continue;
            }
            
            lookup[category] = strategy;
            logger.LogDebug("Registered insights strategy '{StrategyType}' for category '{Category}'", 
                strategy.GetType().Name, category);
        }
        
        return lookup;
    }
}