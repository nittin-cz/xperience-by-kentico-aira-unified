using CMS.ContactManagement;
using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.ContentWorkflowEngine;
using CMS.DataEngine;
using CMS.EmailLibrary;

using Kentico.Xperience.AiraUnified.Insights.Models;

namespace Kentico.Xperience.AiraUnified.Insights;

/// <summary>
/// Implementation of the Aira Unified insights service that provides content, email, and contact group insights.
/// </summary>
internal sealed class AiraUnifiedInsightsService : IAiraUnifiedInsightsService
{
    private readonly IContentItemManagerFactory contentItemManagerFactory;
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;
    private readonly IInfoProvider<ContactGroupInfo> contactGroupInfoProvider;
    private readonly IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider;
    private readonly IInfoProvider<ContentItemLanguageMetadataInfo> contentItemLanguageMetadataInfoProvider;
    private readonly IInfoProvider<ContentWorkflowStepInfo> contentWorkflowStepInfoProvider;
    private readonly IInfoProvider<EmailStatisticsInfo> emailStatisticsInfoProvider;
    private readonly IInfoProvider<EmailConfigurationInfo> emailConfigurationInfoProvider;
    private readonly IInfoProvider<ContactInfo> contactInfoProvider;
    private readonly IInfoProvider<EmailChannelInfo> emailChannelInfoProvider;

    private string[]? reusableTypes;
    private string[]? emailTypes;
    private string[]? pageTypes;
    private string[] ReusableTypes => reusableTypes ??= GetReusableTypes();
    private string[] PageTypes => pageTypes ??= GetPageTypes();
    private string[] EmailTypes => emailTypes ??= GetEmailTypes();

    private const string CONTENT_ITEM_EMAIL_TYPE = "Email";
    private const string CONTENT_ITEM_WEBSITE_TYPE = "Website";
    private const string CONTENT_ITEM_REUSABLE_TYPE = "Reusable";

    private const string SCHEDULED_IDENTIFIER = "Scheduled";
    private const string DRAFT_IDENTIFIER = "Draft";

    /// <summary>
    /// Initializes a new instance of the <see cref="AiraUnifiedInsightsService"/> class.
    /// </summary>
    /// <param name="contentItemManagerFactory">Factory for creating content item managers.</param>
    /// <param name="contentQueryExecutor">Executor for content queries.</param>
    /// <param name="contentLanguageInfoProvider">Provider for content language information.</param>
    /// <param name="contactGroupInfoProvider">Provider for contact group information.</param>
    /// <param name="contactGroupMemberInfoProvider">Provider for contact group member information.</param>
    /// <param name="contentItemLanguageMetadataInfoProvider">Provider for content item language metadata.</param>
    /// <param name="contentWorkflowStepInfoProvider">Provider for content workflow step information.</param>
    /// <param name="emailStatisticsInfoProvider">Provider for email statistics information.</param>
    /// <param name="emailConfigurationInfoProvider">Provider for email configuration information.</param>
    /// <param name="contactInfoProvider">Provider for contact information.</param>
    /// <param name="emailChannelInfoProvider">Provider for email channel information.</param>
    public AiraUnifiedInsightsService(
        IContentItemManagerFactory contentItemManagerFactory,
        IContentQueryExecutor contentQueryExecutor,
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider,
        IInfoProvider<ContactGroupInfo> contactGroupInfoProvider,
        IInfoProvider<ContactGroupMemberInfo> contactGroupMemberInfoProvider,
        IInfoProvider<ContentItemLanguageMetadataInfo> contentItemLanguageMetadataInfoProvider,
        IInfoProvider<ContentWorkflowStepInfo> contentWorkflowStepInfoProvider,
        IInfoProvider<EmailStatisticsInfo> emailStatisticsInfoProvider,
        IInfoProvider<EmailConfigurationInfo> emailConfigurationInfoProvider,
        IInfoProvider<ContactInfo> contactInfoProvider,
        IInfoProvider<EmailChannelInfo> emailChannelInfoProvider)
    {
        this.contentItemManagerFactory = contentItemManagerFactory;
        this.contentQueryExecutor = contentQueryExecutor;
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;
        this.contactGroupInfoProvider = contactGroupInfoProvider;
        this.contactGroupMemberInfoProvider = contactGroupMemberInfoProvider;
        this.contentItemLanguageMetadataInfoProvider = contentItemLanguageMetadataInfoProvider;
        this.contentWorkflowStepInfoProvider = contentWorkflowStepInfoProvider;
        this.emailStatisticsInfoProvider = emailStatisticsInfoProvider;
        this.emailConfigurationInfoProvider = emailConfigurationInfoProvider;
        this.contactInfoProvider = contactInfoProvider;
        this.emailChannelInfoProvider = emailChannelInfoProvider;
    }

