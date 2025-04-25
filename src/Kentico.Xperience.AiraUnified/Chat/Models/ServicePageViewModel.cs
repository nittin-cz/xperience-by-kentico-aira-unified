namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// View model for the service page displayed when the AI service is unavailable.
/// </summary>
internal sealed class ServicePageViewModel
{
    /// <summary>
    /// The chat unavailable icon URL.
    /// </summary>
    public string ChatUnavailableIconUrl { get; set; } = string.Empty;


    /// <summary>
    /// The Aira Unified icon URL.
    /// </summary>
    public string ChatAiraIconUrl { get; set; } = string.Empty;


    /// <summary>
    /// The message explaining that the chat service is unavailable.
    /// </summary>
    public string ChatUnavailableMainMessage { get; set; } = string.Empty;


    /// <summary>
    /// The message explaining to try again later.
    /// </summary>
    public string ChatUnavailableTryAgainMessage { get; set; } = string.Empty;
}
