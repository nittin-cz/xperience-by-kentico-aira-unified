namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Represents performance metrics for an email campaign including delivery, engagement, and bounce statistics.
/// </summary>
internal sealed class EmailMetricsModel
{
    /// <summary>
    /// Gets or sets the total number of emails sent in the campaign.
    /// </summary>
    public int TotalSent { get; set; }

    /// <summary>
    /// Gets or sets the number of emails successfully delivered to recipients.
    /// </summary>
    public int Delivered { get; set; }

    /// <summary>
    /// Gets or sets the number of times the email was opened by recipients.
    /// </summary>
    public int Opened { get; set; }

    /// <summary>
    /// Gets or sets the percentage of delivered emails that were opened.
    /// </summary>
    public decimal OpenRate { get; set; }

    /// <summary>
    /// Gets or sets the total number of clicks on links in the email.
    /// </summary>
    public decimal Clicks { get; set; }

    /// <summary>
    /// Gets or sets the number of unique recipients who clicked on links in the email.
    /// </summary>
    public decimal UniqueClicks { get; set; }

    /// <summary>
    /// Gets or sets the percentage of recipients who unsubscribed from the email list.
    /// </summary>
    public decimal UnsubscribeRate { get; set; }

    /// <summary>
    /// Gets or sets the number of times the email was reported as spam.
    /// </summary>
    public int SpamReports { get; set; }

    /// <summary>
    /// Gets or sets the number of temporary delivery failures (soft bounces). Null if not available.
    /// </summary>
    public int? SoftBounces { get; set; }

    /// <summary>
    /// Gets or sets the number of permanent delivery failures (hard bounces). Null if not available.
    /// </summary>
    public int? HardBounces { get; set; }
}
