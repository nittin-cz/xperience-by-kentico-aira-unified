namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
/// Defines a strategy for loading insights data for a specific category.
/// </summary>
public interface IInsightsStrategy
{
    /// <summary>
    /// Unique category identifier (e.g., "content", "email", "users").
    /// Must be unique across all strategies.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Type of Blazor component for rendering the data.
    /// Component must have parameters: Data (object) and Timestamp (DateTime?).
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// Loads insights data for the given context.
    /// </summary>
    /// <param name="context">Context containing UserId and additional parameters</param>
    /// <returns>Object data for insights</returns>
    Task<object> LoadDataAsync(InsightsContext context);

    /// <summary>
    /// Determines whether to use mock data instead of real data.
    /// Default implementation reads from configuration.
    /// </summary>
    bool UseMockData { get; }

    /// <summary>
    /// Provides mock data for development/testing purposes.
    /// Called only when UseMockData = true.
    /// </summary>
    /// <param name="context">Context for mock data</param>
    /// <returns>Mock object data</returns>
    Task<object> LoadMockDataAsync(InsightsContext context);
}
