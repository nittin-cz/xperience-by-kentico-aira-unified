using Kentico.Xperience.AiraUnified.Admin.InfoModels;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Service for managing Aira Unified configuration settings.
/// </summary>
public interface IAiraUnifiedConfigurationService
{
    /// <summary>
    /// Retrieves the Aira Unified configuration.
    /// </summary>
    Task<AiraUnifiedConfigurationItemInfo> GetAiraUnifiedConfiguration();


    /// <summary>
    /// Tries to save or update the Aira Unified configuration.
    /// </summary>
    /// <param name="configurationModel">The configuration model to save or update.</param>
    /// <returns>True if the configuration was saved or updated successfully, false otherwise.</returns>
    Task<bool> TrySaveOrUpdateConfiguration(AiraUnifiedConfigurationModel configurationModel);
}
