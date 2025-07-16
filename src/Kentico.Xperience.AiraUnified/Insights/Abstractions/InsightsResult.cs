namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
/// Result of insights data loading operation.
/// </summary>
public class InsightsResult
{
    /// <summary>
    /// Indicates successful data loading
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Loaded data (null if Success = false)
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// Error message (null if Success = true)
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Metadata about the loaded data
    /// </summary>
    public InsightsMetadata Metadata { get; init; } = new();

    /// <summary>
    /// Component type for rendering
    /// </summary>
    public Type? ComponentType { get; init; }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static InsightsResult CreateSuccess(object data, InsightsMetadata metadata, Type componentType)
        => new() { Success = true, Data = data, Metadata = metadata, ComponentType = componentType };

    /// <summary>
    /// Creates an error result
    /// </summary>
    public static InsightsResult CreateError(string errorMessage)
        => new() { Success = false, ErrorMessage = errorMessage };

    /// <summary>
    /// Creates a result for strategy not found
    /// </summary>
    public static InsightsResult NotFound(string category)
        => new() { Success = false, ErrorMessage = $"No insights strategy found for category: {category}" };
}
