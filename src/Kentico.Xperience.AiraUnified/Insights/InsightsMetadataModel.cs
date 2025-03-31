namespace Kentico.Xperience.AiraUnified.Insights
{
    public class InsightsMetadataModel
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? InstanceName { get; set; }
        public string? Version { get; set; }
        public string? DataFreshness { get; set; }
    }
}
