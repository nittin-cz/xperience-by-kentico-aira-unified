using CMS;
using CMS.Core;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.AiraUnified.Admin;

[assembly: RegisterModule(typeof(AiraUnifiedAdminModule))]

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Manages administration features and integration.
/// </summary>
internal sealed class AiraUnifiedAdminModule() : AdminModule(nameof(AiraUnifiedAdminModule))
{
    /// <summary>
    /// Initializes the admin module with the specified parameters.
    /// </summary>
    /// <param name="parameters">The module initialization parameters.</param>
    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RegisterClientModule("kentico", "xperience-integrations-aira-unified");
    }
}
