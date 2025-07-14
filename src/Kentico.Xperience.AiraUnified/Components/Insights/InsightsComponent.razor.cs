using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.AiraUnified.Components.Insights;

public partial class InsightsComponent : ComponentBase
{
    [Parameter] public string? Category { get; set; }
    [Parameter] public object? Data { get; set; }
    [Parameter] public DateTime? Timestamp { get; set; }
    [Parameter] public Type? ComponentType { get; set; }
    
    /// <summary>
    /// Renders the component dynamically based on ComponentType
    /// </summary>
    private RenderFragment RenderDynamicComponent() => builder =>
    {
        if (ComponentType == null || Data == null)
        {
            return;
        }

        try
        {
            builder.OpenComponent(0, ComponentType);
            builder.AddAttribute(1, "Data", Data);
            builder.AddAttribute(2, "Timestamp", Timestamp ?? DateTime.Now);
            builder.CloseComponent();
        }
        catch (Exception ex)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "alert alert-danger");
            builder.AddContent(2, $"Error rendering component {ComponentType.Name}: {ex.Message}");
            builder.CloseElement();
        }
    };
}
