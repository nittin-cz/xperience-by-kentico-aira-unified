namespace Kentico.Xperience.AiraUnified.NavBar.Models;

/// <summary>
/// Holds navbar identifier for a specified page.
/// </summary>
internal sealed class NavBarRequestModel
{
    /// <summary>
    /// The navbar page identifier.
    /// </summary>
    public string PageIdentifier { get; set; } = string.Empty;
}
