namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// View model for Blazor chat page
/// </summary>
public sealed class BlazorChatViewModel
{
    /// <summary>
    /// Thread ID
    /// </summary>
    public int ThreadId { get; set; }

    /// <summary>
    /// Thread name
    /// </summary>
    public string ThreadName { get; set; } = string.Empty;

    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Logo image relative path
    /// </summary>
    public string LogoImgRelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Base URL
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
}
