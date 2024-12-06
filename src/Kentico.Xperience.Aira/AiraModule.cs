using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira;

using Microsoft.Extensions.DependencyInjection;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Aira.Widgets;

[assembly: RegisterModule(type: typeof(AiraModule))]
[assembly: RegisterWidget("Kentico.Xperience.Aira.RegistrationWidget", typeof(RegistrationWidgetViewComponent), "Registration Widget", typeof(RegistrationWidgetProperties))]

namespace Kentico.Xperience.Aira;

internal class AiraModule : Module
{
    private IAiraModuleInstaller installer = null!;
    private AiraEndpointDataSource endpointDataSource = null!;

    public AiraModule() : base(nameof(AiraModule))
    {
    }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        var services = parameters.Services;

        installer = services.GetRequiredService<IAiraModuleInstaller>();
        endpointDataSource = services.GetRequiredService<AiraEndpointDataSource>();

        ApplicationEvents.Initialized.Execute += InitializeModule;
    }

    private void InitializeModule(object? sender, EventArgs e)
    {
        installer.Install();
        endpointDataSource.UpdateEndpoints();
    }
}
