namespace Kentico.Xperience.AiraUnified.Insights
{
    /// <summary>
    /// Represents the complete email insights data including summary statistics and individual campaign details.
    /// </summary>
    public class EmailInsightsDataModel
    {
        /// <summary>
        /// Gets or sets the summary statistics for all email campaigns.
        /// </summary>
        public EmailSummaryModel Summary { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of email campaigns with their individual metrics and details.
        /// </summary>
        public List<EmailCampaignModel> Campaigns { get; set; } = [];
    }
}
