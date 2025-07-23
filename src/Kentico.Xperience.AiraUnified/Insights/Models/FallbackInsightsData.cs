namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Fallback data model used when insights deserialization fails.
/// Provides debugging information and prevents complete failure.
/// </summary>
public sealed class FallbackInsightsData
{
    /// <summary>
    /// The category that failed to deserialize
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The raw JSON data that failed to deserialize
    /// </summary>
    public string RawJson { get; set; } = string.Empty;

    /// <summary>
    /// Error message describing the failure
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the fallback was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
