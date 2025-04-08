using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Insights;

/// <summary>
/// Service providing aira unified insights.
/// </summary>
internal interface IAiraUnifiedInsightsService
{
    /// <summary>
    /// Gets content items insights.
    /// </summary>
    /// <param name="contentType"><see cref="ContentType"/>Reusable or website content type.</param>
    /// <param name="userId">Admin application user.</param>
    /// <param name="status">Status of the content type.</param>
    /// <returns><see cref="ContentItemModel"/></returns>
    Task<List<ContentItemModel>> GetContentInsights(ContentType contentType, int userId, string? status = null);

    /// <summary>
    /// Gets email insights.
    /// </summary>
    /// <returns><see cref="EmailCampaignModel"/></returns>
    Task<List<EmailCampaignModel>> GetEmailInsights();

    /// <summary>
    /// Gets contact groups insights.
    /// </summary>
    /// <param name="names">Names of the contact groups.</param>
    /// <returns><see cref="ContactGroupsInsightsModel"/></returns>
    ContactGroupsInsightsModel GetContactGroupInsights(string[] names);
}
