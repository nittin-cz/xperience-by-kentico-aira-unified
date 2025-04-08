namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents a summary of contact statistics including total, active, and inactive contact counts.
/// </summary>
internal sealed class ContactsSummaryModel
{
    /// <summary>
    /// Gets or sets the total number of contacts in the system.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of active contacts. Null if the count is not available.
    /// </summary>
    public int? ActiveCount { get; set; }

    /// <summary>
    /// Gets or sets the number of inactive contacts. Null if the count is not available.
    /// </summary>
    public int? InactiveCount { get; set; }
}
