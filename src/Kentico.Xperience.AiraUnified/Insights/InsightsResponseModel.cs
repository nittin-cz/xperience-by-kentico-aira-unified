namespace Kentico.Xperience.AiraUnified.Insights
{
    public class InsightsResponseModel
    {
        public bool IsInsightsQuery { get; set; }
        public string? Category { get; set; }
        public string? QueryDescription { get; set; }
        public object? InsightsData { get; set; }
        public InsightsMetadataModel? Metadata { get; set; }
    }
}
