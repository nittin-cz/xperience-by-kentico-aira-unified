using CMS.DataEngine;

using HotChocolate.Authorization;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Chat.Models;
using Kentico.Xperience.Aira.NavBar;
using Htmx;

using Kentico.Membership;
using Kentico.Xperience.Aira.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Kentico.Xperience.Aira;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager
) : Controller
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
    public async Task<IActionResult> Index() =>
        View("~/Views/Chat.cshtml");

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Signin() => View("~/Authentication/_SignIn.cshtml");

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SignIn([FromForm] SignInViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("~/Authentication/_SignIn.cshtml", model);
        }

        var signInResult = SignInResult.Failed;
        try
        {
            var member = await GetMember();

            if (member is null)
            {
                signInResult = SignInResult.Failed;
            }
            else if (!member.Enabled)
            {
                return PartialView("~/Authentication/_SignIn.cshtml", model);
            }
            else
            {
                signInResult = await signInManager.PasswordSignInAsync(member.UserName!, model.Password, isPersistent: true, lockoutOnFailure: false);
            }
        }
        catch (Exception ex)
        {
            signInResult = SignInResult.Failed;
        }

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Your sign-in attempt was not successful. Please try again.");

            return PartialView("~/Authentication/_SignIn.cshtml", model);
        }

        string redirectUrl = $"{Request.PathBase}/aira/chat";

        Response.Htmx(h => h.Redirect(redirectUrl));

        return Request.IsHtmx()
        ? Ok()
        : Redirect(redirectUrl);

        async Task<ApplicationUser?> GetMember()
        {
            var member = await userManager.FindByNameAsync(model.UserNameOrEmail);

            if (member is not null)
            {
                return member;
            }

            return await userManager.FindByEmailAsync(model.UserNameOrEmail);
        }
    }
}
