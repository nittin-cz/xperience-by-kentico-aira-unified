namespace Kentico.Xperience.AiraUnified.Insights
{
    public class EmailCampaignModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "website" nebo "reusable"
        public string Status { get; set; } = string.Empty; // "draft", "scheduled", "sent"
        public DateTime? LastModified { get; set; }
        public DateTime? SentDate { get; set; }
        public EmailMetricsModel? Metrics { get; set; }
    }
}
