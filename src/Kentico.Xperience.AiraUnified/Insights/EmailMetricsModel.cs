namespace Kentico.Xperience.AiraUnified.Insights
{
    public class EmailMetricsModel
    {
        public int Sent { get; set; }
        public int Delivered { get; set; }
        public int Opened { get; set; }
        public decimal OpenRate { get; set; }
        public decimal LinksClicked { get; set; }
        public decimal UnsubscribeRate { get; set; }
        public int SpamReports { get; set; }
    }
}
