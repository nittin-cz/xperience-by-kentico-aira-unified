using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Represents a mutable endpoint data source that can be updated dynamically.
/// </summary>
internal abstract class MutableEndpointDataSource : EndpointDataSource
{
    private readonly object endpointLock = new();
    private IReadOnlyList<Endpoint> endpoints = [];
    private CancellationTokenSource cancellationTokenSource;
    private IChangeToken changeToken;


    /// <summary>
    /// Initializes a new instance of the MutableEndpointDataSource class.
    /// </summary>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="changeToken">The change token.</param>
    protected MutableEndpointDataSource(CancellationTokenSource cancellationTokenSource, IChangeToken changeToken)
    {
        SetEndpoints(endpoints);
        this.cancellationTokenSource = cancellationTokenSource;
        this.changeToken = changeToken;
    }


    /// <inheritdoc />
    public override IChangeToken GetChangeToken() => changeToken;


    /// <inheritdoc />
    public override IReadOnlyList<Endpoint> Endpoints => endpoints;


    /// <summary>
    /// Sets the endpoints for this data source and triggers a change notification.
    /// </summary>
    /// <param name="endpoints">The new list of endpoints to set.</param>
    /// <remarks>
    /// This method is thread-safe and will trigger a change notification to any subscribers
    /// of the change token. The old change token will be cancelled before the new one is created.
    /// </remarks>
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

