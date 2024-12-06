using CMS.DataEngine;

using HotChocolate.Authorization;

using Kentico.Xperience.Aira.Admin.InfoModels;

using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.Aira;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController : Controller
{
    private readonly IInfoProvider<AiraConfigurationItemInfo> airaConfigurationInfoProvider;

    public AiraCompanionAppController(IInfoProvider<AiraConfigurationItemInfo> airaConfigurationInfoProvider)
        => this.airaConfigurationInfoProvider = airaConfigurationInfoProvider;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var configuration = await airaConfigurationInfoProvider.Get().GetEnumerableTypedResultAsync();

        var chatModel = new AiraPathsModel
        {
            PathBase = configuration.First().AiraConfigurationItemAiraPathBase,
            ChatMessagePath = "chat/message"
        };

        return View("~/Views/Chat.cshtml", chatModel);
    }

    [HttpPost]
    public async Task<IActionResult> PostChatMessage([FromBody] List<AiraChatMessageModel> request) => Ok();
}

public class AiraChatMessageModel
{
    public string Role { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class AiraPathsModel
{
    public string PathBase { get; set; } = string.Empty;
    public string ChatMessagePath { get; set; } = string.Empty;
}
