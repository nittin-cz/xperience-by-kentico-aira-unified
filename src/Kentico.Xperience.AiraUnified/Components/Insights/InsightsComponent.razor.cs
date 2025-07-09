using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.AiraUnified.Components.Insights;

public partial class InsightsComponent : ComponentBase
{
    [Parameter] public string? Category { get; set; }
    [Parameter] public object? Data { get; set; }
    [Parameter] public DateTime? Timestamp { get; set; }
}
