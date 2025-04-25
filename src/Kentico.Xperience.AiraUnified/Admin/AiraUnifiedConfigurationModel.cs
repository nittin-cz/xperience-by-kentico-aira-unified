using CMS.MediaLibrary;

using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Admin.Providers;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Represents the configuration model for Aira Unified.
/// </summary>
public sealed class AiraUnifiedConfigurationModel
{
    [RequiredValidationRule]
    [TextInputComponent(Label = "{$AiraUnifiedConfigurationModel.RelativePathBase.Label$}", Order = 0,
        ExplanationText = "{$AiraUnifiedConfigurationModel.RelativePathBase.ExplanationText$}")]
    public string RelativePathBase { get; set; } = string.Empty;


    [RequiredValidationRule]
    [AssetSelectorComponent(Label = "{$AiraUnifiedConfigurationModel.Logo.Label$}", Order = 1,
        ExplanationText = "{$AiraUnifiedConfigurationModel.Logo.ExplanationText$}", AllowedExtensions = "jpg;jpeg;png",
        MaximumAssets = 1)]
    public IEnumerable<AssetRelatedItem>? Logo { get; set; }


    [RequiredValidationRule]
    [TextInputComponent(Label = "{$AiraUnifiedConfigurationModel.ChatTitle.Label$}", Order = 2,
        ExplanationText = "{$AiraUnifiedConfigurationModel.ChatTitle.ExplanationText$}")]
    public string ChatTitle { get; set; } = string.Empty;


    [RequiredValidationRule]
    [TextInputComponent(Label = "{$AiraUnifiedConfigurationModel.SmartUploadTitle.Label$}", Order = 3,
        ExplanationText = "{$AiraUnifiedConfigurationModel.SmartUploadTitle.ExplanationText$}")]
    public string SmartUploadTitle { get; set; } = string.Empty;


    [RequiredValidationRule]
    [DropDownComponent(Label = "{$AiraUnifiedConfigurationModel.Workspace.Label$}",
        DataProviderType = typeof(WorkspaceProvider), Order = 4,
        ExplanationText = "{$AiraUnifiedConfigurationModel.Workspace.ExplanationText$}")]
    public string Workspace { get; set; } = string.Empty;


    public AiraUnifiedConfigurationModel() { }


    public AiraUnifiedConfigurationModel(
        AiraUnifiedConfigurationItemInfo airaUnifiedConfigurationInfo) =>
        MapFromAiraUnifiedConfigurationInfo(airaUnifiedConfigurationInfo);


    public void MapFromAiraUnifiedConfigurationInfo(AiraUnifiedConfigurationItemInfo info)
    {
        RelativePathBase = info.AiraUnifiedConfigurationItemAiraPathBase;

        if (Guid.TryParse(info.AiraUnifiedConfigurationItemAiraRelativeLogoId, out var relativeLogoUrlGuid))
        {
            var relativeLogoUrlAsset = new AssetRelatedItem { Identifier = relativeLogoUrlGuid };
            Logo = [relativeLogoUrlAsset];
        }

        ChatTitle = info.AiraUnifiedConfigurationItemAiraUnifiedChatTitle;
        Workspace = info.AiraUnifiedConfigurationWorkspaceName;
        SmartUploadTitle = info.AiraUnifiedConfigurationItemAiraSmartUploadTitle;
    }


    public AiraUnifiedConfigurationItemInfo MapToAiraUnifiedConfigurationInfo(
        AiraUnifiedConfigurationItemInfo? info = null
    )
    {
        info ??= new AiraUnifiedConfigurationItemInfo();
        info.AiraUnifiedConfigurationItemAiraPathBase = RelativePathBase;
        info.AiraUnifiedConfigurationItemAiraRelativeLogoId = GetImageIdentifier(Logo);
        info.AiraUnifiedConfigurationItemAiraUnifiedChatTitle = ChatTitle;
        info.AiraUnifiedConfigurationItemAiraSmartUploadTitle = SmartUploadTitle;
        info.AiraUnifiedConfigurationWorkspaceName = Workspace;

        return info;
    }


    private static string GetImageIdentifier(IEnumerable<AssetRelatedItem>? asset) =>
        asset?.FirstOrDefault()?.Identifier.ToString() ?? string.Empty;
}
