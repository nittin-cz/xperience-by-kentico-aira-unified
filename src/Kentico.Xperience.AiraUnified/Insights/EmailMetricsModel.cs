namespace Kentico.Xperience.AiraUnified.Insights
{
    public class EmailMetricsModel
    {
        public int Sent { get; set; }
        public int Delivered { get; set; }
        public int Opened { get; set; }
        public int Clicked { get; set; }
        public double OpenRate { get; set; }
        public double ClickRate { get; set; }
        public double UnsubscribeRate { get; set; }
        public int SpamReports { get; set; }
    }
}
