namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents marketing insights data including contact statistics and contact group information.
/// </summary>
internal sealed class MarketingInsightsDataModel
{
    /// <summary>
    /// Gets or sets the summary statistics for all contacts in the system.
    /// </summary>
    public ContactsSummaryModel Contacts { get; set; } = new();


    /// <summary>
    /// Gets or sets the list of contact groups with their respective metrics and statistics.
    /// </summary>
    public List<ContactGroupModel> ContactGroups { get; set; } = [];


    /// <summary>
    /// Gets or sets the recipient lists with their respective metrics and statistics.
    /// </summary>
    public List<ContactGroupModel> RecipientLists { get; set; } = [];
}
