using System.Reflection;
using System.Text.Json;

using CMS.DataEngine;
using CMS.Membership;

using Kentico.Membership;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.NavBar.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Represents a data source for Aira Unified endpoints.
/// </summary>
internal sealed class AiraUnifiedEndpointDataSource : MutableEndpointDataSource
{
    private readonly IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider;


    private const string ApplicationJsonIdentifier = "application/json";
    private const string MultipartFormDataIdentifier = "multipart/form-data";
    private const string InvalidOrMissingRequestBodyMessage = "Invalid or missing request body.";
    private const string MissingContentTypeMessage = "Missing Content-Type header.";
    private const string ExpectedApplicationJsonOrMultipartFormData = "Unsupported content type. Expected 'application/json' or 'multipart/form-data'.";
    private const string EmptyRequestBody = "Empty request body.";
    private const string HttpRequired = "HTTPS is required.";
    private const string ExpectedApplicationJsonMessage = "Unsupported content type. Expected 'application/json'.";



    /// <summary>
    /// Initializes a new instance of the AiraUnifiedEndpointDataSource class.
    /// </summary>
    /// <param name="airaUnifiedConfigurationProvider">The provider for Aira Unified configuration items.</param>
    public AiraUnifiedEndpointDataSource(IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider)
        : base(new CancellationTokenSource(), new CancellationChangeToken(CancellationToken.None))
        => this.airaUnifiedConfigurationProvider = airaUnifiedConfigurationProvider;


    /// <summary>
    /// Updates the endpoints.
    /// </summary>
    public void UpdateEndpoints()
        => SetEndpoints(MakeEndpoints());


