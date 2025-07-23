using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.AiraUnified.Components.Insights;

/// <summary>
/// Blazor component for displaying insights data.
/// </summary>
public partial class InsightsComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets the category of the insights.
    /// </summary>
    [Parameter] public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the data for the insights.
    /// </summary>
    [Parameter] public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the insights.
    /// </summary>
    [Parameter] public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the component type for dynamic rendering.
    /// </summary>
    [Parameter] public Type? ComponentType { get; set; }

    /// <summary>
    /// Renders the component dynamically based on ComponentType.
    /// </summary>
    /// <returns>A render fragment for the dynamic component.</returns>
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
