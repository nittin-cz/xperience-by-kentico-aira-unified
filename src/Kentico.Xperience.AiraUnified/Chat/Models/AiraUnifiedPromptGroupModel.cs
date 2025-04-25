namespace Kentico.Xperience.AiraUnified.Chat.Models;

/// <summary>
/// Represents a group of suggested prompts in the chat interface.
/// </summary>
internal sealed class AiraUnifiedPromptGroupModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the prompt group.
    /// </summary>
    public int QuickPromptsGroupId { get; set; }


    /// <summary>
    /// Gets or sets the collection of suggested prompt texts.
    /// </summary>
    public IEnumerable<string> QuickPrompts { get; set; } = [];
}
