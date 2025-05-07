# Mock Messages Guide for MockAiHttpClient

This document describes how to work with mock messages in `MockAiHttpClient.cs` for local development.

## Message Structure

`MockAiHttpClient` distinguishes between three conversation states:

1. **Initial** - first message when starting the chat.
2. **Returning** - message when returning to an existing conversation.
3. **Ongoing** - regular messages during conversation.

## Modifying Messages

### 1. Initial Message

Modify the `GetInitialMessageResponse()` method:

```csharp
private static Task<AiraUnifiedAIResponse?> GetInitialMessageResponse() => Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
{
    Responses =
    [
        new ResponseMessageModel 
        { 
            Content = "MOCK: Hello! I'm your AI assistant. How can I help you today?", 
            ContentType = "text",
            Metadata = new Dictionary<string, string>
            {
                { "timestamp", DateTime.UtcNow.ToString("O") },
                { "version", "1.0.0" }
            }
        }
    ],
    SuggestedQuestions =
    [
        "What can you help me with?",
        "How do I create content?",
        "How do I manage contacts?",
        "Show me content insights"
    ]
});
```

### 2. Returning Message

Modify the `GetReturningMessageResponse()` method:

```csharp
private static Task<AiraUnifiedAIResponse?> GetReturningMessageResponse() => Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
{
    Responses =
    [
        new ResponseMessageModel 
        { 
            Content = "MOCK: Welcome back! I'm here to help you. What would you like to know?", 
            ContentType = "text",
            Metadata = new Dictionary<string, string>
            {
                { "timestamp", DateTime.UtcNow.ToString("O") },
                { "version", "1.0.0" }
            }
        }
    ],
    SuggestedQuestions =
    [
        "Continue previous conversation",
        "Start new topic",
        "Show me my content",
        "Check marketing insights"
    ]
});
```

### 3. Ongoing Messages

Modify the `GetOngoingMessageResponse()` method. This method has several response types:

#### a) Regular Text Response

```csharp
new ResponseMessageModel 
{ 
    Content = "MOCK: Here's how you can do that...", 
    ContentType = "text",
    Metadata = new Dictionary<string, string>
    {
        { "timestamp", DateTime.UtcNow.ToString("O") },
        { "version", "1.0.0" }
    }
}
```

#### b) Insights Response

```csharp
new ResponseMessageModel 
{ 
    Content = JsonSerializer.Serialize(new ContentInsightsDataModel
    {
        Summary = new ContentSummaryModel
        {
            DraftCount = 5,
            ScheduledCount = 3,
            PublishedCount = 12,
            TotalCount = 20
        },
        ReusableContent = new ContentCategoryModel
        {
            Name = "Reusable Content",
            Count = 8,
            LastUpdated = DateTime.UtcNow.AddDays(-1)
        },
        WebsiteContent = new ContentCategoryModel
        {
            Name = "Website Content",
            Count = 12,
            LastUpdated = DateTime.UtcNow
        }
    }),
    ContentType = "insights",
    Metadata = new Dictionary<string, string>
    {
        { "timestamp", DateTime.UtcNow.ToString("O") },
        { "version", "1.0.0" },
        { "type", "content" }
    }
}
```

#### c) Error Response

```csharp
new ResponseMessageModel 
{ 
    Content = "MOCK: I'm sorry, I couldn't process your request.", 
    ContentType = "error",
    Metadata = new Dictionary<string, string>
    {
        { "timestamp", DateTime.UtcNow.ToString("O") },
        { "version", "1.0.0" },
        { "errorCode", "INVALID_REQUEST" }
    }
}
```

## Adding New Response Types

To add a new response type:

1. Define the response model.
2. Add a new case to the switch expression in `GetOngoingMessageResponse()`.
3. Update the response handling in the frontend.

Example of adding a new response type:

```csharp
// 1. Define the model
public class CustomResponseModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
}

// 2. Add to switch expression
"custom" => new ResponseMessageModel
{
    Content = JsonSerializer.Serialize(new CustomResponseModel
    {
        Title = "Custom Response",
        Description = "This is a custom response",
        Timestamp = DateTime.UtcNow
    }),
    ContentType = "custom",
    Metadata = new Dictionary<string, string>
    {
        { "timestamp", DateTime.UtcNow.ToString("O") },
        { "version", "1.0.0" }
    }
}
```

## Testing Different Scenarios

To test different scenarios:

1. **Initial State**
   - Start a new conversation.
   - Verify welcome message.
   - Check suggested questions.

2. **Returning State**
   - Return to an existing conversation.
   - Verify returning message.
   - Check conversation history.

3. **Ongoing State**
   - Regular messages: Type any text.
   - Insights: Use keywords like "content", "marketing", "email".
   - Errors: Use invalid input or trigger error conditions.

## Development Tips

1. Always keep the "MOCK:" prefix in text messages for easy identification of mock responses.
2. Use collection expressions (`[ ... ]`) instead of `new List<T> { ... }`.
3. Include timestamps in metadata.
4. Use consistent versioning.
5. Add appropriate error handling.
6. Test all response types.
7. Document new response types.
8. Always use the 'M' suffix for decimal values (e.g., `45.5M`).
9. Use explicit cast for decimal to double conversion: `(double)45.5M`.

## Insights Data Structure

### Content Insights
```csharp
new ContentInsightsDataModel
{
    Summary = new ContentSummaryModel
    {
        DraftCount = 5,
        ScheduledCount = 3,
        PublishedCount = 12,
        TotalCount = 20
    },
    ReusableContent = new ContentCategoryModel { ... },
    WebsiteContent = new ContentCategoryModel { ... }
}
```

### Marketing Insights
```csharp
new MarketingInsightsDataModel
{
    Contacts = new ContactsSummaryModel { ... },
    ContactGroups = [ ... ]
}
```

### Email Insights
```csharp
new EmailInsightsDataModel
{
    Summary = new EmailSummaryModel { ... },
    Campaigns = [ ... ]
}
```