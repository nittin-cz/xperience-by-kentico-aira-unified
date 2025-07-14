namespace Kentico.Xperience.AiraUnified.Insights.Abstractions;

/// <summary>
/// Metadata information about insights data.
/// </summary>
public class InsightsMetadata
{
    /// <summary>
    /// UTC timestamp when insights data was generated
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Category of the insights
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Whether mock data was used
    /// </summary>
    public bool UseMockData { get; set; }
    
    /// <summary>
    /// Version of the system that generated the data
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Additional metadata properties
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();
}