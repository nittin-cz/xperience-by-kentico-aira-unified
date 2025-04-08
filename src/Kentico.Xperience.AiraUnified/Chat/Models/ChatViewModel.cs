using Kentico.Xperience.AiraUnified.NavBar.Models;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// View model for the Chat page.
/// </summary>
internal sealed class ChatViewModel
{
    /// <summary>
    /// View model for the service page displayed when the ai service is unavailable.
    /// </summary>
    public ServicePageViewModel ServicePageViewModel { get; set; } = new ServicePageViewModel();

    /// <summary>
    /// Aira Unified base url.
    /// </summary>
    public string AiraUnifiedPathBase { get; set; } = string.Empty;

    /// <summary>
    /// Relative path to the AI icon displayed in the chat as the avatar of the AI.
    /// </summary>
    public string AIIconImagePath { get; set; } = string.Empty;

    /// <summary>
    /// Path of the endpoint responsible for removing used suggested prompt group.
    /// </summary>
    public string RemovePromptUrl { get; set; } = string.Empty;

    /// <summary>
    /// Path of the endpoint which retrieves the user's history.
    /// </summary>
    public string HistoryUrl { get; set; } = string.Empty;

    /// <summary>
    /// Path of the endpoint which retrieves the navigation model <see cref="NavBarViewModel"/>.
    /// </summary>
    public string NavigationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the chat page recognised by the navigation <see cref="AiraUnifiedController.Navigation(NavBarRequestModel)"/> endpoint.
    /// </summary>
    public string NavigationPageIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// The name of the thread.
    /// </summary>
    public string ThreadName { get; set; } = string.Empty;

    /// <summary>
    /// The id of the thread.
    /// </summary>
    public int ThreadId { get; set; }

    /// <summary>
    /// Relative path of the <see cref="AiraUnifiedController.PostChatMessage(Microsoft.AspNetCore.Http.IFormCollection, int)"/> endpoint.
    /// </summary>
    public string ChatUrl { get; set; } = string.Empty;

    /// <summary>
    /// Path of the logo displayed in the PWA.
    /// </summary>
    public string LogoImgRelativePath { get; set; } = string.Empty;
}
