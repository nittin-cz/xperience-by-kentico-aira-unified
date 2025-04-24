using System.Text.Json.Serialization;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

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
