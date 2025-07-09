namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents a category of content items with their status counts and the list of items.
/// </summary>
public sealed class ContentCategoryModel
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
    /// Gets or sets the list of content items in this category.
    /// </summary>
    public List<ContentItemModel> Items { get; set; } = [];
}
