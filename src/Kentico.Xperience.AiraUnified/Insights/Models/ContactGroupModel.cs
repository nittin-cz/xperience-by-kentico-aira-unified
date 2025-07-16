namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents a contact group with its metrics and statistics.
/// </summary>
public sealed class ContactGroupModel
{
    /// <summary>
    /// Gets or sets the display name of the contact group.
    /// </summary>
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets the total number of contacts in the group.
    /// </summary>
    public int ContactCount { get; set; }


    /// <summary>
    /// Gets or sets the percentage ratio of contacts in this group compared to the total number of contacts.
    /// </summary>
    public decimal RatioPercentage { get; set; }
}
