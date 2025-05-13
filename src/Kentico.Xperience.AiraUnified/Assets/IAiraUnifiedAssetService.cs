using Kentico.Content.Web.Mvc;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;

using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.AiraUnified.Assets;

/// <summary>
/// Service responsible for handling files and accessing content item asset management.
/// </summary>
internal interface IAiraUnifiedAssetService
{
    /// <summary>
    /// Creates content item assets for a content type configured for mass asset upload.
    /// </summary>
    /// <param name="files">Files which will be added to the corresponding content item asset.</param>
    /// <param name="userId">The admin application user id.</param>
    Task<bool> HandleFileUpload(IFormFileCollection files, int userId);


    /// <summary>
    /// Checks whether the specified user has a role which contains the specified permission to the Aira Unified. 
    /// </summary>
    /// <param name="permission">System permission name.</param>
    /// <param name="userId">The admin application user id.</param>
    /// <returns>A task indicating whether the user has the specified permission.</returns>
    Task<bool> DoesUserHaveAiraUnifiedPermission(string permission, int userId);


    /// <summary>
    /// Retrieves the file extensions which are configured for mass asset upload.
    /// </summary>
    /// <returns>Allowed file extensions.</returns>
    Task<string> GetAllowedFileExtensions();


    /// <summary>
    /// Retrieves URL for a file displayed in the PWA.
    /// </summary>
    /// <param name="identifier">Identifier of the file.</param>
    /// <returns><see cref="IMediaFileUrl"/>The file reference.</returns>
    IMediaFileUrl? GetMediaFileUrl(string identifier);


    /// <summary>
    /// Retrieves URL of an image with configured URL or logs warning if it doesn't exist.
    /// </summary>
    /// <param name="configuredUrl">The URL configured for the image, can be null.</param>
    /// <param name="defaultUrl">The fallback URL to use if configuredUrl is null or invalid.</param>
    /// <param name="imagePurpose">The purpose or context for which the image is being used.</param>
    string GetSanitizedImageUrl(string? configuredUrl, string defaultUrl, string imagePurpose);


    /// <summary>
    /// Retrieves URL of the Aira Unified logo specified in the configuration.
    /// </summary>
    /// <param name="configuration">The configuration item containing the logo URL settings.</param>
    /// <returns>The sanitized logo URL from the configuration.</returns>
    string GetSanitizedLogoUrl(AiraUnifiedConfigurationItemInfo configuration);


    /// <summary>
    /// Retrieves URL of the Aira Unified logo specified in the configuration.
    /// </summary>
    /// <returns>The sanitized logo URL from the configuration.</returns>
    Task<string> GetSanitizedLogoUrl();
}
