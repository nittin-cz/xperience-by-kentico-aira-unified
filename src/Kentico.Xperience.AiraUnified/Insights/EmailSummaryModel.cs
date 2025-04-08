namespace Kentico.Xperience.AiraUnified.Insights
{
    /// <summary>
    /// Represents summary statistics for email campaigns including counts by status and average engagement metrics.
    /// </summary>
    public class EmailSummaryModel
    {
        /// <summary>
        /// Gets or sets the number of email campaigns in draft status.
        /// </summary>
        public int DraftCount { get; set; }

        /// <summary>
        /// Gets or sets the number of email campaigns in scheduled status.
        /// </summary>
        public int ScheduledCount { get; set; }

        /// <summary>
        /// Gets or sets the number of email campaigns that have been sent.
        /// </summary>
        public int SentCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of email campaigns.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the average open rate across all sent email campaigns.
        /// </summary>
        public double AverageOpenRate { get; set; }

        /// <summary>
        /// Gets or sets the average click rate across all sent email campaigns.
        /// </summary>
        public double AverageClickRate { get; set; }
    }
}
