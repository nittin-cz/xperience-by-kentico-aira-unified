using Kentico.Xperience.AiraUnified.Admin;

using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.AiraUnified;

/// <summary>
/// The main controller exposing the manifest.
/// </summary>
[ApiController]
[Route(AiraUnifiedConstants.RCLUrlPrefix)]
public sealed class AiraUnifiedManifestController(IAiraUnifiedConfigurationService airaUnifiedConfigurationService)
    : Controller
{
    /// <summary>
    /// Endpoint exposing the manifest file for the PWA.
    /// </summary>
    /// <returns>The manifest file for the PWA.</returns>
    [HttpGet("manifest.json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetPwaManifest()
    {
        var configuration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();

        var libraryBasePath = '/' + AiraUnifiedConstants.RCLUrlPrefix;

        var manifest = new
        {
            name = "AiraUnified",
            short_name = "AiraUnified",
            start_url = $"{configuration.AiraUnifiedConfigurationItemAiraPathBase}/{AiraUnifiedConstants.ChatRelativeUrl}",
            display = "standalone",
            background_color = "#ffffff",
            theme_color = "#ffffff",
            scope = "/",
            icons = new[]
            {
                new
                {
                    src = $"{libraryBasePath}/img/favicon/android-chrome-192x192.png",
                    sizes = "192x192",
                    type = "image/png"
                },
                new
                {
                    src = $"{libraryBasePath}/img/favicon/android-chrome-512x512.png",
                    sizes = "512x512",
                    type = "image/png"
                }
            }
        };

        return Json(manifest);
    }
}
