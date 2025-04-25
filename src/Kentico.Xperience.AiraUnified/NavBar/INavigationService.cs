using Kentico.Xperience.AiraUnified.NavBar.Models;

namespace Kentico.Xperience.AiraUnified.NavBar;

/// <summary>
/// Service managing the Navigation.
/// </summary>
internal interface INavigationService
{
    /// <summary>
    /// Generates the Navigation View Model.
    /// </summary>
    /// <param name="activePage">The page active in the PWA.</param>
    /// <param name="baseUrl">Application base URL.</param>
    /// <returns>A <see cref="NavBarViewModel"/> containing the navigation view model.</returns>
    Task<NavBarViewModel> GetNavBarViewModel(string activePage, string baseUrl);


    /// <summary>
    /// Safely builds an absolute Aira Unified URI by combining a base URL with a relative path validating the URI can be created from the Aira Unified path base.
    /// </summary>
    /// <param name="baseUrl">Application base URL.</param>
    /// <param name="airaPathBase">Aira Unified path base specified in the admin UI.</param>
    /// <param name="relativePathParts">Another relative path parts. (Not starting with "/")</param>
    /// <returns>The resulting <see cref="Uri"/> or null if the uri can not be created.</returns>
    Uri? BuildUriOrNull(string baseUrl, string airaPathBase, params string[] relativePathParts);
}
