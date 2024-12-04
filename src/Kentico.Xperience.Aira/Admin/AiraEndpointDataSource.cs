using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

using System.Collections.Generic;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Kentico.Xperience.Aira.Admin;

//internal class AiraEndpointDataSource : MutableEndpointDataSource
//{
//    public AiraEndpointDataSource() => SetEndpoints(MakeEndpoints());

//    private IReadOnlyList<Endpoint> MakeEndpoints()
//    {
//        var endpoints = new List<Endpoint>();

//        return new[]
//        {
//            CreateEndpoint("/setEndpoint/{**route}", async context => {
//                SetEndpoints(MakeEndpoints(context.Request.RouteValues["route"].ToString()));
//            })
//        };
//​    }
//    private static Endpoint CreateEndpoint(string pattern, RequestDelegate requestDelegate) =>
//        new RouteEndpointBuilder(
//            requestDelegate: requestDelegate,
//            routePattern: RoutePatternFactory.Parse(pattern),
//            order: 0)
//        .Build();
//}

internal abstract class MutableEndpointDataSource : EndpointDataSource
{
    private readonly object endpointLock = new();
​
    private IReadOnlyList<Endpoint> endpoints = [];
​
    private CancellationTokenSource cancellationTokenSource;
​
    private IChangeToken changeToken;

    protected MutableEndpointDataSource(IReadOnlyList<Endpoint> endpoints, CancellationTokenSource cancellationTokenSource, IChangeToken changeToken)
    {
        SetEndpoints(endpoints);
        this.endpoints = endpoints;
        this.cancellationTokenSource = cancellationTokenSource;
        this.changeToken = changeToken;
    }
​
    public override IChangeToken GetChangeToken() => changeToken;
​
    public override IReadOnlyList<Endpoint> Endpoints => endpoints;
​
    public void SetEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        lock (endpointLock)
        {
            var oldCancellationTokenSource = cancellationTokenSource;
​
            this.endpoints = endpoints;
​
            cancellationTokenSource = new CancellationTokenSource();
            changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
​
            oldCancellationTokenSource?.Cancel();
        }
    }
}
