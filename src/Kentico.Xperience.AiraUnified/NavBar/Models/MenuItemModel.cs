﻿namespace Kentico.Xperience.AiraUnified.NavBar.Models;

/// <summary>
/// Model for Navigation menu item.
/// </summary>
internal sealed class MenuItemModel
{
    /// <summary>
    /// The title displayed on top of the PWA page when selected in the Menu.
    /// </summary>
    public string Title { get; set; } = string.Empty;


    /// <summary>
    /// The image displayed on top of the PWA page when selected in the Menu.
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;


    /// <summary>
    /// The image displayed next to this menu item in the Navigation.
    /// </summary>
    public string MenuImage { get; set; } = string.Empty;


    /// <summary>
    /// URL of the page represented by this menu item.
    /// </summary>
    public string? Url { get; set; } = string.Empty;
}
