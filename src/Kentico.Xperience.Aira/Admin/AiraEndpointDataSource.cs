using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin.InfoModels;

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Kentico.Xperience.Aira.Admin;

internal class AiraEndpointDataSource : MutableEndpointDataSource
{
    private readonly IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider;

    public AiraEndpointDataSource(IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider)
        : base(new CancellationTokenSource(), new CancellationChangeToken(new CancellationToken()))
        => this.airaConfigurationProvider = airaConfigurationProvider;

    public void UpdateEndpoints()
        => SetEndpoints(MakeEndpoints());

    private IReadOnlyList<Endpoint> MakeEndpoints()
    {
        var configuration = airaConfigurationProvider.Get().GetEnumerableTypedResult().SingleOrDefault();

        if (configuration is null)
        {
            return [];
        }

        if (string.IsNullOrEmpty(configuration.AiraConfigurationItemAiraPathBase))
        {
            return [];
        }

        return
        [
            CreateEndpoint($"{configuration.AiraConfigurationItemAiraPathBase}/index", async context => {
                var routeData = new RouteData();
                routeData.Values["controller"] = "AiraCompanionApp";
                routeData.Values["action"] = "Index";

                var actionDescriptor = new ControllerActionDescriptor
                {
                    ControllerName = "AiraCompanionApp",
                    ActionName = "Index",
                    ControllerTypeInfo = typeof(AiraCompanionAppController).GetTypeInfo()
                };

                var actionContext = new ActionContext(context, routeData, actionDescriptor);
                var controllerContext = new ControllerContext(actionContext);
                var controllerFactory = context.RequestServices.GetRequiredService<IControllerFactory>();
                object controller = controllerFactory.CreateController(controllerContext);

                if (controller is AiraCompanionAppController airaController)
                {
                    airaController.ControllerContext = controllerContext;
                    airaController.ControllerContext.HttpContext = context;

                    var result = await airaController.Index();

                    await result.ExecuteResultAsync(controllerContext);
                }
            })
        ];
    }
    private static Endpoint CreateEndpoint(string pattern, RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
            requestDelegate: requestDelegate,
            routePattern: RoutePatternFactory.Parse(pattern),
            order: 0)
        .Build();
}

internal abstract class MutableEndpointDataSource : EndpointDataSource
{
    private readonly object endpointLock = new();
    private IReadOnlyList<Endpoint> endpoints = [];
    private CancellationTokenSource cancellationTokenSource;
    private IChangeToken changeToken;
    protected MutableEndpointDataSource(CancellationTokenSource cancellationTokenSource, IChangeToken changeToken)
    {
        SetEndpoints(endpoints);
        this.cancellationTokenSource = cancellationTokenSource;
        this.changeToken = changeToken;
    }
    public override IChangeToken GetChangeToken() => changeToken;
    public override IReadOnlyList<Endpoint> Endpoints => endpoints;
    protected void SetEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        lock (endpointLock)
        {
            var oldCancellationTokenSource = cancellationTokenSource;
            this.endpoints = endpoints;
            cancellationTokenSource = new CancellationTokenSource();
            changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
            oldCancellationTokenSource?.Cancel();
        }
    }
}
