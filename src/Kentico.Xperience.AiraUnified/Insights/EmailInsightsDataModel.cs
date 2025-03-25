namespace Kentico.Xperience.AiraUnified.Insights
{
    public class EmailInsightsDataModel
    {
        public EmailSummaryModel Summary { get; set; } = new();
        public List<EmailCampaignModel> Campaigns { get; set; } = [];
    }
}
