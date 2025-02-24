using CMS.Base;
using CMS.Core;

using Kentico.Membership;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.AssetUploader.Models;
using Kentico.Xperience.AiraUnified.Authentication;
using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.NavBar;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.AiraUnified;

/// <summary>
/// The main controller exposing the PWA.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraUnifiedController : Controller
{
    private readonly AdminUserManager adminUserManager;
    private readonly IAiraUnifiedConfigurationService airaUnifiedConfigurationService;
    private readonly IAiraUnifiedChatService airaUnifiedChatService;
    private readonly IAiraUnifiedAssetService airaUnifiedAssetService;
    private readonly INavigationService navBarService;
    private readonly IEventLogService eventLogService;

    public AiraUnifiedController(
        AdminUserManager adminUserManager,
        IAiraUnifiedConfigurationService airaUnifiedConfigurationService,
        IAiraUnifiedAssetService airaUnifiedAssetService,
        INavigationService navBarService,
        IAiraUnifiedChatService airaUnifiedChatService,
        IEventLogService eventLogService)
    {
        this.adminUserManager = adminUserManager;
        this.airaUnifiedConfigurationService = airaUnifiedConfigurationService;
        this.airaUnifiedAssetService = airaUnifiedAssetService;
        this.airaUnifiedChatService = airaUnifiedChatService;
        this.navBarService = navBarService;
        this.eventLogService = eventLogService;
    }

    /// <summary>
    /// Endpoint exposing access to the Chat page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var airaUnifiedPathBase = await GetAiraPathBase();

        var configuration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();
        var logoUrl = airaUnifiedAssetService.GetSanitizedLogoUrl(configuration);

        var chatModel = new ChatViewModel
        {
            PathBase = airaUnifiedPathBase,
            AIIconImagePath = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureStarImgPath}",
            RemovePromptUrl = AiraUnifiedConstants.RemoveUsedPromptGroupRelativeUrl,
            ServicePageViewModel = new ServicePageViewModel()
            {
                ChatAiraIconUrl = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureStarImgPath}",
                ChatUnavailableIconUrl = $"/{AiraUnifiedConstants.RCLUrlPrefix}/{AiraUnifiedConstants.PictureChatBotSmileBubbleOrangeImgPath}",
                ChatUnavailableMainMessage = Resource.ServicePageChatUnavailable,
                ChatUnavailableTryAgainMessage = Resource.ServicePageChatTryAgainLater
            },
            NavigationUrl = AiraUnifiedConstants.NavigationUrl,
            NavigationPageIdentifier = AiraUnifiedConstants.ChatRelativeUrl,
            HistoryUrl = $"{AiraUnifiedConstants.ChatRelativeUrl}/{AiraUnifiedConstants.ChatHistoryUrl}",
            ChatUrl = $"{AiraUnifiedConstants.ChatRelativeUrl}/{AiraUnifiedConstants.ChatMessageUrl}",
            LogoImgRelativePath = logoUrl
        };

        return View("~/Chat/Chat.cshtml", chatModel);
    }

    /// <summary>
    /// Retrieves the navigation view model.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Navigation([FromBody] NavBarRequestModel request)
    {
        if (!string.Equals(request.PageIdentifier, AiraUnifiedConstants.ChatRelativeUrl) &&
            !string.Equals(request.PageIdentifier, AiraUnifiedConstants.SmartUploadRelativeUrl))
        {
            return BadRequest("The navigation menu for the specified page does not exist.");
        }

        var model = await navBarService.GetNavBarViewModel(request.PageIdentifier, baseUrl: HttpContext.Request.GetBaseUrl());

        if (model.ChatItem.Url is null)
        {
            eventLogService.LogError(nameof(INavigationService), nameof(INavigationService.GetNavBarViewModel), $"Invalid chat page url.");
            return BadRequest("Invalid chat page url.");
        }

        if (model.SmartUploadItem.Url is null)
        {
            //var airaPathBase = 

            //eventLogService.LogError(nameof(INavigationService), nameof(INavigationService.GetNavBarViewModel), $"Invalid smart upload page url resulting from .");
            return BadRequest("Invalid smart upload page url.");
        }

        return Ok(model);
    }

    /// <summary>
    /// Endpoint exposing the user's chat history.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetChatHistory()
    {
        var user = await adminUserManager.GetUserAsync(User);

        // User can not be null, because he is already checked in the AiraUnifiedEndpointDataSource middleware.
        var history = await airaUnifiedChatService.GetUserChatHistory(user!.UserID);

        if (history.Count == 0)
        {
            history = [
                new AiraUnifiedChatMessageViewModel
                {
                    Message = Resource.InitialAiraMessageIntroduction,
                    Role = AiraUnifiedConstants.AiraUnifiedChatRoleName
                },
                new AiraUnifiedChatMessageViewModel
                {
                    Message = Resource.InitialAiraMessagePromptExplanation,
                    Role = AiraUnifiedConstants.AiraUnifiedChatRoleName
                }
            ];

            return Ok(history);
        }

        history.Add(new AiraUnifiedChatMessageViewModel
        {
            Message = Resource.WelcomeBackAiraMessage,
            Role = AiraUnifiedConstants.AiraUnifiedChatRoleName
        });

        return Ok(history);
    }

    /// <summary>
    /// Endpoint allowing chat communication via the chat interface.
    /// </summary>
    /// <param name="request">The <see cref="IFormCollection"/> including the chat message.</param>
    [HttpPost]
    public async Task<IActionResult> PostChatMessage(IFormCollection request)
    {
        var user = await adminUserManager.GetUserAsync(User);

        string? message;

#pragma warning disable IDE0079 // Kept for development. This will be restored in subsequent versions.
#pragma warning disable S6932 // Kept for development. This will be restored in subsequent versions.
        if (request.TryGetValue("message", out var messages))
        {
            message = messages.ToString().Replace("\"", "");
        }
#pragma warning restore S6932 //
#pragma warning restore IDE0079 //
        else
        {
            return Ok();
        }
        if (string.IsNullOrEmpty(message))
        {
            return Ok();
        }

        AiraUnifiedChatMessageViewModel result;

        // User can not be null, because he is already checked in the AiraUnifiedEndpointDataSource middleware
        airaUnifiedChatService.SaveMessage(message, user!.UserID, AiraUnifiedConstants.UserChatRoleName);

        try
        {
            var aiResponse = await airaUnifiedChatService.GetAIResponseOrNull(message, numberOfIncludedHistoryMessages: 5, user.UserID);

            if (aiResponse is null)
            {
                result = new AiraUnifiedChatMessageViewModel
                {
                    ServiceUnavailable = true,
                };

                return Ok(result);
            }

            airaUnifiedChatService.SaveMessage(aiResponse.Response, user.UserID, AiraUnifiedConstants.AiraUnifiedChatRoleName);

            airaUnifiedChatService.UpdateChatSummary(user.UserID, message);

            result = new AiraUnifiedChatMessageViewModel
            {
                Role = AiraUnifiedConstants.AiraUnifiedChatRoleName,
                Message = aiResponse.Response
            };

            if (aiResponse.SuggestedQuestions is not null)
            {
                var promptGroup = airaUnifiedChatService.SaveAiraPrompts(user.UserID, aiResponse.SuggestedQuestions);
                result.QuickPrompts = promptGroup.QuickPrompts;
                result.QuickPromptsGroupId = promptGroup.QuickPromptsGroupId.ToString();
            }
        }
        catch (Exception ex)
        {
            result = new AiraUnifiedChatMessageViewModel
            {
                Role = AiraUnifiedConstants.AiraUnifiedChatRoleName,
                ServiceUnavailable = true,
                Message = $"Error: {ex.Message}"
            };
        }

        return Ok(result);
    }

    /// <summary>
    /// Endpoint allowing removal of a used suggested prompt group.
    /// </summary>
    /// <param name="model">The <see cref="AiraUnifiedUsedPromptGroupModel"/> with the information about the prompt group.</param>
    [HttpPost]
    public IActionResult RemoveUsedPromptGroup([FromBody] AiraUnifiedUsedPromptGroupModel model)
    {
        airaUnifiedChatService.RemoveUsedPrompts(model.GroupId);

        return Ok();
    }

    /// <summary>
    /// Endpoint allowing upload of the files via smart upload.
    /// </summary>
    /// <param name="request">The <see cref="IFormCollection"/> request containing the uploaded files.</param>
    [HttpPost]
    public async Task<IActionResult> PostImages(IFormCollection request)
    {
        var user = await adminUserManager.GetUserAsync(User);

        // User can not be null, because he is already checked in the AiraUnifiedEndpointDataSource middleware
        var uploadSuccessful = await airaUnifiedAssetService.HandleFileUpload(request.Files, user!.UserID);

        if (uploadSuccessful)
        {
            return Ok();
        }

        return BadRequest("Attempted to upload file with forbidden format.");
    }

    /// <summary>
    /// Endpoint allowing accessing the smart upload page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Assets()
    {
        var airaUnifiedPathBase = await GetAiraPathBase();

        var model = new AssetsViewModel
        {
            PathBase = airaUnifiedPathBase,
            AllowedFileExtensionsUrl = $"{AiraUnifiedConstants.SmartUploadRelativeUrl}/{AiraUnifiedConstants.SmartUploadAllowedFileExtensionsUrl}",
            SelectFilesButton = Resource.SmartUploadSelectFilesButton,
            FilesUploadedMessage = Resource.SmartUploadFilesUploadedMessage,
            NavigationPageIdentifier = AiraUnifiedConstants.SmartUploadRelativeUrl,
            NavigationUrl = AiraUnifiedConstants.NavigationUrl
        };

        return View("~/AssetUploader/Assets.cshtml", model);
    }

    /// <summary>
    /// Endpoint retrieving the allowed smart upload file extensions.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllowedFileExtensions()
    {
        var allowedExtensions = await airaUnifiedAssetService.GetAllowedFileExtensions();

        return Ok(allowedExtensions);
    }

    /// <summary>
    /// Endpoint exposing the manifest file for the PWA.
    /// </summary>
    [HttpGet($"/{AiraUnifiedConstants.RCLUrlPrefix}/manifest.json")]
    [Produces("application/json")]
    public async Task<IActionResult> GetPwaManifest()
    {
        var configuration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();

        var libraryBasePath = '/' + AiraUnifiedConstants.RCLUrlPrefix;

        var manifest = new
        {
            name = "AiraUnified",
            short_name = "AiraUnified",
            start_url = $"{configuration.AiraUnifiedConfigurationItemAiraPathBase}/{AiraUnifiedConstants.ChatRelativeUrl}",
            display = "standalone",
            background_color = "#ffffff",
            theme_color = "#ffffff",
            scope = "/",
            icons = new[]
            {
                new
                {
                    src = $"{libraryBasePath}/img/favicon/android-chrome-192x192.png",
                    sizes = "192x192",
                    type = "image/png"
                },
                new
                {
                    src = $"{libraryBasePath}/img/favicon/android-chrome-512x512.png",
                    sizes = "512x512",
                    type = "image/png"
                }
            }
        };

        return Json(manifest);
    }

    /// <summary>
    /// Endpoint retrieving the SignIn page.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn()
    {
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();

        var model = new SignInViewModel
        {
            PathBase = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase,
            ChatRelativeUrl = AiraUnifiedConstants.ChatRelativeUrl,
        };

        return View("~/Authentication/SignIn.cshtml", model);
    }

    private async Task<string> GetAiraPathBase()
    {
        var configuration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();

        return configuration.AiraUnifiedConfigurationItemAiraPathBase;
    }
}
