namespace Kentico.Xperience.AiraUnified.Insights
{
    public class EmailSummaryModel
    {
        public int DraftCount { get; set; }
        public int ScheduledCount { get; set; }
        public int SentCount { get; set; }
        public int TotalCount { get; set; }
        public double AverageOpenRate { get; set; }
        public double AverageClickRate { get; set; }
    }
}