    /// <inheritdoc />
    public async Task<List<ContentItemModel>> GetContentInsights(ContentType contentType, int userId, string? status = null)
    {
        var content = await GetContent(userId, contentType.ToString(), status);

        return content.ToList();
    }

    /// <inheritdoc />
    public async Task<List<EmailCampaignModel>> GetEmailInsights()
    {
        var statistics = await emailStatisticsInfoProvider.Get().GetEnumerableTypedResultAsync();

        var regularEmails = await emailConfigurationInfoProvider
            .Get()
            .WhereEquals(nameof(EmailConfigurationInfo.EmailConfigurationPurpose), "Regular")
            .GetEnumerableTypedResultAsync();

        var emailsInsights = new List<EmailCampaignModel>();
        var emailChannels = (await emailChannelInfoProvider.Get().GetEnumerableTypedResultAsync()).ToList();
        
        foreach (var email in regularEmails)
        {
            var stats = statistics.FirstOrDefault(s => s.EmailStatisticsEmailConfigurationID == email.EmailConfigurationID);

            var channelDefaultLanguage = emailChannels.FirstOrDefault(item =>
                item.EmailChannelChannelID == email.EmailConfigurationEmailChannelID)?.EmailChannelPrimaryContentLanguageID ?? 1;
            
            var languageMetadata = contentItemLanguageMetadataInfoProvider
                .Get()
                .WhereEquals(nameof(ContentItemLanguageMetadataInfo.ContentItemLanguageMetadataContentItemID), email.EmailConfigurationContentItemID)
                .WhereEquals(nameof(ContentItemLanguageMetadataInfo.ContentItemLanguageMetadataContentLanguageID), channelDefaultLanguage)
                .FirstOrDefault();

            var emailInsight = new EmailCampaignModel
            {
                Name = languageMetadata?.ContentItemLanguageMetadataDisplayName ?? email.EmailConfigurationName,
                Id = email.EmailConfigurationID.ToString(),
                LastModified = languageMetadata?.ContentItemLanguageMetadataModifiedWhen
            };
            
            if (stats != null)
            {
                emailInsight.Metrics = new EmailMetricsModel()
                {
                    TotalSent = stats.EmailStatisticsTotalSent,
                    Delivered = stats.EmailStatisticsEmailsDelivered,
                    Opened = stats.EmailStatisticsEmailOpens,
                    Clicks = stats.EmailStatisticsEmailClicks,
                    UniqueClicks = stats.EmailStatisticsEmailUniqueClicks,
                    UnsubscribeRate = stats.EmailStatisticsUniqueUnsubscribes,
                    SpamReports = stats.EmailStatisticsSpamReports ?? 0,
                    SoftBounces = stats.EmailStatisticsEmailSoftBounces,
                    HardBounces = stats.EmailStatisticsEmailHardBounces
                };
            }
            
            emailsInsights.Add(emailInsight);
        }

        return emailsInsights;
    }

