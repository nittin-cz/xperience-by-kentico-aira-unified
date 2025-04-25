using Kentico.Xperience.AiraUnified.NavBar.Models;

namespace Kentico.Xperience.AiraUnified.AssetUploader;

/// <summary>
/// View model for the Smart upload page.
/// </summary>
internal sealed class AssetsViewModel
{
    /// <summary>
    /// Path base for the Smart upload page.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;


    /// <summary>
    /// Url to access the allowed file extensions.
    /// </summary>
    public string AllowedFileExtensionsUrl { get; set; } = string.Empty;


    /// <summary>
    /// The button triggering the file selection.
    /// </summary>
    public string SelectFilesButton { get; set; } = string.Empty;


    /// <summary>
    /// The message displayed on successful asset upload.
    /// </summary>
    public string FilesUploadedMessage { get; set; } = string.Empty;


    /// <summary>
    /// Path of the endpoint which retrieves the navigation model <see cref="NavBarViewModel"/>.
    /// </summary>
    public string NavigationUrl { get; set; } = string.Empty;


    /// <summary>
    /// Identifier of the smart upload page recognised by the navigation <see cref="AiraUnifiedController.Navigation(NavBarRequestModel)"/> endpoint.
    /// </summary>
    public string NavigationPageIdentifier { get; set; } = string.Empty;


    /// <summary>
    /// Smart upload upload URL.
    /// </summary>
    public string UploadUrl { get; set; } = string.Empty;
}
