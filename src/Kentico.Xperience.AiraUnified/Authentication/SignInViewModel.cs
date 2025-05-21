namespace Kentico.Xperience.AiraUnified.Authentication;

/// <summary>
/// View model for the Sign in operation.
/// </summary>
internal sealed class SignInViewModel
{
    /// <summary>
    /// URL of the chat page relative to the Aira Unified base URL.
    /// </summary>
    public string ChatUrl { get; set; } = string.Empty;

    /// <summary>
    /// Aira Unified base URL.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;

    /// <summary>
    /// Path of the logo displayed in the PWA.
    /// </summary>
    public string LogoImageRelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Error message displayed if the user misses a permission.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Xperience by Kentico admin path.
    /// </summary>
    public string AdminPath { get; set; } = string.Empty;
}