    /// <inheritdoc />
    public ContactGroupsInsightsModel GetContactGroupInsights(string[] names)
    {
        var allCount = contactInfoProvider.Get().ToList().Count;
        var contactGroups = GetContactGroups(names);
        var groups = new List<ContactGroupInsightsModel>();

        foreach (var contactGroup in contactGroups)
        {
            groups.Add(new ContactGroupInsightsModel
            {
                Id = contactGroup.ContactGroupID,
                Name = contactGroup.ContactGroupDisplayName,
                Conditions = contactGroup.ContactGroupDynamicCondition,
                Count = contactGroupMemberInfoProvider.GetContactsInContactGroupCount(contactGroup.ContactGroupID)
            });
        }

        return new ContactGroupsInsightsModel
        {
            AllCount = allCount,
            Groups = groups
        };
    }

    /// <summary>
    /// Gets content items based on the specified parameters.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="classType">The content class type.</param>
    /// <param name="status">The content status.</param>
    /// <returns>A collection of content item models.</returns>
    private async Task<IEnumerable<ContentItemModel>> GetContent(int userId, string classType = CONTENT_ITEM_REUSABLE_TYPE, string? status = null)
    {
        var builder = classType switch
        {
            CONTENT_ITEM_EMAIL_TYPE => GetContentItemBuilder(EmailTypes),
            CONTENT_ITEM_WEBSITE_TYPE => GetContentItemBuilder(PageTypes),
            _ => GetContentItemBuilder(ReusableTypes),
        };

        var options = new ContentQueryExecutionOptions
        {
            ForPreview = true,
            IncludeSecuredItems = true
        };

        if (builder is null)
        {
            return [];
        }

        if (status == DRAFT_IDENTIFIER)
        {
            builder.Parameters(q => q.Where(w => w
                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataVersionStatus), VersionStatus.Draft)
                .Or()
                .WhereEquals(nameof(ContentItemCommonDataInfo.ContentItemCommonDataVersionStatus), VersionStatus.InitialDraft)));

