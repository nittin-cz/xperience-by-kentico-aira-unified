﻿using System.Text.Json.Serialization;

using Kentico.Xperience.AiraUnified.Insights;
using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// The ai response model.
/// </summary>
internal sealed class AiraUnifiedAIResponse
{
    /// <summary>
    /// The chat summary retrieved from the ai response according to the user's conversation history.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// The suggested prompts.
    /// </summary>
    [JsonPropertyName("quick_options")]
    public List<string> QuickOptions { get; set; } = [];

    /// <summary>
    /// The text response of the ai endpoint.
    /// </summary>
    [JsonPropertyName("responses")]
    public List<ResponseMessageModel> Responses { get; set; } = [];

    /// <summary>
    /// The application insights
    /// </summary>
    [JsonPropertyName("insights")]
    public InsightsResponseModel? Insights { get; set; }
}

/// <summary>
/// The ai response message model.
/// </summary>
public class ResponseMessageModel
{
    /// <summary>
    /// The type of content retrieved from ai endpoint.
    /// </summary>
    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The content of the message retrieved from ai endpoint.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