    /// <summary>
    /// Creates the endpoints.
    /// </summary>
    /// <returns>The endpoints.</returns>
    private IReadOnlyList<Endpoint> MakeEndpoints()
    {
        var configuration = airaUnifiedConfigurationProvider.Get().GetEnumerableTypedResult().SingleOrDefault();

        if (configuration is null)
        {
            return [];
        }

        if (string.IsNullOrEmpty(configuration.AiraUnifiedConfigurationItemAiraPathBase))
        {
            return [];
        }

        return
        [
            CreateAiraEndpointWithQueryParams(configuration,
                AiraUnifiedConstants.ChatRelativeUrl,
                nameof(AiraUnifiedController.BlazorChat),
                AiraUnifiedConstants.ChatThreadIdParameterName,
                (controller, threadId) => controller.BlazorChat(threadId),
                requiredPermission: SystemPermissions.VIEW
            ),
            // CreateAiraEndpointWithQueryParams(configuration,
            //     AiraUnifiedConstants.ChatRelativeUrl,
            //     nameof(AiraUnifiedController.Index),
            //     AiraUnifiedConstants.ChatThreadIdParameterName,
            //     (controller, threadId) => controller.Index(threadId),
            //     requiredPermission: SystemPermissions.VIEW
            // ),
            CreateAiraEndpointWithRouteValue(configuration,
                $"{AiraUnifiedConstants.ChatRelativeUrl}/{AiraUnifiedConstants.ChatHistoryUrl}/{{{AiraUnifiedConstants.ChatThreadIdParameterName}:int}}",
                nameof(AiraUnifiedController.GetOrCreateChatHistory),
                AiraUnifiedConstants.ChatThreadIdParameterName,
                (controller, threadId) => controller.GetOrCreateChatHistory(threadId),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraEndpoint(configuration,
                AiraUnifiedConstants.NewChatThreadRelativeUrl,
                nameof(AiraUnifiedController.NewChatThread),
                controller => controller.NewChatThread(),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraEndpoint(configuration,
                $"{AiraUnifiedConstants.ChatThreadSelectorRelativeUrl}/{AiraUnifiedConstants.AllChatThreadsRelativeUrl}",
                nameof(AiraUnifiedController.GetChatThreads),
                controller => controller.GetChatThreads(),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraEndpoint(configuration,
                AiraUnifiedConstants.ChatThreadSelectorRelativeUrl,
                nameof(AiraUnifiedController.ChatThreadSelector),
                controller => controller.ChatThreadSelector(),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraEndpointFromBody<NavBarRequestModel>(configuration,
                AiraUnifiedConstants.NavigationUrl,
                nameof(AiraUnifiedController.Navigation),
                (controller, model) => controller.Navigation(model),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraEndpointFromIFormCollectionWithRouteValue(configuration,
                $"{AiraUnifiedConstants.ChatRelativeUrl}/{AiraUnifiedConstants.ChatMessageUrl}/{{{AiraUnifiedConstants.ChatThreadIdParameterName}:int}}",
                nameof(AiraUnifiedController.PostChatMessage),
                AiraUnifiedConstants.ChatThreadIdParameterName,
                (controller, request, threadId) => controller.PostChatMessage(request, threadId),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraIFormCollectionEndpoint(configuration,
                 $"{AiraUnifiedConstants.SmartUploadRelativeUrl}/{AiraUnifiedConstants.SmartUploadUploadUrl}",
                nameof(AiraUnifiedController.PostImages),
                (controller, request) => controller.PostImages(request),
                requiredPermission: SystemPermissions.CREATE
            ),
            CreateAiraEndpoint(configuration,
                AiraUnifiedConstants.SmartUploadRelativeUrl,
                nameof(AiraUnifiedController.Assets),
                controller => controller.Assets(),
                requiredPermission: SystemPermissions.CREATE
            ),
            CreateAiraEndpointWithConditionalRedirect(configuration,
                AiraUnifiedConstants.SigninRelativeUrl,
                nameof(AiraUnifiedController.SignIn),
                (controller) => controller.SignIn(),
                permissionToRedirectedEndpoint: SystemPermissions.VIEW,
                redirectUrl: AiraUnifiedConstants.ChatRelativeUrl
            ),
            CreateAiraEndpointWithConditionalRedirect(configuration,
                subPath: string.Empty,
                nameof(AiraUnifiedController.SignIn),
                (controller) => controller.SignIn(),
                permissionToRedirectedEndpoint: SystemPermissions.VIEW,
                redirectUrl: AiraUnifiedConstants.ChatRelativeUrl
            ),
            CreateAiraEndpointFromBody<AiraUnifiedUsedPromptGroupModel>(configuration,
                AiraUnifiedConstants.RemoveUsedPromptGroupRelativeUrl,
                nameof(AiraUnifiedController.RemoveUsedPromptGroup),
                (controller, model) => controller.RemoveUsedPromptGroup(model),
                requiredPermission: SystemPermissions.VIEW
            ),
            CreateAiraEndpoint(configuration,
                $"{AiraUnifiedConstants.SmartUploadRelativeUrl}/{AiraUnifiedConstants.SmartUploadAllowedFileExtensionsUrl}",
                nameof(AiraUnifiedController.GetAllowedFileExtensions),
                controller => controller.GetAllowedFileExtensions(),
                requiredPermission: SystemPermissions.CREATE
            )
        ];
    }


    private static Endpoint CreateAiraEndpointWithQueryParams(AiraUnifiedConfigurationItemInfo configurationInfo, string subPath, string actionName, string actionParameterName, Func<AiraUnifiedController, int?, Task<IActionResult>> action, string? requiredPermission = null)
    => CreateEndpoint($"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await ValidateRequestXorSetRedirect(context, configurationInfo, requiredPermission))
        {
            return;
        }

        IActionResult? result;

        if (!context.Request.Query.ContainsKey(actionParameterName) || !int.TryParse(context.Request.Query[actionParameterName], out var val1))
        {
            result = await action.Invoke(airaController, null);
            await result.ExecuteResultAsync(airaController.ControllerContext);

            return;
        }

        result = await action.Invoke(airaController, val1);

        await result.ExecuteResultAsync(airaController.ControllerContext);
    });


    private static Endpoint CreateAiraEndpointWithRouteValue(AiraUnifiedConfigurationItemInfo configurationInfo, string subPath, string actionName, string actionParameterName, Func<AiraUnifiedController, int, Task<IActionResult>> action, string? requiredPermission = null)
    => CreateEndpoint($"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await ValidateRequestXorSetRedirect(context, configurationInfo, requiredPermission))
        {
            return;
        }

        if (!context.Request.RouteValues.TryGetValue(actionParameterName, out var actionParameterStringValue) ||
            !int.TryParse(actionParameterStringValue?.ToString(), out var actionParameterIntValue))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Missing {actionParameterName}");
            return;
        }

        if (!await CheckHttps(context) ||
            (requiredPermission is not null && !await AuthorizeOrSetRedirectToSignIn(context, configurationInfo.AiraUnifiedConfigurationItemAiraPathBase, requiredPermission))
        )
        {
            return;
        }

        var result = await action.Invoke(airaUnifiedController, actionParameterIntValue);
        await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
    });


    private static Endpoint CreateAiraEndpointFromIFormCollectionWithRouteValue(AiraUnifiedConfigurationItemInfo configurationInfo, string subPath, string actionName, string actionParameterName, Func<AiraUnifiedController, IFormCollection, int, Task<IActionResult>> action, string? requiredPermission = null)
    => CreateEndpoint($"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await ValidateRequestXorSetRedirect(context, configurationInfo, requiredPermission))
        {
            return;
        }

        if (!context.Request.RouteValues.TryGetValue(actionParameterName, out var actionParameterStringValue) ||
            !int.TryParse(actionParameterStringValue?.ToString(), out var actionParameterIntValue))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Missing {actionParameterName}");
            return;
        }

        if (context.Request.ContentType is null)
        {
            return;
        }

        if (context.Request.ContentType.Contains(MultipartFormDataIdentifier))
        {
            var requestObject = await context.Request.ReadFormAsync();
            var result = await action.Invoke(airaController, requestObject, actionParameterIntValue);
            await result.ExecuteResultAsync(airaController.ControllerContext);
        }
        else if (string.Equals(context.Request.ContentType, ApplicationJsonIdentifier, StringComparison.OrdinalIgnoreCase))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            var formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                { "message", body }
            });

            var result = await action.Invoke(airaController, formCollection, actionParameterIntValue);
            await result.ExecuteResultAsync(airaController.ControllerContext);
        }
    });


    private static Endpoint CreateAiraEndpointFromBody<T>(
       AiraUnifiedConfigurationItemInfo configurationInfo,
       string subPath,
       string actionName,
       Func<AiraUnifiedController, T, IActionResult> actionWithModel,
       string? requiredPermission = null
    ) where T : class, new()
    => CreateEndpoint($"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await ValidateRequestXorSetRedirect(context, configurationInfo, requiredPermission))
        {
            return;
        }

        if (context.Request.ContentType is not null &&
            string.Equals(context.Request.ContentType, ApplicationJsonIdentifier, StringComparison.OrdinalIgnoreCase))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            try
            {
                var requestObject = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (requestObject is not null)
                {
                    var result = actionWithModel.Invoke(airaUnifiedController, requestObject);
                    await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(InvalidOrMissingRequestBodyMessage);
                }
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization errors
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Invalid JSON format: {ex.Message}");
            }
        }
        else
        {
            // Handle unsupported content types
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsync(ExpectedApplicationJsonMessage);
        }
    });


    private static Endpoint CreateAiraEndpointFromBody<T>(
        AiraUnifiedConfigurationItemInfo configurationInfo,
        string subPath,
        string actionName,
        Func<AiraUnifiedController, T, Task<IActionResult>> actionWithModel,
        string? requiredPermission = null
    ) where T : class, new()
    => CreateEndpoint($"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await ValidateRequestXorSetRedirect(context, configurationInfo, requiredPermission))
        {
            return;
        }

        if (context.Request.ContentType is null ||
            !string.Equals(context.Request.ContentType, ApplicationJsonIdentifier, StringComparison.OrdinalIgnoreCase))
        {
            // Handle unsupported content types
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsync(ExpectedApplicationJsonMessage);
            return;
        }

        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();

        try
        {
            var requestObject = JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (requestObject is not null)
            {
                var result = await actionWithModel.Invoke(airaUnifiedController, requestObject);
                await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(InvalidOrMissingRequestBodyMessage);
            }
        }
        catch (JsonException ex)
        {
            // Handle JSON deserialization errors
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Invalid JSON format: {ex.Message}");
        }
    });


    private static Endpoint CreateAiraEndpoint(AiraUnifiedConfigurationItemInfo configurationInfo,
        string subPath,
        string actionName,
        Func<AiraUnifiedController, Task<IActionResult>> action,
        string? requiredPermission = null)
    => CreateEndpoint($"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await ValidateRequestXorSetRedirect(context, configurationInfo, requiredPermission))
        {
            return;
        }

        var result = await action.Invoke(airaUnifiedController);
        await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
    });


    private static Endpoint CreateAiraEndpointWithConditionalRedirect(AiraUnifiedConfigurationItemInfo configurationInfo,
        string subPath,
        string actionName,
        Func<AiraUnifiedController, Task<IActionResult>> action,
        string permissionToRedirectedEndpoint,
        string redirectUrl)
    {
        var path = string.Equals(subPath, string.Empty) ? $"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}"
            : $"{configurationInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}";

        return CreateEndpoint(path, async context =>
        {
            var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

            if (!await CheckHttps(context) ||
                await SetRedirectIfAuthorized(context, configurationInfo.AiraUnifiedConfigurationItemAiraPathBase, permissionToRedirectedEndpoint, redirectUrl))
            {
                return;
            }

            var result = await action.Invoke(airaUnifiedController);
            await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
        });
    }


    private static Endpoint CreateAiraIFormCollectionEndpoint(AiraUnifiedConfigurationItemInfo configurationItemInfo,
        string subPath,
        string actionName,
        Func<AiraUnifiedController, IFormCollection, Task<IActionResult>> action,
        string? requiredPermission = null)
    => CreateEndpoint($"{configurationItemInfo.AiraUnifiedConfigurationItemAiraPathBase}/{subPath}", async context =>
    {
        var airaUnifiedController = await GetAiraUnifiedControllerInContext(context, actionName);

        if (!await ValidateRequestXorSetRedirect(context, configurationItemInfo, requiredPermission))
        {
            return;
        }

        if (context.Request.ContentType is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(MissingContentTypeMessage);
            return;
        }

        if (context.Request.ContentType.Contains(MultipartFormDataIdentifier))
        {
            var requestObject = await context.Request.ReadFormAsync();
            var result = await action.Invoke(airaUnifiedController, requestObject);
            await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
        }
        else if (string.Equals(context.Request.ContentType, ApplicationJsonIdentifier))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(EmptyRequestBody);
                return;
            }

            var formCollection = new FormCollection(new Dictionary<string, StringValues>
            {
                { "message", body }
            });

            var result = await action.Invoke(airaUnifiedController, formCollection);
            await result.ExecuteResultAsync(airaUnifiedController.ControllerContext);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsync(ExpectedApplicationJsonOrMultipartFormData);
        }
    });


    private static async Task<AiraUnifiedController> GetAiraUnifiedControllerInContext(HttpContext context, string actionName)
    {
        var controllerShortName = nameof(AiraUnifiedController).Replace("Controller", string.Empty);

        var routeData = new RouteData { Values = { ["controller"] = controllerShortName, ["action"] = actionName } };

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = controllerShortName,
            ActionName = actionName,
            ControllerTypeInfo = typeof(AiraUnifiedController).GetTypeInfo()
        };

        await AuthenticateAiraEndpoint(context);

        var actionContext = new ActionContext(context, routeData, actionDescriptor);
        var controllerContext = new ControllerContext(actionContext);
        var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
        var airaUnifiedController = (AiraUnifiedController)controllerFactory.CreateController(controllerContext);

        airaUnifiedController.ControllerContext = controllerContext;
        airaUnifiedController.ControllerContext.HttpContext = context;

        return airaUnifiedController;
    }


    private static async Task AuthenticateAiraEndpoint(HttpContext context)
    {
        if (context.User.Identity is null || !context.User.Identity.IsAuthenticated)
        {
            var authenticateResult = await context.RequestServices
                .GetRequiredService<IAuthenticationService>()
                .AuthenticateAsync(context, AiraUnifiedConstants.XperienceAdminSchemeName);

            if (authenticateResult is { Succeeded: true, Principal: not null })
            {
                context.User = authenticateResult.Principal;
            }
        }
    }


    private static async Task<bool> CheckHttps(HttpContext context)
    {
        if (!context.Request.IsHttps)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(HttpRequired);
            return false;
        }

        context.Response.Headers.XFrameOptions = "SAMEORIGIN";
        context.Response.Headers.ContentSecurityPolicy = "frame-ancestors 'self'";
        context.Response.Headers.StrictTransportSecurity = "max-age=31536000; includeSubDomains; preload";

        return true;
    }


    private static async Task<bool> ValidateRequestXorSetRedirect(HttpContext context, AiraUnifiedConfigurationItemInfo configurationInfo, string? requiredPermission = null)
    => await CheckHttps(context)
      && (requiredPermission is null || await AuthorizeOrSetRedirectToSignIn(context, configurationInfo.AiraUnifiedConfigurationItemAiraPathBase, requiredPermission));


    private static Endpoint CreateEndpoint(string pattern, RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
            requestDelegate: requestDelegate,
            routePattern: RoutePatternFactory.Parse(pattern),
            order: 0)
        .Build();


    private static async Task<bool> AuthorizeOrSetRedirectToSignIn(HttpContext context, string airaUnifiedPathBase, string permission)
    {
        var adminUserManager = context.RequestServices.GetRequiredService<AdminUserManager>();
        var airaUnifiedAssetService = context.RequestServices.GetRequiredService<IAiraUnifiedAssetService>();
        var userProvider = context.RequestServices.GetRequiredService<IInfoProvider<UserInfo>>();

        var user = await adminUserManager.GetUserAsync(context.User);
        var signInRedirectUrl = $"{airaUnifiedPathBase}/{AiraUnifiedConstants.SigninRelativeUrl}";

        if (user is null || !userProvider.Get().WhereEquals(nameof(UserInfo.UserGUID), user.UserGUID).Any())
        {
            context.Response.Redirect($"{signInRedirectUrl}?{AiraUnifiedConstants.SigninMissingPermissionParameterName}={permission}");
            return false;
        }

        var hasAiraViewPermission = await airaUnifiedAssetService.DoesUserHaveAiraUnifiedPermission(permission, user.UserID) || user.IsAdministrator();

        if (hasAiraViewPermission)
        {
            return true;
        }

        context.Response.Redirect(signInRedirectUrl);

        return false;
    }


    private static async Task<bool> SetRedirectIfAuthorized(HttpContext context, string airaUnifiedPathBase, string permission, string redirectSubPath)
    {
        var adminUserManager = context.RequestServices.GetRequiredService<AdminUserManager>();
        var airaUnifiedAssetService = context.RequestServices.GetRequiredService<IAiraUnifiedAssetService>();
        var userProvider = context.RequestServices.GetRequiredService<IInfoProvider<UserInfo>>();

        var user = await adminUserManager.GetUserAsync(context.User);

        var fullRedirectUrl = $"{airaUnifiedPathBase}/{redirectSubPath}";

        if (user is null
            || !userProvider.Get().WhereEquals(nameof(UserInfo.UserGUID), user.UserGUID).Any()
            || !(await airaUnifiedAssetService.DoesUserHaveAiraUnifiedPermission(permission, user.UserID) || user.IsAdministrator()))
        {
            return false;
        }

        context.Response.Redirect(fullRedirectUrl);

        return true;
    }
}
