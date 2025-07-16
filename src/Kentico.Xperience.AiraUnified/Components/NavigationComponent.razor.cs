using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.NavBar;
using Kentico.Xperience.AiraUnified.NavBar.Models;

using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.AiraUnified.Components;

/// <summary>
/// Blazor component for navigation functionality.
/// </summary>
public partial class NavigationComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the navigation service.
    /// </summary>
    [Inject] internal INavigationService NavigationService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the page identifier for navigation.
    /// </summary>
    [Parameter] public string PageIdentifier { get; set; } = AiraUnifiedConstants.ChatRelativeUrl;

    /// <summary>
    /// Gets or sets the base URL.
    /// </summary>
    [Parameter] public string BaseUrl { get; set; } = string.Empty;

    private NavBarViewModel? navBarModel;
    private bool isLoading = true;
    private bool hasError = false;
    private string errorMessage = string.Empty;

    /// <summary>
    /// Initializes the component and loads navigation data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            navBarModel = await NavigationService.GetNavBarViewModel(PageIdentifier, BaseUrl);
            isLoading = false;
        }
        catch (Exception ex)
        {
            hasError = true;
            errorMessage = ex.Message;
            isLoading = false;
        }

        StateHasChanged();
    }
}
