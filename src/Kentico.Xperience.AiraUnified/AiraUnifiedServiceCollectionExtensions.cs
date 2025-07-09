using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Insights;
using Kentico.Xperience.AiraUnified.NavBar;
using Kentico.Xperience.AiraUnified.Services;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Application startup extension methods.
/// </summary>
public static class AiraUnifiedServiceCollectionExtensions
{
    /// <summary>
    /// Adds Aira Unified services and custom module to application.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>The application services.</param>
    /// <param name="configuration">The application <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> application services.</returns>
    public static IServiceCollection AddKenticoAiraUnified(this IServiceCollection services, IConfiguration configuration)
        => services.AddKenticoAiraUnifiedInternal(configuration);


    /// <summary>
    /// Allows using the Aira Unified dynamic endpoint creation.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void UseAiraUnifiedEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<AiraUnifiedEndpointDataSource>()
            ?? throw new InvalidOperationException("Did you forget to call Services.AddKenticoAiraUnified()?");
        endpoints.DataSources.Add(dataSource);
    }


    private static IServiceCollection AddKenticoAiraUnifiedInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        services.AddHttpClient();

        var options = configuration.GetSection(nameof(AiraUnifiedOptions)).Get<AiraUnifiedOptions>();

        services
            .AddSingleton<IAiraUnifiedModuleInstaller, AiraUnifiedModuleInstaller>()
            .AddSingleton<AiraUnifiedEndpointDataSource>()
            .AddScoped<ContentItemAssetUploaderComponent>()
            .AddScoped<AiraUnifiedConfigurationService>()
            .AddScoped<IAiraUnifiedConfigurationService, AiraUnifiedConfigurationService>()
            .AddScoped<IAiraUnifiedInsightsService>(sp => options?.AiraUnifiedUseMockInsights == true
                ? new MockAiraUnifiedInsightsService()
                : sp.GetRequiredService<AiraUnifiedInsightsService>())
            .AddScoped<AiraUnifiedInsightsService>()
            .AddScoped<IAiraUnifiedAssetService, AiraUnifiedAssetService>()
            .AddScoped<IAiraUnifiedChatService, AiraUnifiedChatService>()
            .AddScoped<INavigationService, NavigationService>()
            .AddScoped<IAiHttpClient>(sp => options?.AiraUnifiedUseMockClient == true ? new MockAiHttpClient() : new AiHttpClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<IOptions<AiraUnifiedOptions>>()))
            .Configure<AiraUnifiedOptions>(configuration.GetSection(nameof(AiraUnifiedOptions)));

        services.AddServerSideBlazor(circuitOptions =>
        {
            circuitOptions.DetailedErrors = true; // Pro development
        });
        
        services.AddScoped<IChatService, ChatService>();

        // Session pro chat
        // services.AddSession(sessionOptions =>
        // {
        //     sessionOptions.IdleTimeout = TimeSpan.FromMinutes(30);
        //     sessionOptions.Cookie.HttpOnly = true;
        //     sessionOptions.Cookie.IsEssential = true;
        //     sessionOptions.Cookie.Name = ".AspNetCore.Session";
        // });
        
        return services;
    }
}
