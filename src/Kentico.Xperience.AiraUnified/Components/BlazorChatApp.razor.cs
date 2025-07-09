using Microsoft.AspNetCore.Components;

namespace Kentico.Xperience.AiraUnified.Components;

public partial class BlazorChatApp : ComponentBase
{
    [Parameter] public int ThreadId { get; set; }
    [Parameter] public string ThreadName { get; set; } = string.Empty;
    [Parameter] public int UserId { get; set; }
    [Parameter] public string LogoImgRelativePath { get; set; } = string.Empty;
    [Parameter] public string BaseUrl { get; set; } = string.Empty;
}
