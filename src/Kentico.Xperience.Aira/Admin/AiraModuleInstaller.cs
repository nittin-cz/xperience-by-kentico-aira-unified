using CMS.DataEngine;
using CMS.Modules;

namespace Kentico.Xperience.Aira.Admin;

internal interface IAiraModuleInstaller
{
    void Install();
}

internal class AiraModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider) : IAiraModuleInstaller
{
    private readonly IInfoProvider<ResourceInfo> resourceInfoProvider = resourceInfoProvider;

    public void Install()
    {
        var resourceInfo = InstallModule();
        InstallModuleClasses(resourceInfo);
    }
    private ResourceInfo InstallModule()
    {
        var resourceInfo = resourceInfoProvider.Get(AiraConstants.ResourceName)
            ?? resourceInfoProvider.Get("Kentico.Xperience.TagManager")
            ?? new ResourceInfo();

        resourceInfo.ResourceDisplayName = AiraConstants.ResourceDisplayName;
        resourceInfo.ResourceName = AiraConstants.ResourceName;
        resourceInfo.ResourceDescription = AiraConstants.ResourceDescription;
        resourceInfo.ResourceIsInDevelopment = AiraConstants.ResourceIsInDevelopment;
        if (resourceInfo.HasChanged)
        {
            resourceInfoProvider.Set(resourceInfo);
        }

        return resourceInfo;
    }

    private static void InstallModuleClasses(ResourceInfo resourceInfo) { }

}
