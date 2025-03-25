namespace Kentico.Xperience.AiraUnified.Insights
{
    public class InsightsMetadataModel
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string InstanceName { get; set; } = "Xperience by Kentico";
        public string Version { get; set; } = "30.2.0";
        public string DataFreshness { get; set; } = "just now";
    }
}
