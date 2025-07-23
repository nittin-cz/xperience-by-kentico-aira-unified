namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents metadata information about the insights data including timestamp, instance details, and data freshness.
/// </summary>
public sealed class InsightsMetadataModel
{
    /// <summary>
    /// Gets or sets the UTC timestamp when the insights data was generated.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;


    /// <summary>
    /// Gets or sets the name of the instance that generated the insights data. Null if not available.
    /// </summary>
    public string? InstanceName { get; set; }


    /// <summary>
    /// Gets or sets the version of the system that generated the insights data. Null if not available.
    /// </summary>
    public string? Version { get; set; }


    /// <summary>
    /// Gets or sets information about the freshness of the insights data. Null if not available.
    /// </summary>
    public string? DataFreshness { get; set; }
}
