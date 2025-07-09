using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.NavBar;
using Kentico.Xperience.AiraUnified.NavBar.Models;

using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.AiraUnified.Components;

public partial class NavigationComponent : ComponentBase
{
    [Inject] internal INavigationService NavigationService { get; set; } = null!;
    [Parameter] public string PageIdentifier { get; set; } = AiraUnifiedConstants.ChatRelativeUrl;
    [Parameter] public string BaseUrl { get; set; } = string.Empty;
    
    private NavBarViewModel? navBarModel;
    private bool isLoading = true;
    private bool hasError = false;
    private string errorMessage = string.Empty;

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
