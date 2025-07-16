using System.Text.Json.Serialization;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// Dynamic insights category
/// </summary>
internal sealed class AiraInsightCategory
{
    /// <summary>
    /// Unique identifier for the category (e.g., 'marketing', 'content', 'email').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;


    /// <summary>
    /// Display name for the category.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// Brief description of what this category covers.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;


    /// <summary>
    /// List of follow-up questions for this category.
    /// </summary>
    [JsonPropertyName("follow_up_questions")]
    public List<string> FollowUpQuestions { get; set; } = new();
}
