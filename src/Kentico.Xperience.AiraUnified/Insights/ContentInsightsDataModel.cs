namespace Kentico.Xperience.AiraUnified.Insights
{
    public class ContentInsightsDataModel
    {
        public ContentSummaryModel Summary { get; set; } = new();
        public ContentCategoryModel WebsiteContent { get; set; } = new();
        public ContentCategoryModel ReusableContent { get; set; } = new();
    }
}
