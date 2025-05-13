using CMS.ContentEngine;

using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Insights;

/// <summary>
/// Mock implementation of insights service for local development.
/// </summary>
internal sealed class MockAiraUnifiedInsightsService : IAiraUnifiedInsightsService
{
    /// <inheritdoc />
    public Task<List<ContentItemModel>> GetContentInsights(ContentType contentType, int userId, string? status = null) =>
        Task.FromResult(new List<ContentItemModel>
        {
            new()
            {
                Id = 1,
                Name = "Newsletter Template",
                DisplayName = "Newsletter Template",
                ContentTypeId = 1,
                ContentTypeName = "Email",
                VersionStatus = VersionStatus.InitialDraft,
                LanguageId = 1
            },
            new()
            {
                Id = 2,
                Name = "Product Catalog",
                DisplayName = "Product Catalog",
                ContentTypeId = 2,
                ContentTypeName = "Catalog",
                VersionStatus = VersionStatus.Published,
                LanguageId = 1
            },
            new()
            {
                Id = 3,
                Name = "Home Page",
                DisplayName = "Home Page",
                ContentTypeId = 3,
                ContentTypeName = "Page",
                VersionStatus = VersionStatus.InitialDraft,
                LanguageId = 1
            },
            new()
            {
                Id = 4,
                Name = "About Us",
                DisplayName = "About Us",
                ContentTypeId = 3,
                ContentTypeName = "Page",
                VersionStatus = VersionStatus.Published,
                LanguageId = 1
            }
        });


    /// <inheritdoc />
    public Task<List<EmailCampaignModel>> GetEmailInsights() =>
        Task.FromResult(new List<EmailCampaignModel>
        {
            new()
            {
                Id = "1",
                Name = "Monthly Newsletter",
                Type = "reusable",
                Status = "sent",
                LastModified = DateTime.UtcNow.AddDays(-5),
                SentDate = DateTime.UtcNow.AddDays(-4),
                Metrics = new EmailMetricsModel
                {
                    TotalSent = 10000,
                    Delivered = 9850,
                    Opened = 4500,
                    OpenRate = 45.7M,
                    Clicks = 1200M,
                    UniqueClicks = 1000M,
                    UnsubscribeRate = 0.5M,
                    SpamReports = 2,
                    SoftBounces = 50,
                    HardBounces = 100
                }
            },
            new()
            {
                Id = "2",
                Name = "Product Launch",
                Type = "website",
                Status = "scheduled",
                LastModified = DateTime.UtcNow.AddDays(-1),
                Metrics = new EmailMetricsModel
                {
                    TotalSent = 0,
                    Delivered = 0,
                    Opened = 0,
                    OpenRate = 0M,
                    Clicks = 0M,
                    UniqueClicks = 0M,
                    UnsubscribeRate = 0M,
                    SpamReports = 0,
                    SoftBounces = 0,
                    HardBounces = 0
                }
            }
        });


    /// <inheritdoc />
    public ContactGroupsInsightsModel GetContactGroupInsights(string[] names) =>
        new()
        {
            AllCount = 1000,
            Groups = names.Select(name => new ContactGroupInsightsModel
            {
                Id = name switch
                {
                    "Newsletter Subscribers" => 1,
                    "Active Customers" => 2,
                    "Prospects" => 3,
                    _ => 0
                },
                Name = name,
                Conditions = "Dynamic condition for " + name,
                Count = name switch
                {
                    "Newsletter Subscribers" => 500,
                    "Active Customers" => 300,
                    "Prospects" => 200,
                    _ => 0
                }
            }).ToList()
        };
}
