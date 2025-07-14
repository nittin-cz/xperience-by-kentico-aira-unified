namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
/// Orchestrates the process of loading insights data.
/// </summary>
public interface IInsightsOrchestrator
{
    /// <summary>
    /// Processes insights data request
    /// </summary>
    /// <param name="request">Insights request</param>
    /// <returns>Result with data or error</returns>
    Task<InsightsResult> ProcessInsightsAsync(InsightsRequest request);
}