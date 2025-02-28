namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// View model for the Chat Thread Selector page.
/// </summary>
public class ChatThreadSelectorViewModel
{
    /// <summary>
    /// Aira Unified base url.
    /// </summary>
    public string AiraUnifiedBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Relative path of the endpoint which retrieves the navigation model <see cref="NavBar.NavBarViewModel"/>.
    /// </summary>
    public string NavigationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the chat page recognised by the navigation <see cref="AiraUnifiedController.Navigation(NavBar.NavBarRequestModel)"/> endpoint.
    /// </summary>
    public string NavigationPageIdentifier { get; set; } = string.Empty;
}
