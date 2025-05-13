using System.Text.Json.Serialization;

using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// The AI response model.
/// </summary>
internal sealed class AiraUnifiedAIResponse
{
    /// <summary>
    /// The chat summary retrieved from the AI response according to the user's conversation history.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;


    /// <summary>
    /// The suggested prompts.
    /// </summary>
    [JsonPropertyName("quick_options")]
    public List<string> QuickOptions { get; set; } = [];


    /// <summary>
    /// The text response of the AI endpoint.
    /// </summary>
    [JsonPropertyName("responses")]
    public List<ResponseMessageModel> Responses { get; set; } = [];


    /// <summary>
    /// The application insights
    /// </summary>
    [JsonPropertyName("insights")]
    public InsightsResponseModel? Insights { get; set; }
}
