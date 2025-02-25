namespace Kentico.Xperience.AiraUnified.Authentication;

public class SignInViewModel
{
    /// <summary>
    /// Url of the chat page relative to the Aira unified base url.
    /// </summary>
    public string ChatUrl { get; set; } = string.Empty;

    /// <summary>
    /// Aira unified base url.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;
}
