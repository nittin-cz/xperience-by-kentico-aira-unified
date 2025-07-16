using System.Text.Json;

using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Models;

using Microsoft.Extensions.Logging;

namespace Kentico.Xperience.AiraUnified.Chat.Services;

/// <summary>
/// Enhanced parser for insights data that supports dynamic strategies and backward compatibility.
/// </summary>
internal sealed class EnhancedInsightsParser
{
    private readonly IInsightsStrategyFactory strategyFactory;
    private readonly ILogger<EnhancedInsightsParser> logger;

    public EnhancedInsightsParser(
        IInsightsStrategyFactory strategyFactory,
        ILogger<EnhancedInsightsParser> logger)
    {
        this.strategyFactory = strategyFactory;
        this.logger = logger;
    }

    /// <summary>
    /// Parses system message JSON to extract insights data with full type support.
    /// Supports both legacy format (v1) and new format (v2) for backward compatibility.
    /// </summary>
    /// <param name="json">JSON string from system message</param>
    /// <returns>Tuple with category, typed data, timestamp, and component type</returns>
    public (string? category, object? data, DateTime? timestamp, Type? componentType) ParseSystemMessage(string json)
    {
        try
        {
            var rawData = JsonSerializer.Deserialize<JsonElement>(json);

            // Check for version to determine parsing strategy
            var version = 1; // Default to legacy format
            if (rawData.TryGetProperty("version", out var versionElement))
            {
                version = versionElement.GetInt32();
            }

            return version switch
            {
                2 => ParseEnhancedFormat(rawData),
                _ => ParseLegacyFormat(rawData)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse insights message: {Json}", json);
            return (null, null, null, null);
        }
    }

    /// <summary>
    /// Parses the new enhanced format (v2) with full type information
    /// </summary>
    private (string? category, object? data, DateTime? timestamp, Type? componentType) ParseEnhancedFormat(JsonElement rawData)
    {
        try
        {
            var serializationModel = JsonSerializer.Deserialize<InsightsSerializationModel>(rawData.GetRawText());
            if (serializationModel == null)
            {
                return (null, null, null, null);
            }

            var category = serializationModel.Category;
            var timestamp = serializationModel.Metadata?.Timestamp;

            // Resolve component type from stored type name
            Type? componentType = null;
            if (!string.IsNullOrEmpty(serializationModel.ComponentType))
            {
                componentType = ResolveTypeFromName(serializationModel.ComponentType);
            }

            // If we don't have component type from storage, try to get it from strategy
            if (componentType == null && !string.IsNullOrEmpty(category))
            {
                var strategy = strategyFactory.GetStrategy(category);
                componentType = strategy?.ComponentType;
            }

            // Deserialize data using type information
            object? data = null;
            if (!string.IsNullOrEmpty(serializationModel.InsightsData))
            {
                if (!string.IsNullOrEmpty(serializationModel.DataType))
                {
                    // Use stored type information for deserialization
                    var dataType = ResolveTypeFromName(serializationModel.DataType);
                    if (dataType != null)
                    {
                        data = JsonSerializer.Deserialize(serializationModel.InsightsData, dataType);
                    }
                }
                else if (!string.IsNullOrEmpty(category))
                {
                    // Fallback to strategy-based deserialization
                    data = DeserializeUsingStrategy(category, serializationModel.InsightsData);
                }
            }

            return (category, data, timestamp, componentType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse enhanced format insights data");
            return (null, null, null, null);
        }
    }

    /// <summary>
    /// Parses the legacy format (v1) for backward compatibility
    /// </summary>
    private (string? category, object? data, DateTime? timestamp, Type? componentType) ParseLegacyFormat(JsonElement rawData)
    {
        try
        {
            // Extract category
            if (!rawData.TryGetProperty("category", out var categoryElement))
            {
                return (null, null, null, null);
            }

            var category = categoryElement.GetString();

            // Extract insights data
            if (!rawData.TryGetProperty("insightsData", out var dataElement))
            {
                return (category, null, null, null);
            }

            // Extract timestamp from metadata
            DateTime? timestamp = null;
            if (rawData.TryGetProperty("metadata", out var metadataElement) &&
                metadataElement.TryGetProperty("timestamp", out var timestampElement))
            {
                timestamp = timestampElement.GetDateTime();
            }

            // Get component type from strategy
            Type? componentType = null;
            if (!string.IsNullOrEmpty(category))
            {
                var strategy = strategyFactory.GetStrategy(category);
                componentType = strategy?.ComponentType;
            }

            // Deserialize data using strategy or fallback to hardcoded types
            object? data = null;
            if (!string.IsNullOrEmpty(category))
            {
                data = DeserializeUsingStrategy(category, dataElement.GetRawText());

                // Fallback to legacy hardcoded deserialization if strategy fails
                data ??= category.ToLowerInvariant() switch
                {
                    "content" => JsonSerializer.Deserialize<ContentInsightsDataModel>(dataElement.GetRawText()),
                    "email" => JsonSerializer.Deserialize<EmailInsightsDataModel>(dataElement.GetRawText()),
                    "marketing" => JsonSerializer.Deserialize<MarketingInsightsDataModel>(dataElement.GetRawText()),
                    _ => null
                };
            }

            return (category, data, timestamp, componentType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse legacy format insights data for category: {Category}",
                rawData.TryGetProperty("category", out var cat) ? cat.GetString() : "unknown");
            return (null, null, null, null);
        }
    }

    /// <summary>
    /// Attempts to deserialize data using the insights strategy
    /// </summary>
    private object? DeserializeUsingStrategy(string category, string jsonData)
    {
        try
        {
            var strategy = strategyFactory.GetStrategy(category);
            if (strategy == null)
            {
                logger.LogWarning("No strategy found for category: {Category}", category);
                return CreateFallbackData(category, jsonData);
            }

            // Try to get the expected data type from the strategy
            var dataType = GetStrategyDataType(strategy);
            if (dataType != null)
            {
                try
                {
                    return JsonSerializer.Deserialize(jsonData, dataType);
                }
                catch (JsonException ex)
                {
                    logger.LogWarning(ex, "JSON deserialization failed for type {DataType}, category: {Category}. Using fallback.",
                        dataType.Name, category);
                    return CreateFallbackData(category, jsonData);
                }
            }

            logger.LogWarning("Could not determine data type for strategy: {StrategyType}, category: {Category}. Using fallback.",
                strategy.GetType().Name, category);
            return CreateFallbackData(category, jsonData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize data using strategy for category: {Category}. Using fallback.", category);
            return CreateFallbackData(category, jsonData);
        }
    }

    /// <summary>
    /// Creates fallback data when deserialization fails
    /// </summary>
    private object? CreateFallbackData(string category, string jsonData)
    {
        try
        {
            // Return raw JSON data with category information for debugging
            return new FallbackInsightsData
            {
                Category = category,
                RawJson = jsonData,
                Error = "Failed to deserialize insights data",
                Timestamp = DateTime.UtcNow
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to determine the data type expected by a strategy through reflection
    /// </summary>
    private Type? GetStrategyDataType(IInsightsStrategy strategy)
    {
        try
        {
            // Look for mock data method to infer the return type
            var mockDataMethod = strategy.GetType().GetMethod("LoadMockDataAsync");
            if (mockDataMethod?.ReturnType == typeof(Task<object>))
            {
                // Try to invoke mock data method to get an instance and determine its type
                var task = (Task<object>)mockDataMethod.Invoke(strategy, [new InsightsContext { Category = strategy.Category, UserId = 0 }])!;
                task.Wait();
                return task.Result?.GetType();
            }

            // Fallback: try to analyze component type parameter attributes
            var componentType = strategy.ComponentType;
            var dataParameter = componentType?.GetProperties()
                .FirstOrDefault(p => p.Name == "Data" && p.PropertyType != typeof(object));

            return dataParameter?.PropertyType;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Could not determine data type for strategy: {StrategyType}", strategy.GetType().Name);
            return null;
        }
    }

    /// <summary>
    /// Resolves a Type from its assembly-qualified name with error handling
    /// </summary>
    private Type? ResolveTypeFromName(string typeName)
    {
        try
        {
            return Type.GetType(typeName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to resolve type: {TypeName}", typeName);
            return null;
        }
    }
}
