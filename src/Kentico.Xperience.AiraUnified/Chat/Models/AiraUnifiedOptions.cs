using Kentico.Xperience.AiraUnified.Admin;

namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// Configuration options for the Aira Unified integration.
/// </summary>
internal sealed class AiraUnifiedOptions
{
    /// <summary>
    /// The Aira Unified service subscription key.
    /// </summary>
    public string AiraUnifiedApiSubscriptionKey { get; set; } = string.Empty;


    /// <summary>
    /// The AI service endpoint URL. If not specified, falls back to <see cref="AiraUnifiedConstants.AiraUnifiedAIEndpoint"/>.
    /// </summary>
    public string? AiraUnifiedAIEndpoint { get; set; } = AiraUnifiedConstants.AiraUnifiedAIEndpoint;


    /// <summary>
    /// When true, uses mock AI client instead of real HTTP client. Useful for local development.
    /// </summary>
    public bool AiraUnifiedUseMockClient { get; set; }


    /// <summary>
    /// Whether to use mock insights service for local development.
    /// </summary>
    public bool AiraUnifiedUseMockInsights { get; set; }

    /// <summary>
    /// The Xperience By Kentico Admin ui path. Allows for overriding the admin path in case your administration uses a different domain.
    /// </summary>
    public string? XbyKAdminPath { get; set; }
}
