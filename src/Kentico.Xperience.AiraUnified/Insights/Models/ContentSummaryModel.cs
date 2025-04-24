namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents summary statistics for content items including counts by status.
/// </summary>
internal sealed class ContentSummaryModel
{
    /// <summary>
    /// Gets or sets the number of content items in draft status.
    /// </summary>
    public int DraftCount { get; set; }


    /// <summary>
    /// Gets or sets the number of content items in scheduled status.
    /// </summary>
    public int ScheduledCount { get; set; }


    /// <summary>
    /// Gets or sets the number of published content items. Null if the count is not available.
    /// </summary>
    public int? PublishedCount { get; set; }


    /// <summary>
    /// Gets or sets the total number of content items. Null if the count is not available.
    /// </summary>
    public int? TotalCount { get; set; }
}
