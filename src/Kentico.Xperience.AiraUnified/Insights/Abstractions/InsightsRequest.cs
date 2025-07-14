namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
/// Request for loading insights data.
/// </summary>
public record InsightsRequest(int UserId, string Category)
{
    /// <summary>
    /// Additional parameters for the strategy
    /// </summary>
    public Dictionary<string, object> Parameters { get; init; } = new();
    
    /// <summary>
    /// Creates context from request
    /// </summary>
    public InsightsContext ToContext() => new()
    {
        UserId = UserId,
        Category = Category,
        Parameters = Parameters
    };
}