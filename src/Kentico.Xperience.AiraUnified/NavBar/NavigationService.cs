﻿using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.NavBar.Models;

namespace Kentico.Xperience.AiraUnified.NavBar;

/// <inheritdoc />
internal sealed class NavigationService(
    IAiraUnifiedConfigurationService airaUnifiedConfigurationService,
    IAiraUnifiedAssetService airaUnifiedAssetService)
    : INavigationService
{
    /// <inheritdoc />
    public async Task<NavBarViewModel> GetNavBarViewModel(string activePage, string baseUrl)
    {
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();
        var sanitizedLogoUrl = airaUnifiedAssetService.GetSanitizedLogoUrl(airaUnifiedConfiguration);

        var chatItemUrl = BuildUriOrNull(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase, AiraUnifiedConstants.ChatThreadSelectorRelativeUrl);

        var smartUploadUrl = BuildUriOrNull(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase, AiraUnifiedConstants.SmartUploadRelativeUrl);

        return new NavBarViewModel
        {
            LogoImgRelativePath = sanitizedLogoUrl,
            TitleImagePath = activePage == AiraUnifiedConstants.ChatRelativeUrl ?
             $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureNetworkGraphImgPath}"
             : $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PicturePlaceholderImgPath}",

            TitleText = activePage == AiraUnifiedConstants.ChatRelativeUrl ?
                airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraUnifiedChatTitle
                : airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraSmartUploadTitle,

            MenuMessage = Resource.NavigationMenuMessage,

            ChatItem = new MenuItemModel
            {
                Title = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraUnifiedChatTitle,
                MenuImage = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureNetworkGraphImgPath}",
                Url = chatItemUrl?.ToString()
            },
            SmartUploadItem = new MenuItemModel
            {
                Title = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraSmartUploadTitle,
                MenuImage = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PicturePlaceholderImgPath}",
                Url = smartUploadUrl?.ToString()
            }
        };
    }


    /// <inheritdoc />  
    public Uri? BuildUriOrNull(string baseUrl, string airaPathBase, params string[] relativePathParts)
        => Uri.TryCreate($"{baseUrl}{airaPathBase}/{string.Join('/', relativePathParts)}", UriKind.Absolute, out var uri) ? uri : null;
}
