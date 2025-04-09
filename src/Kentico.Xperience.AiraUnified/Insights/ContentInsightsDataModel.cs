namespace Kentico.Xperience.AiraUnified.Insights
{
    /// <summary>
    /// Represents the complete content insights data including summary statistics and categorized content.
    /// </summary>
    public class ContentInsightsDataModel
    {
        /// <summary>
        /// Gets or sets the summary statistics for all content.
        /// </summary>
        public ContentSummaryModel Summary { get; set; } = new();

        /// <summary>
        /// Gets or sets the website content category with its statistics and items.
        /// </summary>
        public ContentCategoryModel WebsiteContent { get; set; } = new();

        /// <summary>
        /// Gets or sets the reusable content category with its statistics and items.
        /// </summary>
        public ContentCategoryModel ReusableContent { get; set; } = new();
    }
}
