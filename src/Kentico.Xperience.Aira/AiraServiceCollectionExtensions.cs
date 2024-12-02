using Kentico.Xperience.Aira.Admin;

namespace Microsoft.Extensions.DependencyInjection;

public static class AiraServiceCollectionExtensions
{
    public static IServiceCollection AddKenticoAira(this IServiceCollection services)
        => services.AddKenticoAiraInternal();

    private static IServiceCollection AddKenticoAiraInternal(this IServiceCollection services)
        => services.AddSingleton<IAiraModuleInstaller, AiraModuleInstaller>();
}
