using Kentico.Xperience.AiraUnified.NavBar.Models;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Provides application constant strings.
/// </summary>
internal static class AiraUnifiedConstants
{
    /// <summary>
    /// Relative path to the chat page in the PWA.
    /// </summary>
    public const string ChatRelativeUrl = "chat";


    /// <summary>
    /// Relative path to create a new AIRA chat thread.
    /// </summary>
    public const string NewChatThreadRelativeUrl = "threads/new";


    /// <summary>
    /// Relative path to user's threads selector page.
    /// </summary>
    public const string ChatThreadSelectorRelativeUrl = "threads";


    /// <summary>
    /// Relative path to user's thread list.
    /// </summary>
    public const string AllChatThreadsRelativeUrl = "all";


    /// <summary>
    /// Relative path for sending chat messages to aira unified chat.
    /// </summary>
    public const string ChatMessageUrl = "message";


    /// <summary>
    /// Relative path for retrieval of user's chat history.
    /// </summary>
    public const string ChatHistoryUrl = "history";


    /// <summary>
    /// Relative path for retrieval of the navigation model <see cref="NavBarViewModel"/> data.
    /// </summary>
    public const string NavigationUrl = "nav";


    /// <summary>
    /// Relative path to the smart upload page in the PWA.
    /// </summary>
    public const string SmartUploadRelativeUrl = "assets";


    /// <summary>
    /// Relative path for retrieval of allowed file extensions in the Smart upload Aira unified page.
    /// </summary>
    public const string SmartUploadAllowedFileExtensionsUrl = "allowedFileExtensions";


    /// <summary>
    /// Relative path for uploading assets to Aira unified.
    /// </summary>
    public const string SmartUploadUploadUrl = "upload";


    /// <summary>
    /// Relative path to the PWA signIn page.
    /// </summary>
    public const string SigninRelativeUrl = "signin";


    /// <summary>
    /// Name of the optional query parameter of the signIn endpoint.
    /// </summary>
    public const string SigninMissingPermissionParameterName = "missingPermission";


    /// <summary>
    /// Name of the post chat message thread id parameter name.
    /// </summary>
    public const string ChatThreadIdParameterName = "chatThreadId";


    /// <summary>
    /// Relative path to the used prompt group removal endpoint.
    /// </summary>
    public const string RemoveUsedPromptGroupRelativeUrl = "prompt/use";


    /// <summary>
    /// The name if the AI assistant role used for Aira Unified chat component identifier.
    /// </summary>
    public const string AiraUnifiedFrontEndChatComponentAIAssistantRoleName = "ai";


    /// <summary>
    /// Aira Unified system role name.
    /// </summary>
    public const string AiraUnifiedSystemRoleName = "system";


    /// <summary>
    /// The name of the AI role used for AI endpoint identifier.
    /// </summary>
    public const string AIRequestAIAssistantRoleName = "assistant";


    /// <summary>
    /// The name of the user role used for AI endpoint identifier.
    /// </summary>
    public const string AIRequestUserRoleName = "user";


    /// <summary>
    /// The AI endpoint.
    /// </summary>
    public const string AiraUnifiedAIEndpoint = "https://kenticoairaapimanager.azure-api.net/aira/rag";


    /// <summary>
    /// Url prefix where static assets of this package are accessible.
    /// </summary>
    public const string RCLUrlPrefix = "_content/Kentico.Xperience.AiraUnified";


    /// <summary>
    /// Path where the placeholder icon is located.
    /// </summary>
    public const string PicturePlaceholderImgPath = "img/icons/picture-placeholder-purple.svg";


    /// <summary>
    /// Path where the network icon is located.
    /// </summary>
    public const string PictureNetworkGraphImgPath = "img/icons/network-purple.svg";


    /// <summary>
    /// Path where the star icon is located.
    /// </summary>
    public const string PictureStarImgPath = "img/icons/stars-icon.svg";


    /// <summary>
    /// Path where the chat bot smile bubble orange icon is located.
    /// </summary>
    public const string PictureChatBotSmileBubbleOrangeImgPath = "img/icons/chatbot-smile-bubble-orange.svg";


    /// <summary>
    /// Name of the Xperience Admin User authentication scheme.
    /// </summary>
    public const string XperienceAdminSchemeName = "Xperience.Application";


    /// <summary>
    /// Resource Display Name of this module.
    /// </summary>
    public const string ResourceDisplayName = "Kentico Integration - Aira Unified";


    /// <summary>
    /// Resource Name of this module.
    /// </summary>
    public const string ResourceName = "CMS.Integration.AiraUnified";


    /// <summary>
    /// Resource Description of this module.
    /// </summary>
    public const string ResourceDescription = "Kentico Aira Unified custom data";


    /// <summary>
    /// Configuration Key of the mass asset upload.
    /// </summary>
    public const string MassAssetUploadConfigurationKey = "CMSMassAssetUploadConfiguration";


    /// <summary>
    /// Resource is in development attribute of this module.
    /// </summary>
    public const bool ResourceIsInDevelopment = false;


    /// <summary>
    /// Insights draft identifier.
    /// </summary>
    public const string InsightsDraftIdentifier = "Draft";


    /// <summary>
    /// Insights scheduled identifier.
    /// </summary>
    public const string InsightsScheduledIdentifier = "Scheduled";


    /// <summary>
    /// Insights published identifier.
    /// </summary>
    public const string InsightsPublishedIdentifier = "Published";


    /// <summary>
    /// Version string used for cache busting of static assets.
    /// This should match the version in package.json.
    /// </summary>
    public const string AssetVersion = "1.1.4";
}
