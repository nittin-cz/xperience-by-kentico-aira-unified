namespace Kentico.Xperience.AiraUnified.Insights
{
    public class ContentCategoryModel
    {
        public int DraftCount { get; set; }
        public int ScheduledCount { get; set; }
        public List<ContentItemModel> Items { get; set; } = [];
    }
}
