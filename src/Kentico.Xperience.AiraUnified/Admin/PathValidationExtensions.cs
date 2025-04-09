using System.Text.RegularExpressions;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Provides extension methods for validating path strings.
/// </summary>
internal static class PathValidationExtensions
{
    /// <summary>
    /// Validates if a given path is a valid subpath.
    /// </summary>
    /// <param name="path">The path to validate.</param>
    /// <returns>True if the path is valid, false otherwise.</returns>
    public static bool IsValidSubPath(this string path) =>
        !string.IsNullOrEmpty(path)
        && path[0] == '/'
        && !path.Contains("//")
        // Ensure the path does not contain traversal sequences
        && !path.Contains("..")
        && !path.Contains("%2e%2e")
        && !path.Contains("%2E%2E")
        // Allow only alphanumeric, hyphen, underscore, and single forward slashes
        && Regex.IsMatch(path, @"^\/[a-zA-Z0-9-_\/]*$")
        // Prevent trailing slashes to maintain consistency
        && (path.Length <= 1 || !path.EndsWith('/'));
}
