using CMS.DataEngine;
using CMS.DataEngine.Query;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Aira.Admin.InfoModels;

using IFormItemCollectionProvider = Kentico.Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider;

namespace Kentico.Xperience.Aira.Admin.UIPages;

internal abstract class BaseAiraConfigurationEditPage : ModelEditPage<AiraConfigurationModel>
{
    protected readonly IInfoProvider<AiraConfigurationItemInfo> AiraConfigurationProvider;

    protected BaseAiraConfigurationEditPage(
        IFormItemCollectionProvider formItemCollectionProvider,
        IFormDataBinder formDataBinder,
        IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider)
        : base(formItemCollectionProvider, formDataBinder)
    => AiraConfigurationProvider = airaConfigurationProvider;

    protected async Task<AiraConfiguratioResult> ValidateAndProcess(AiraConfigurationModel configuration)
    {
        var existingConfiguration = (await AiraConfigurationProvider.Get().GetEnumerableTypedResultAsync()).SingleOrDefault();

        if (existingConfiguration is null)
        {
            if (!string.IsNullOrWhiteSpace(configuration.RelativePathBase))
            {
                int configurationCount = await AiraConfigurationProvider.Get().GetCountAsync();

                if (configurationCount > 0)
                {
                    return AiraConfiguratioResult.Failure;
                }

                var newConfigurationInfo = new AiraConfigurationItemInfo
                {
                    AiraConfigurationItemAiraPathBase = configuration.RelativePathBase
                };

                AiraConfigurationProvider.Set(newConfigurationInfo);

                return AiraConfiguratioResult.Success;
            }

            return AiraConfiguratioResult.Failure;
        }

        existingConfiguration.AiraConfigurationItemAiraPathBase = configuration.RelativePathBase;
        AiraConfigurationProvider.Set(existingConfiguration);

        return AiraConfiguratioResult.Success;
    }
}

internal enum AiraConfiguratioResult
{
    Success,
    Failure
}
