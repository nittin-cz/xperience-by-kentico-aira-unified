using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Kentico.Xperience.AiraUnified.Components;

/// <summary>
/// Blazor component for the chat page functionality.
/// </summary>
public partial class BlazorChatPage : ComponentBase
{
    /// <summary>
    /// Gets or sets the chat service.
    /// </summary>
    [Inject] private IAiraUnifiedChatService ChatService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the JavaScript runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    /// <summary>
    /// Gets or sets the thread ID for the chat.
    /// </summary>
    [Parameter] public int ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the thread name for the chat.
    /// </summary>
    [Parameter] public string ThreadName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID for the chat.
    /// </summary>
    [Parameter] public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the relative path to the logo image.
    /// </summary>
    [Parameter] public string LogoImgRelativePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL.
    /// </summary>
    [Parameter] public string BaseUrl { get; set; } = string.Empty;

    private List<AiraUnifiedChatMessageViewModel> messages = [];
    private string currentMessage = string.Empty;
    private bool isLoading = false;
    private ElementReference messagesContainer;
    private ElementReference messageInput;

    /// <summary>
    /// Initializes the component and loads chat history.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        await LoadChatHistory();
        await ScrollToBottom();
    }

    /// <summary>
    /// Executes after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">True if this is the first render, false otherwise.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await messageInput.FocusAsync();
        }
    }

    /// <summary>
    /// Loads the chat history for the current user and thread.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task LoadChatHistory()
    {
        try
        {
            var historyMessages = await ChatService.GetChatHistoryAsync(UserId, ThreadId);
            messages = [.. historyMessages];
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($@"Error loading chat history: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends a message to the chat and processes the AI response.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(currentMessage) || isLoading)
        {
            return;
        }

        var messageText = currentMessage.Trim();
        currentMessage = string.Empty;
        isLoading = true;

        try
        {
            // Create new collection with user message
            var newMessages = new List<AiraUnifiedChatMessageViewModel>(messages)
            {
                new()
                {
                    Message = messageText,
                    Role = "user",
                    CreatedWhen = DateTime.Now
                }
            };

            // Atomically replace the collection
            messages = newMessages;
            StateHasChanged();
            await ScrollToBottom();

            // Send message and get AI response
            var aiResponse = await ChatService.SendMessageAsync(messageText, UserId, ThreadId);

            if (aiResponse != null)
            {
                // Create new collection with AI response
                var updatedMessages = new List<AiraUnifiedChatMessageViewModel>(messages)
                {
                    aiResponse
                };

                messages = updatedMessages;
                StateHasChanged();
                await ScrollToBottom();
            }
        }
        catch (Exception ex)
        {
            var errorMessages = new List<AiraUnifiedChatMessageViewModel>(messages)
            {
                new()
                {
                    Message = $"Error: {ex.Message}",
                    Role = "system",
                    CreatedWhen = DateTime.Now
                }
            };
            messages = errorMessages;
            StateHasChanged();
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
            await messageInput.FocusAsync();
        }
    }


    /// <summary>
    /// Uses a quick prompt as the current message and sends it.
    /// </summary>
    /// <param name="prompt">The prompt text to use.</param>
    /// <param name="groupId">The group ID for the prompt.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task UseQuickPrompt(string prompt)
    {
        currentMessage = prompt;
        StateHasChanged();

        return Task.FromResult(0);
    }

    /// <summary>
    /// Handles key down events for the input field.
    /// </summary>
    /// <param name="e">The keyboard event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await SendMessage();
        }
    }

    /// <summary>
    /// Handles input change events.
    /// </summary>
    /// <param name="e">The change event arguments.</param>
    private void OnInputChange(ChangeEventArgs e) => currentMessage = e.Value?.ToString() ?? string.Empty;

    /// <summary>
    /// Scrolls the chat container to the bottom.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ScrollToBottom()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("scrollToBottom", messagesContainer);
        }
        catch
        {
            // Ignore JS errors
        }
    }
}
