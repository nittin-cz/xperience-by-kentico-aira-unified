using System.Text.Json.Serialization;

namespace Kentico.Xperience.AiraUnified.Insights
{
    /// <summary>
    /// The AI insights response model
    /// </summary>
    public class InsightsResponseModel
    {
        /// <summary>
        /// Indicates whether this is an insights query.
        /// </summary>
        [JsonPropertyName("is_insights_query")]
        public bool IsInsightsQuery { get; set; }

        /// <summary>
        /// The category of the query or insight.
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Description of the insights query.
        /// </summary>
        [JsonPropertyName("query_description")]
        public string? QueryDescription { get; set; }

        /// <summary>
        /// The data containing the insights.
        /// </summary>
        public object? InsightsData { get; set; }

        /// <summary>
        /// Metadata associated with the insights.
        /// </summary>
        public InsightsMetadataModel? Metadata { get; set; }
    }
}
