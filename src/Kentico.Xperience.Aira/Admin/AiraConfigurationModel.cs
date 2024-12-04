using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Aira.Admin.InfoModels;

namespace Kentico.Xperience.Aira.Admin;

internal class AiraConfigurationModel
{
    [RequiredValidationRule]
    [TextInputComponent(Label = "Relative Path Base", Order = 0, ExplanationText = "Relative path where the ACA is available.")]
    public string RelativePathBase { get; set; } = string.Empty;

    public AiraConfigurationModel() { }

    public AiraConfigurationModel(
        AiraConfigurationItemInfo airaConfigurationInfo) => RelativePathBase = airaConfigurationInfo.AiraConfigurationItemAiraPathBase;

    public void MapToAiraConfigurationInfo(AiraConfigurationItemInfo info) => RelativePathBase = info.AiraConfigurationItemAiraPathBase;
}
