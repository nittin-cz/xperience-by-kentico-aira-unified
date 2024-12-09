using CMS.DataEngine;

using HotChocolate.Authorization;

using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.NavBar;

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

        var chatModel = new ChatViewModel
        {
            PathsModel = new AiraPathsModel
            {
                PathBase = configuration.First().AiraConfigurationItemAiraPathBase,
                ChatMessagePath = "chat/message",
            },
            NavBarViewModel = new NavBarViewModel
            {
                LogoImgRelativePath = AiraCompanionAppConstants.RelativeLogoUrl,
                TitleImagePath = AiraCompanionAppConstants.RelativeChatImgUrl,
                TitleText = AiraCompanionAppConstants.ChatTitle,
                ChatItem = new MenuItemModel
                {
                    Title = AiraCompanionAppConstants.ChatTitle,
                    ImagePath = AiraCompanionAppConstants.RelativeChatImgUrl,
                    Url = AiraCompanionAppConstants.ChatRelativeUrl
                },
                SmartUploadItem = new MenuItemModel
                {
                    Title = AiraCompanionAppConstants.SmartUploadTitle,
                    ImagePath = AiraCompanionAppConstants.RelativeSmartUploadUrl,
                    Url = AiraCompanionAppConstants.ChatRelativeUrl
                }
            }
        };

        return View("~/Chat/Chat.cshtml", chatModel);
    }

    [HttpPost]
    public async Task<IActionResult> PostChatMessage([FromBody] AiraChatRequest request)
        => Ok(new AiraChatMessageModel { Role = "ai", Text = "Ok" });
}
