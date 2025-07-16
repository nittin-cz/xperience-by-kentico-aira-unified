using Kentico.Xperience.AiraUnified.Chat;
using Kentico.Xperience.AiraUnified.Chat.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Kentico.Xperience.AiraUnified.Components
{
    public partial class BlazorChatPage : ComponentBase
    {
        [Inject] private IAiraUnifiedChatService ChatService { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter] public int ThreadId { get; set; }
        [Parameter] public string ThreadName { get; set; } = string.Empty;
        [Parameter] public int UserId { get; set; }
        [Parameter] public string LogoImgRelativePath { get; set; } = string.Empty;
        [Parameter] public string BaseUrl { get; set; } = string.Empty;

        private List<AiraUnifiedChatMessageViewModel> messages = new();
        private string currentMessage = string.Empty;
        private bool isLoading = false;
        private ElementReference messagesContainer;
        private ElementReference messageInput;

        protected override async Task OnInitializedAsync()
        {
            await LoadChatHistory();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await messageInput.FocusAsync();
            }
        }

        private async Task LoadChatHistory()
        {
            try
            {
                var historyMessages = await ChatService.GetChatHistoryAsync(UserId, ThreadId);
                messages = new List<AiraUnifiedChatMessageViewModel>(historyMessages);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chat history: {ex.Message}");
            }
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(currentMessage) || isLoading)
                return;

            var messageText = currentMessage.Trim();
            currentMessage = string.Empty;
            isLoading = true;

            try
            {
                // Vytvořte novou kolekci s přidanou user zprávou
                var newMessages = new List<AiraUnifiedChatMessageViewModel>(messages)
                {
                    new()
                    {
                        Message = messageText,
                        Role = "user",
                        CreatedWhen = DateTime.Now
                    }
                };

                // Atomicky nahraďte kolekci
                messages = newMessages;
                StateHasChanged();
                await ScrollToBottom();

                // Send message and get AI response
                var aiResponse = await ChatService.SendMessageAsync(messageText, UserId, ThreadId);
                
                if (aiResponse != null)
                {
                    // Opět vytvořte novou kolekci s AI odpovědí
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

        private async Task UseQuickPrompt(string prompt, string groupId)
        {
            currentMessage = prompt;
            await SendMessage();
            
            if (!string.IsNullOrEmpty(groupId))
            {
                await ChatService.RemoveUsedPromptsAsync(groupId);
                StateHasChanged();
            }
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !e.ShiftKey)
            {
                await SendMessage();
            }
        }
        
        private void OnInputChange(ChangeEventArgs e)
        {
            currentMessage = e.Value?.ToString() ?? string.Empty;
            //StateHasChanged();
        }
        
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
}
