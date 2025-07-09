using System.Text.Json;
using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Chat.Services;

/// <summary>
/// Helper class for parsing insights data from JSON messages.
/// </summary>
internal static class InsightsParser
{
    /// <summary>
    /// Parses system message JSON to extract insights data.
    /// </summary>
    /// <param name="json">JSON string from system message</param>
    /// <returns>Tuple with category, typed data, and timestamp</returns>
    public static (string? category, object? data, DateTime? timestamp) ParseSystemMessage(string json)
    {
        try
        {
            var rawData = JsonSerializer.Deserialize<JsonElement>(json);
            
            // Extract category
            if (!rawData.TryGetProperty("category", out var categoryElement))
                return (null, null, null);
            
            var category = categoryElement.GetString();
            
            // Extract insights data
            if (!rawData.TryGetProperty("insightsData", out var dataElement))
                return (category, null, null);
            
            // Extract timestamp from metadata
            DateTime? timestamp = null;
            if (rawData.TryGetProperty("metadata", out var metadataElement) && 
                metadataElement.TryGetProperty("timestamp", out var timestampElement))
            {
                timestamp = timestampElement.GetDateTime();
            }
            
            // Deserialize data based on category
            object? typedData = category switch
            {
                "content" => JsonSerializer.Deserialize<ContentInsightsDataModel>(dataElement.GetRawText()),
                "email" => JsonSerializer.Deserialize<EmailInsightsDataModel>(dataElement.GetRawText()),
                "marketing" => JsonSerializer.Deserialize<MarketingInsightsDataModel>(dataElement.GetRawText()),
                _ => null
            };
            
            return (category, typedData, timestamp);
        }
        catch (Exception)
        {
            // Log error if needed
            return (null, null, null);
        }
    }
}
