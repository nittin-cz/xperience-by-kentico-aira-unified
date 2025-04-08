using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

using Kentico.Xperience.AiraUnified;
using Kentico.Xperience.AiraUnified.Admin;

using Microsoft.Extensions.DependencyInjection;

[assembly: RegisterModule(type: typeof(AiraUnifiedModule))]

namespace Kentico.Xperience.AiraUnified;

/// <summary>
/// Represents the Aira Unified module.
/// </summary>
internal sealed class AiraUnifiedModule : Module
{
    private IAiraUnifiedModuleInstaller installer = null!;
    private AiraUnifiedEndpointDataSource endpointDataSource = null!;

    /// <summary>
    /// Initializes a new instance of the AiraUnifiedModule class.
    /// </summary>
    public AiraUnifiedModule() : base(nameof(AiraUnifiedModule))
    {
    }

    /// <inheritdoc />
    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        var services = parameters.Services;

        installer = services.GetRequiredService<IAiraUnifiedModuleInstaller>();
        endpointDataSource = services.GetRequiredService<AiraUnifiedEndpointDataSource>();

        ApplicationEvents.Initialized.Execute += InitializeModule;
    }

    private void InitializeModule(object? sender, EventArgs e)
    {
        installer.Install();
        endpointDataSource.UpdateEndpoints();
    }
}
