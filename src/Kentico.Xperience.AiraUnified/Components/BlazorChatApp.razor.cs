using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.AiraUnified.Components;

/// <summary>
/// Blazor component for the chat application.
/// </summary>
public partial class BlazorChatApp : ComponentBase
{
    /// <summary>
    /// Gets or sets the thread ID for the chat.
    /// </summary>
    [Parameter] public int ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the thread name for the chat.
    /// </summary>
    [Parameter] public string ThreadName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID for the chat.
    /// </summary>
    [Parameter] public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the relative path to the logo image.
    /// </summary>
    [Parameter] public string LogoImgRelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL.
    /// </summary>
    [Parameter] public string BaseUrl { get; set; } = string.Empty;
}
