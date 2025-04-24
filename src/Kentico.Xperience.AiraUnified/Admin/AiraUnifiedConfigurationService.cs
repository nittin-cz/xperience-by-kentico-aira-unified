using CMS.DataEngine;
using CMS.DataEngine.Query;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Service for managing Aira Unified configuration settings.
/// </summary>
internal sealed class AiraUnifiedConfigurationService : IAiraUnifiedConfigurationService
{
    private readonly IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider;
    private readonly AiraUnifiedEndpointDataSource airaUnifiedEndpointDataSource;

    private const string AiraUnifiedConfigurationNotFoundMessage = "Aira Unified has not been configured yet.";

    /// <summary>
    /// Initializes a new instance of the AiraUnifiedConfigurationService class.
    /// </summary>
    /// <param name="airaUnifiedConfigurationProvider">The provider for Aira Unified configuration items.</param>
    /// <param name="airaUnifiedEndpointDataSource">The data source for Aira Unified endpoints.</param>
    public AiraUnifiedConfigurationService(IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider, AiraUnifiedEndpointDataSource airaUnifiedEndpointDataSource)
    {
        this.airaUnifiedConfigurationProvider = airaUnifiedConfigurationProvider;
        this.airaUnifiedEndpointDataSource = airaUnifiedEndpointDataSource;
    }


    /// <inheritdoc/>
    public async Task<bool> TrySaveOrUpdateConfiguration(AiraUnifiedConfigurationModel configurationModel)
    {
        var existingConfiguration = (await airaUnifiedConfigurationProvider.Get().GetEnumerableTypedResultAsync()).SingleOrDefault();

        if (existingConfiguration is null)
        {
            if (string.IsNullOrWhiteSpace(configurationModel.RelativePathBase))
            {
                return false;
            }
            var configurationCount = await airaUnifiedConfigurationProvider.Get().GetCountAsync();

            if (configurationCount > 0)
            {
                return false;
            }

            var newConfigurationInfo = configurationModel.MapToAiraUnifiedConfigurationInfo();
            await airaUnifiedConfigurationProvider.SetAsync(newConfigurationInfo);

            return true;
        }

        existingConfiguration = configurationModel.MapToAiraUnifiedConfigurationInfo(existingConfiguration);

        await airaUnifiedConfigurationProvider.SetAsync(existingConfiguration);

        airaUnifiedEndpointDataSource.UpdateEndpoints();

        return true;
    }


    /// <inheritdoc />
    public Task<AiraUnifiedConfigurationItemInfo> GetAiraUnifiedConfiguration()
    {
        var airaUnifiedConfigurationItemList = airaUnifiedConfigurationProvider.Get().SingleOrDefault();

        return Task.FromResult(airaUnifiedConfigurationItemList
                               ?? throw new InvalidOperationException(AiraUnifiedConfigurationNotFoundMessage));
    }
}
