namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
/// Context for loading insights data.
/// </summary>
public class InsightsContext
{
    /// <summary>
    /// ID of the user for whom data is being loaded
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Insights category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Additional parameters for the strategy
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = [];

    /// <summary>
    /// Adds a parameter to the context
    /// </summary>
    public InsightsContext WithParameter(string key, object value)
    {
        Parameters[key] = value;
        return this;
    }

    /// <summary>
    /// Gets a typed parameter from the context
    /// </summary>
    public T? GetParameter<T>(string key) => Parameters.TryGetValue(key, out var value) && value is T typedValue
            ? typedValue
            : default;
}
