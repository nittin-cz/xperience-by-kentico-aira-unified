using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.AiraUnified;

/// <summary>
/// Provides extension methods for HttpRequest objects.
/// </summary>  
internal static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the base URL of the request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The base URL of the request.</returns>
    public static string GetBaseUrl(this HttpRequest request)
    {
        var pathBase = request.PathBase.ToString();
        var baseUrl = $"{request.Scheme}://{request.Host}";

        return !string.IsNullOrWhiteSpace(pathBase) ? $"{baseUrl}{pathBase}" : baseUrl;
    }


    /// <summary>
    /// Gets the URL with the schema of the request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="url">The URL to get the schema of.</param>
    /// <returns>The URL with the schema of the request.</returns>
    public static string GetUrlWithSchema(this HttpRequest request, string? url) => !string.IsNullOrEmpty(url) ? $"{request.Scheme}://{url}" : request.GetBaseUrl();
}
