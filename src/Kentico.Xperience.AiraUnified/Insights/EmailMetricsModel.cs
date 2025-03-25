namespace Kentico.Xperience.AiraUnified.Insights
{
    public class EmailMetricsModel
    {
        public int TotalSent { get; set; }
        public int Delivered { get; set; }
        public int Opened { get; set; }
        public decimal OpenRate { get; set; }
        public decimal Clicks { get; set; }
        public decimal UniqueClicks { get; set; }
        public decimal UnsubscribeRate { get; set; }
        public int SpamReports { get; set; }
        public int? SoftBounces { get; set; }
        public int? HardBounces { get; set; }
    }
}
