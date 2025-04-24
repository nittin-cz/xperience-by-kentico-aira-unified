using Kentico.Xperience.AiraUnified.NavBar.Models;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// View model for the Chat Thread Selector page.
/// </summary>
internal sealed class ChatThreadSelectorViewModel
{
    /// <summary>
    /// Aira Unified base url.
    /// </summary>
    public string AiraUnifiedBaseUrl { get; set; } = string.Empty;


    /// <summary>
    /// Relative path of the endpoint which retrieves the navigation model <see cref="NavBarViewModel"/>.
    /// </summary>
    public string NavigationUrl { get; set; } = string.Empty;


    /// <summary>
    /// Identifier of the chat page recognised by the navigation <see cref="AiraUnifiedController.Navigation(NavBarRequestModel)"/> endpoint.
    /// </summary>
    public string NavigationPageIdentifier { get; set; } = string.Empty;


    /// <summary>
    /// URL for retrieving the user's collection of chat threads.
    /// </summary>
    public string UserThreadCollectionUrl { get; set; } = string.Empty;


    /// <summary>
    /// URL for the main chat interface.
    /// </summary>
    public string ChatUrl { get; set; } = string.Empty;


    /// <summary>
    /// Name of the query parameter used to identify the chat thread in the URL.
    /// </summary>
    public string ChatQueryParameterName { get; set; } = string.Empty;


    /// <summary>
    /// URL for creating a new chat thread.
    /// </summary>
    public string NewChatThreadUrl { get; set; } = string.Empty;
}
