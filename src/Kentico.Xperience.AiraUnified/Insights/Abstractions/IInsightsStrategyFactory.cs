namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
/// Factory for retrieving insights strategies.
/// </summary>
public interface IInsightsStrategyFactory
{
    /// <summary>
    /// Gets strategy for the given category
    /// </summary>
    /// <param name="category">Insights category</param>
    /// <returns>Strategy or null if not found</returns>
    IInsightsStrategy? GetStrategy(string category);

    /// <summary>
    /// Returns all available categories
    /// </summary>
    /// <returns>List of categories</returns>
    IEnumerable<IInsightsStrategy> GetAllStrategies();
}
