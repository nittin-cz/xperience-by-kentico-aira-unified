using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
///     Base class for insights strategies with mock data support.
/// </summary>
public abstract class InsightsStrategyBase : IInsightsStrategy
{
    protected readonly IConfiguration Configuration;
    protected readonly ILogger Logger;

    protected InsightsStrategyBase(IConfiguration configuration, ILogger logger)
    {
        Configuration = configuration;
        Logger = logger;
    }

    public abstract string Category { get; }
    public abstract Type ComponentType { get; }

    /// <summary>
    ///     Determines mock data usage based on configuration.
    ///     Checks category-specific setting first, then global setting.
    /// </summary>
    public virtual bool UseMockData =>
        Configuration.GetValue<bool>($"AiraUnified:MockInsights:{Category}") ||
        Configuration.GetValue("AiraUnified:MockInsights:All", false);

    /// <summary>
    ///     Main method for loading data - decides between real and mock data.
    /// </summary>
    public async Task<object> LoadDataAsync(InsightsContext context)
    {
        if (UseMockData)
        {
            Logger.LogDebug("Using mock data for category '{Category}'", Category);
            return await LoadMockDataAsync(context);
        }

        Logger.LogDebug("Loading real data for category '{Category}'", Category);

        return await LoadRealDataAsync(context);
    }

    /// <summary>
    ///     Loads mock data - must be implemented in derived class.
    /// </summary>
    public abstract Task<object> LoadMockDataAsync(InsightsContext context);

    /// <summary>
    ///     Loads real data - must be implemented in derived class.
    /// </summary>
    protected abstract Task<object> LoadRealDataAsync(InsightsContext context);
}
