using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Insights;

/// <summary>
/// Service providing Aira Unified insights.
/// </summary>
internal interface IAiraUnifiedInsightsService
{
    /// <summary>
    /// Gets content items insights.
    /// </summary>
    /// <param name="contentType"><see cref="ContentType"/>Reusable or website content type.</param>
    /// <param name="userId">Admin application user.</param>
    /// <param name="status">Status of the content type.</param>
    /// <returns>A list of <see cref="ContentItemModel"/> containing content item insights.</returns>
    Task<List<ContentItemModel>> GetContentInsights(ContentType contentType, int userId, string? status = null);


    /// <summary>
    /// Gets email insights.
    /// </summary>
    /// <returns>A list of <see cref="EmailCampaignModel"/> containing email campaign insights.</returns>
    Task<List<EmailCampaignModel>> GetEmailInsights();


    /// <summary>
    /// Gets contact groups insights.
    /// </summary>
    /// <param name="names">Names of the contact groups.</param>
    /// <returns>A <see cref="ContactGroupsInsightsModel"/> containing contact group insights.</returns>
    ContactGroupsInsightsModel GetContactGroupInsights(string[] names);
}
