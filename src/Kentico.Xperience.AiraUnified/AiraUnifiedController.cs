using CMS.Core;

using Kentico.Membership;
using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Assets;
using Kentico.Xperience.AiraUnified.AssetUploader;
using Kentico.Xperience.AiraUnified.Authentication;
using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;
using Kentico.Xperience.AiraUnified.NavBar;
using Kentico.Xperience.AiraUnified.NavBar.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.AiraUnified;

/// <summary>
/// The main controller exposing the PWA.
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
internal sealed class AiraUnifiedController(
    AdminUserManager adminUserManager,
    IAiraUnifiedConfigurationService airaUnifiedConfigurationService,
    IAiraUnifiedAssetService airaUnifiedAssetService,
    INavigationService navigationService,
    IAiraUnifiedChatService airaUnifiedChatService,
    IEventLogService eventLogService)
    : Controller
{
    private const string InvalidPathBaseErrorMessage = "Invalid aira unified path base.";

    /// <summary>
    /// Endpoint exposing access to the Blazor Chat page.
    /// </summary>
    /// <param name="chatThreadId">The chat thread id. If not specified, a new thread will be created.</param>
    /// <returns>A Blazor component view.</returns>
    [HttpGet]
    public async Task<IActionResult> BlazorChat(int? chatThreadId = null)
    {
        var configuration = await GetConfiguration();
        var logoUrl = await airaUnifiedAssetService.GetSanitizedLogoUrl();
        var user = await adminUserManager.GetUserAsync(User);

        var chatThread = await airaUnifiedChatService.GetAiraChatThreadModel(user!.UserID, setAsLastUsed: true, chatThreadId);

        // Vrátit Blazor komponentu s parametry
        return View("~/Views/Chat/BlazorChatHost.cshtml", new BlazorChatViewModel
        {
            ThreadId = chatThread.ThreadId,
            ThreadName = chatThread.ThreadName,
            UserId = user.UserID,
            LogoImgRelativePath = logoUrl,
            BaseUrl = HttpContext.Request.GetBaseUrl()
        });
    }
    


    /// <summary>
    /// Retrieves all chat threads for the current user.
    /// </summary>
    /// <returns>A list of chat threads wrapped in <see cref="AiraUnifiedChatThreadsViewModel"/>.</returns>
    [HttpGet]
    public async Task<IActionResult> GetChatThreads()
    {
        var user = await adminUserManager.GetUserAsync(User);

        var chatThreads = await airaUnifiedChatService.GetThreads(user!.UserID);

        return Ok(new AiraUnifiedChatThreadsViewModel
        {
            ChatThreads = chatThreads
        });
    }


    /// <summary>
    /// Retrieves the chat thread selector view with navigation and thread management URLs.
    /// </summary>
    /// <returns>A view containing the chat thread selector interface.</returns>
    [HttpGet]
    public async Task<IActionResult> ChatThreadSelector()
    {
        var configuration = await GetConfiguration();

        var navigationUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.NavigationUrl);
        var userThreadCollectionUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.ChatThreadSelectorRelativeUrl, AiraUnifiedConstants.AllChatThreadsRelativeUrl);
        var chatUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.ChatRelativeUrl);
        var newChatThreadUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.NewChatThreadRelativeUrl);

        if (navigationUrl is null
            || userThreadCollectionUrl is null
            || chatUrl is null
            || newChatThreadUrl is null)
        {
            eventLogService.LogError(nameof(AiraUnifiedController), nameof(ChatThreadSelector), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

        var model = new ChatThreadSelectorViewModel
        {
            NavigationPageIdentifier = AiraUnifiedConstants.ChatRelativeUrl,
            AiraUnifiedBaseUrl = configuration.AiraUnifiedPathBase,
            NavigationUrl = navigationUrl.ToString(),
            UserThreadCollectionUrl = userThreadCollectionUrl.ToString(),
            ChatUrl = chatUrl.ToString(),
            ChatQueryParameterName = AiraUnifiedConstants.ChatThreadIdParameterName,
            NewChatThreadUrl = newChatThreadUrl.ToString()
        };

        return View("~/Chat/ChatThreadSelector.cshtml", model);
    }


    /// <summary>
    /// Creates a new chat thread and redirects to the chat interface.
    /// </summary>
    /// <returns>A redirect to the chat interface with the newly created thread.</returns>
    [HttpGet]
    public async Task<IActionResult> NewChatThread()
    {
        var configuration = await GetConfiguration();

        var user = await adminUserManager.GetUserAsync(User);

        await airaUnifiedChatService.CreateNewChatThread(user!.UserID);
        var chatRedirectUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.ChatRelativeUrl);

        if (chatRedirectUrl is null)
        {
            eventLogService.LogError(nameof(AiraUnifiedController), nameof(NewChatThread), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

        return Redirect(chatRedirectUrl.ToString());
    }


    /// <summary>
    /// Retrieves the navigation view model.
    /// </summary>
    /// <param name="request">The <see cref="NavBarRequestModel"/> containing the page identifier.</param>
    /// <returns>A <see cref="NavBarViewModel"/> containing the navigation view model.</returns>
    [HttpPost]
    public async Task<IActionResult> Navigation([FromBody] NavBarRequestModel request)
    {
        if (!string.Equals(request.PageIdentifier, AiraUnifiedConstants.ChatRelativeUrl) &&
            !string.Equals(request.PageIdentifier, AiraUnifiedConstants.SmartUploadRelativeUrl))
        {
            return BadRequest("The navigation menu for the specified page does not exist.");
        }

        var model = await navigationService.GetNavBarViewModel(request.PageIdentifier, baseUrl: HttpContext.Request.GetBaseUrl());

        // Only the URLs which origin from the input of the Admin user in aira unified need verification.
        if (model.ChatItem.Url is null
            || model.SmartUploadItem is null)
        {
            eventLogService.LogError(nameof(INavigationService), nameof(INavigationService.GetNavBarViewModel), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

        return Ok(model);
    }


    /// <summary>
    /// Endpoint exposing the user's chat history.
    /// </summary>
    /// <param name="chatThreadId">The chat thread id to retrieve history for.</param>
    /// <returns>A list of chat messages wrapped in <see cref="AiraUnifiedChatMessageViewModel"/>. If no history exists, initial AI messages will be added.</returns>
    [HttpGet]
    public async Task<IActionResult> GetOrCreateChatHistory(int chatThreadId)
    {
        var user = await adminUserManager.GetUserAsync(User);

        // User can not be null, because he is already checked in the AiraUnifiedEndpointDataSource middleware.
        var history = await airaUnifiedChatService.GetUserChatHistory(user!.UserID, chatThreadId);
        var chatThreadState = history.Count == 0 ? ChatStateType.Initial : ChatStateType.Returning;

        if (chatThreadState == ChatStateType.Returning && history[^1].CreatedWhen.AddDays(1) > DateTime.UtcNow)
        {
            return Ok(history);
        }

        var initialMessages = await airaUnifiedChatService.GetInitialAIMessage(chatThreadState);

        if (initialMessages is null || initialMessages.Responses is null)
        {
            history.Add(new AiraUnifiedChatMessageViewModel
            {
                ServiceUnavailable = true
            });

            return Ok(history);
        }

        var thread = await airaUnifiedChatService.GetAiraUnifiedThreadInfoOrNull(user.UserID, chatThreadId);

        var messages = initialMessages.Responses.Select(message => new AiraUnifiedChatMessageViewModel
        {
            Message = message.Content,
            Role = AiraUnifiedConstants.AiraUnifiedFrontEndChatComponentAIAssistantRoleName
        });

        history.AddRange(messages);

        if (thread is not null)
        {
            foreach (var message in initialMessages.Responses)
            {
                await airaUnifiedChatService.SaveMessage(message.Content, user.UserID, ChatRoleType.AIAssistant, thread);
            }
        }

        var lastMessage = history[^1];

        if (initialMessages.QuickOptions is not null && chatThreadState != ChatStateType.Returning)
        {
            var promptGroup = await airaUnifiedChatService.SaveAiraPrompts(user.UserID, initialMessages.QuickOptions, chatThreadId);
            lastMessage.QuickPrompts = promptGroup.QuickPrompts;
            lastMessage.QuickPromptsGroupId = promptGroup.QuickPromptsGroupId.ToString();
        }

        return Ok(history);
    }


    /// <summary>
    /// Endpoint allowing chat communication via the chat interface.
    /// </summary>
    /// <param name="request">The <see cref="IFormCollection"/> containing the chat message.</param>
    /// <param name="chatThreadId">The chat thread id to post the message to.</param>
    /// <returns>A chat message response wrapped in <see cref="AiraUnifiedChatMessageViewModel"/>. If the service is unavailable, an error message will be returned.</returns>
    [HttpPost]
    public async Task<IActionResult> PostChatMessage(IFormCollection request, int chatThreadId)
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
        var userId = user!.UserID;

        var thread = await airaUnifiedChatService.GetAiraUnifiedThreadInfoOrNull(userId, chatThreadId);

        if (thread is null)
        {
            return BadRequest($"The specified chat does not belong to user {user.UserName}.");
        }

        await airaUnifiedChatService.SaveMessage(message, userId, ChatRoleType.User, thread);

        try
        {
            var aiResponse = await airaUnifiedChatService.GetAIResponseOrNull(message, numberOfIncludedHistoryMessages: 5, userId);

            if (aiResponse is null)
            {
                result = new AiraUnifiedChatMessageViewModel
                {
                    ServiceUnavailable = true,
                };

                return Ok(result);
            }

            await airaUnifiedChatService.SaveMessages(aiResponse, userId, thread);

            await airaUnifiedChatService.UpdateChatSummary(userId, message);

            result = new AiraUnifiedChatMessageViewModel
            {
                Role = AiraUnifiedConstants.AiraUnifiedFrontEndChatComponentAIAssistantRoleName,
                Message = aiResponse.Responses[0].Content,
                Insights = aiResponse.Insights
            };

            if (aiResponse.QuickOptions is not null)
            {
                var promptGroup = await airaUnifiedChatService.SaveAiraPrompts(userId, aiResponse.QuickOptions, chatThreadId);
                result.QuickPrompts = promptGroup.QuickPrompts;
                result.QuickPromptsGroupId = promptGroup.QuickPromptsGroupId.ToString();
            }
        }
        catch (Exception ex)
        {
            result = new AiraUnifiedChatMessageViewModel
            {
                Role = AiraUnifiedConstants.AiraUnifiedFrontEndChatComponentAIAssistantRoleName,
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
    /// <returns>An empty response.</returns>
    [HttpPost]
    public IActionResult RemoveUsedPromptGroup([FromBody] AiraUnifiedUsedPromptGroupModel model)
    {
        airaUnifiedChatService.RemoveUsedPrompts(model.GroupId);

        return Ok();
    }


    /// <summary>
    /// Endpoint allowing upload of files via smart upload.
    /// </summary>
    /// <param name="request">The <see cref="IFormCollection"/> containing the uploaded files.</param>
    /// <returns>An empty response on success, or a BadRequest response if the file format is not allowed.</returns>
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
    /// <returns>A view containing the smart upload interface.</returns>
    [HttpGet]
    public async Task<IActionResult> Assets()
    {
        var configuration = await GetConfiguration();

        // Only the URLs which origin from the input of the Admin user in aira unified need verification.
        var allowedFileExtensionsUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.SmartUploadRelativeUrl, AiraUnifiedConstants.SmartUploadAllowedFileExtensionsUrl);
        var navigationUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.NavigationUrl);
        var uploadUrl = navigationService.BuildUriOrNull(configuration.BaseUrl, configuration.AiraUnifiedPathBase, AiraUnifiedConstants.SmartUploadRelativeUrl, AiraUnifiedConstants.SmartUploadUploadUrl);

        if (allowedFileExtensionsUrl is null
            || navigationUrl is null
            || uploadUrl is null)
        {
            eventLogService.LogError(nameof(AiraUnifiedController), nameof(Assets), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

        var model = new AssetsViewModel
        {
            PathBase = configuration.AiraUnifiedPathBase,
            AllowedFileExtensionsUrl = allowedFileExtensionsUrl.ToString(),
            SelectFilesButton = Resource.SmartUploadSelectFilesButton,
            FilesUploadedMessage = Resource.SmartUploadFilesUploadedMessage,
            NavigationPageIdentifier = AiraUnifiedConstants.SmartUploadRelativeUrl,
            NavigationUrl = navigationUrl.ToString(),
            UploadUrl = uploadUrl.ToString()
        };

        return View("~/AssetUploader/Assets.cshtml", model);
    }


    /// <summary>
    /// Endpoint retrieving the allowed smart upload file extensions.
    /// </summary>
    /// <returns>A list of allowed file extensions.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllowedFileExtensions()
    {
        var allowedExtensions = await airaUnifiedAssetService.GetAllowedFileExtensions();

        return Ok(allowedExtensions);
    }


    /// <summary>
    /// Endpoint retrieving the SignIn page.
    /// </summary>
    /// <returns>A view containing the SignIn page.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn()
    {
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();

        var baseUrl = HttpContext.Request.GetBaseUrl();

        // Only the URLs which origin from the input of the Admin user in aira unified need verification.
        var chatUrl = navigationService.BuildUriOrNull(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase, AiraUnifiedConstants.ChatRelativeUrl, AiraUnifiedConstants.ChatMessageUrl);

        if (chatUrl is null)
        {
            eventLogService.LogError(nameof(AiraUnifiedController), nameof(SignIn), InvalidPathBaseErrorMessage);

            return BadRequest(InvalidPathBaseErrorMessage);
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable S6932 // Kept, we are using the aira unified middleware, where we want to specify this parameter, therefore asp.net's query parameter should not be used, because we also want to reference the samme value in that middleware and we would not be able to use a variable for the query parameter.
        var missingPermission = Request.Query[AiraUnifiedConstants.SigninMissingPermissionParameterName].ToString();
#pragma warning restore S6932 //
#pragma warning restore IDE0079 // Remove unnecessary suppression

        var errorMessage = string.IsNullOrEmpty(missingPermission) ? null : $"You do not have the Aira Unified {missingPermission} permission.";

        var logoUrl = await airaUnifiedAssetService.GetSanitizedLogoUrl();

        var model = new SignInViewModel
        {
            PathBase = airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase,
            ChatUrl = AiraUnifiedConstants.ChatRelativeUrl,
            LogoImageRelativePath = logoUrl,
            ErrorMessage = errorMessage
        };

        return View("~/Authentication/SignIn.cshtml", model);
    }


    private sealed record ConfigurationModel(string BaseUrl, string AiraUnifiedPathBase);


    private async Task<ConfigurationModel> GetConfiguration()
    {
        var airaUnifiedConfiguration = await airaUnifiedConfigurationService.GetAiraUnifiedConfiguration();
        var baseUrl = HttpContext.Request.GetBaseUrl();

        return new ConfigurationModel(baseUrl, airaUnifiedConfiguration.AiraUnifiedConfigurationItemAiraPathBase);
    }

}
