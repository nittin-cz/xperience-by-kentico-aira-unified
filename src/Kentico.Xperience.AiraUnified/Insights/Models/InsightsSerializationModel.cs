using System.Text.Json.Serialization;

namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Enhanced model for serializing insights data with type information.
/// Enables proper deserialization of dynamic insights strategies.
/// </summary>
public sealed class InsightsSerializationModel
{
    /// <summary>
    /// Version of the serialization format for backward compatibility
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 2;

    /// <summary>
    /// Indicates if this is an insights query
    /// </summary>
    [JsonPropertyName("isInsightsQuery")]
    public bool IsInsightsQuery { get; set; }

    /// <summary>
    /// Category of the insights (e.g., "content", "email", "marketing", "users")
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>
    /// Description of the query
    /// </summary>
    [JsonPropertyName("queryDescription")]
    public string? QueryDescription { get; set; }

    /// <summary>
    /// Assembly-qualified name of the data type for proper deserialization
    /// </summary>
    [JsonPropertyName("dataType")]
    public string? DataType { get; set; }

    /// <summary>
    /// Assembly-qualified name of the component type for rendering
    /// </summary>
    [JsonPropertyName("componentType")]
    public string? ComponentType { get; set; }

    /// <summary>
    /// Serialized insights data as JSON
    /// </summary>
    [JsonPropertyName("insightsData")]
    public string? InsightsData { get; set; }

    /// <summary>
    /// Metadata about the insights
    /// </summary>
    [JsonPropertyName("metadata")]
    public InsightsMetadataModel? Metadata { get; set; }

    /// <summary>
    /// Creates an enhanced serialization model from insights response
    /// </summary>
    public static InsightsSerializationModel FromInsightsResponse(InsightsResponseModel insights, Type? dataType = null, Type? componentType = null) => new()
    {
        IsInsightsQuery = insights.IsInsightsQuery,
        Category = insights.Category,
        QueryDescription = insights.QueryDescription,
        DataType = dataType?.AssemblyQualifiedName,
        ComponentType = componentType?.AssemblyQualifiedName,
        InsightsData = insights.InsightsData != null
                ? System.Text.Json.JsonSerializer.Serialize(insights.InsightsData)
                : null,
        Metadata = insights.Metadata
    };

    /// <summary>
    /// Converts back to standard insights response model
    /// </summary>
    public InsightsResponseModel ToInsightsResponse(object? deserializedData = null) =>
        new()
        {
            IsInsightsQuery = IsInsightsQuery,
            Category = Category,
            QueryDescription = QueryDescription,
            InsightsData = deserializedData,
            Metadata = Metadata
        };
}
