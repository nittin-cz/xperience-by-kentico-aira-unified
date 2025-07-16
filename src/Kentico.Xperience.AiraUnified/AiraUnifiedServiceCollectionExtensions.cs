using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.Chat.Services;
using Kentico.Xperience.AiraUnified.Insights;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Implementation;
using Kentico.Xperience.AiraUnified.Insights.Strategies;
using Kentico.Xperience.AiraUnified.NavBar;

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

        // Register insights architecture components
        services
            .AddScoped<IInsightsOrchestrator, InsightsOrchestrator>()
            .AddScoped<IInsightsStrategyFactory, InsightsStrategyFactory>()
            .AddScoped<IInsightsStrategy, ContentInsightsStrategy>()
            .AddScoped<IInsightsStrategy, EmailInsightsStrategy>()
            .AddScoped<IInsightsStrategy, MarketingInsightsStrategy>()
            .AddScoped<EnhancedInsightsParser>();

        services.AddServerSideBlazor(circuitOptions =>
        {
            circuitOptions.DetailedErrors = true;
            circuitOptions.DisconnectedCircuitMaxRetained = 100;
            circuitOptions.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
            circuitOptions.MaxBufferedUnacknowledgedRenderBatches = 10;
        });

        return services;
    }

    /// <summary>
    /// Registers a custom insights strategy implementation.
    /// </summary>
    /// <typeparam name="TStrategy">The strategy implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddInsightsStrategy<TStrategy>(this IServiceCollection services)
        where TStrategy : class, IInsightsStrategy => services.AddScoped<IInsightsStrategy, TStrategy>();

    /// <summary>
    /// Registers a custom insights strategy implementation with a factory.
    /// </summary>
    /// <typeparam name="TStrategy">The strategy implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="factory">The factory method to create the strategy instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddInsightsStrategy<TStrategy>(this IServiceCollection services, Func<IServiceProvider, TStrategy> factory)
        where TStrategy : class, IInsightsStrategy => services.AddScoped<IInsightsStrategy>(factory);

    /// <summary>
    /// Registers multiple custom insights strategy implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="strategyTypes">The strategy implementation types.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddInsightsStrategies(this IServiceCollection services, params Type[] strategyTypes)
    {
        foreach (var strategyType in strategyTypes)
        {
            if (!typeof(IInsightsStrategy).IsAssignableFrom(strategyType))
            {
                throw new ArgumentException($@"Type {strategyType.Name} must implement IInsightsStrategy", nameof(strategyTypes));
            }

            services.AddScoped(typeof(IInsightsStrategy), strategyType);
        }

        return services;
    }
}