            return await contentQueryExecutor.GetResult(builder, ContentItemBinder, options);
        }

        var items = await contentQueryExecutor.GetResult(builder, ContentItemBinder, options);

        if (string.IsNullOrEmpty(status))
        {
            return items;
        }

        return status switch
        {
            DRAFT_IDENTIFIER => FilterDrafts(items),
            SCHEDULED_IDENTIFIER => await FilterScheduled(userId, items),
            _ => await FilterCustomWorkflowStep(items, status),
        };
    }

    /// <summary>
    /// Gets contact groups by their names.
    /// </summary>
    /// <param name="names">Array of contact group names.</param>
    /// <returns>A collection of contact group information.</returns>
    private IEnumerable<ContactGroupInfo> GetContactGroups(string[] names)
    {
        List<ContactGroupInfo> result = [];

        if (names is not null && names.Length > 0)
        {
            foreach (var name in names)
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var group = contactGroupInfoProvider.Get().Where(g => g.ContactGroupDisplayName == name).FirstOrDefault();
                if (group is not null)
                {
                    result.Add(group);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Gets a content item query builder for the specified content types.
    /// </summary>
    /// <param name="contentTypes">Array of content type names.</param>
    /// <returns>A content item query builder or null if no content types are specified.</returns>
    private static ContentItemQueryBuilder? GetContentItemBuilder(string[] contentTypes)
    {
        var builder = new ContentItemQueryBuilder();

        return contentTypes.Length switch
        {
            0 => null,
            1 => builder.ForContentType(contentTypes[0]),
            _ => builder.ForContentTypes(q => q.OfContentType(contentTypes)),
        };
    }

    /// <summary>
    /// Filters content items to only include scheduled items.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="items">The content items to filter.</param>
    /// <returns>A collection of scheduled content items.</returns>
    private async Task<IEnumerable<ContentItemModel>> FilterScheduled(int userId, IEnumerable<ContentItemModel> items)
    {
        List<ContentItemModel> result = [];

        var contentItemManager = contentItemManagerFactory.Create(userId);
        foreach (var item in items)
        {
            var language = await contentLanguageInfoProvider.GetAsync(item.LanguageId);
            var isScheduled = await contentItemManager.IsPublishScheduled(item.Id, language.ContentLanguageName);
            if (isScheduled)
            {
                result.Add(item);
            }
        }

        return result;
    }

    /// <summary>
    /// Filters content items to only include draft items.
    /// </summary>
    /// <param name="items">The content items to filter.</param>
    /// <returns>A collection of draft content items.</returns>
    private static IEnumerable<ContentItemModel> FilterDrafts(IEnumerable<ContentItemModel> items)
    {
        List<ContentItemModel> result = [];

        foreach (var item in items)
        {
            if (item.VersionStatus == VersionStatus.Draft)
            {
                result.Add(item);
            }
        }

        return result;
    }

    /// <summary>
    /// Filters content items to only include items in a specific workflow step.
    /// </summary>
    /// <param name="items">The content items to filter.</param>
    /// <param name="status">The workflow step status.</param>
    /// <returns>A collection of content items in the specified workflow step.</returns>
    private async Task<IEnumerable<ContentItemModel>> FilterCustomWorkflowStep(IEnumerable<ContentItemModel> items, string? status)
    {
        List<ContentItemModel> result = [];

        var step = contentWorkflowStepInfoProvider.Get().WhereEquals(nameof(ContentWorkflowStepInfo.ContentWorkflowStepDisplayName), status).FirstOrDefault();

        if (step is not null)
        {
            var languageMetadata = await contentItemLanguageMetadataInfoProvider
                .Get()
                .WhereEquals(nameof(ContentItemLanguageMetadataInfo.ContentItemLanguageMetadataContentWorkflowStepID), step.ContentWorkflowStepID)
                .GetEnumerableTypedResultAsync();

            result.AddRange(
                items.Where(item => languageMetadata
                    .Any(m =>
                        m.ContentItemLanguageMetadataContentItemID == item.Id &&
                        m.ContentItemLanguageMetadataContentLanguageID == item.LanguageId
                    )
                )
            );
        }
        return result;
    }

    /// <summary>
    /// Binds content query data to a content item model.
    /// </summary>
    /// <param name="container">The content query data container.</param>
    /// <returns>A content item model.</returns>
    private ContentItemModel ContentItemBinder(IContentQueryDataContainer container) => new()
    {
        Id = container.ContentItemID,
        Name = container.ContentItemName,
        DisplayName = container.ContentItemName,
        ContentTypeId = container.ContentItemContentTypeID,
        ContentTypeName = container.ContentTypeName,
        VersionStatus = container.ContentItemCommonDataVersionStatus,
        LanguageId = container.ContentItemCommonDataContentLanguageID,
    };

    /// <summary>
    /// Gets an array of reusable content type names.
    /// </summary>
    /// <returns>Array of reusable content type names.</returns>
    private static string[] GetReusableTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, CONTENT_ITEM_REUSABLE_TYPE)
            .Select(c => c.ClassName)
            .ToArray();

    /// <summary>
    /// Gets an array of page content type names.
    /// </summary>
    /// <returns>Array of page content type names.</returns>
    private static string[] GetPageTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, CONTENT_ITEM_WEBSITE_TYPE)
            .Select(c => c.ClassName)
            .ToArray();

    /// <summary>
    /// Gets an array of email content type names.
    /// </summary>
    /// <returns>Array of email content type names.</returns>
    private static string[] GetEmailTypes() =>
        DataClassInfoProvider.GetClasses()
            .Where(nameof(DataClassInfo.ClassContentTypeType), QueryOperator.Equals, CONTENT_ITEM_EMAIL_TYPE)
            .Select(c => c.ClassName)
            .ToArray();
}
