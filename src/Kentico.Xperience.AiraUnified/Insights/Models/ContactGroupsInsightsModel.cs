namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Contact groups insights model.
/// </summary>
internal sealed class ContactGroupsInsightsModel
{
    /// <summary>
    /// Contact group insights model.
    /// </summary>
    public List<ContactGroupInsightsModel> Groups { get; set; } = [];

    /// <summary>
    /// Contact groups dynamic count.
    /// </summary>
    public int AllCount { get; set; }
}
