using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Aira;

internal static class HttpRequestExtensions
{
    public static string GetBaseUrl(this HttpRequest request)
    {
        string pathBase = request.PathBase.ToString();
        string baseUrl = $"{request.Scheme}://{request.Host}";

        return !string.IsNullOrWhiteSpace(pathBase) ? $"{baseUrl}{pathBase}" : baseUrl;
    }

    public static bool IsPreview(this HttpRequest request) => request.PathBase.HasValue && request.PathBase.Value.EndsWith("/preview");

    public static string GetUrlWithSchema(this HttpRequest request, string? url) => !string.IsNullOrEmpty(url) ? $"{request.Scheme}://{url}" : request.GetBaseUrl();
}
