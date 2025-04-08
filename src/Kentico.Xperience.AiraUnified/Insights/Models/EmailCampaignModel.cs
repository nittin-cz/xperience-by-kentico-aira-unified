namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents an email campaign with its metadata and performance metrics.
/// </summary>
internal sealed class EmailCampaignModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the email campaign.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the email campaign.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the email campaign (e.g., "reusable", "website").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the email campaign (e.g., "draft", "scheduled", "sent").
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the email campaign was last modified. Null if not available.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the email campaign was sent. Null if not sent yet.
    /// </summary>
    public DateTime? SentDate { get; set; }

    /// <summary>
    /// Gets or sets the performance metrics of the email campaign. Null if metrics are not available.
    /// </summary>
    public EmailMetricsModel? Metrics { get; set; }
}
