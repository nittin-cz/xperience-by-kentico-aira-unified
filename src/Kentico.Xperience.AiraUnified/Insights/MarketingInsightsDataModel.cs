namespace Kentico.Xperience.AiraUnified.Insights
{
    /// <summary>
    /// Represents marketing insights data including contact statistics and contact group information.
    /// </summary>
    public class MarketingInsightsDataModel
    {
        /// <summary>
        /// Gets or sets the summary statistics for all contacts in the system.
        /// </summary>
        public ContactsSummaryModel Contacts { get; set; } = new ContactsSummaryModel();

        /// <summary>
        /// Gets or sets the list of contact groups with their respective metrics and statistics.
        /// </summary>
        public List<ContactGroupModel> ContactGroups { get; set; } = new List<ContactGroupModel>();
    }
}
