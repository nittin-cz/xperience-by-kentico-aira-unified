using CMS.DataEngine;
using CMS.Workspaces;

using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.AiraUnified.Admin.Providers;

/// <summary>
/// Provides a dropdown options provider for workspaces.
/// </summary>
internal sealed class WorkspaceProvider : IDropDownOptionsProvider
{
    private readonly IInfoProvider<WorkspaceInfo> workspaceProvider;


    /// <summary>
    /// Initializes a new instance of the WorkspaceProvider class.
    /// </summary>
    /// <param name="workspaceProvider">The workspace provider.</param>
    public WorkspaceProvider(IInfoProvider<WorkspaceInfo> workspaceProvider) => this.workspaceProvider = workspaceProvider;


    /// <inheritdoc />
    public async Task<IEnumerable<DropDownOptionItem>> GetOptionItems() =>
        (await workspaceProvider.Get()
        .GetEnumerableTypedResultAsync())
        .Select(x => new DropDownOptionItem
        {
            Value = x.WorkspaceName,
            Text = x.WorkspaceDisplayName
        });
}
