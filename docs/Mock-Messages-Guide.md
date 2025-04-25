# Mock Messages Guide for MockAiHttpClient

This document describes how to work with mock messages in `MockAiHttpClient.cs` for local development.

## Message Structure

`MockAiHttpClient` distinguishes between three conversation states:

1. **Initial** - first message when starting the chat
2. **Returning** - message when returning to an existing conversation
3. **Ongoing** - regular messages during conversation

## Modifying Messages

### 1. Initial Message

Modify the `GetInitialMessageResponse()` method:

```csharp
private static Task<AiraUnifiedAIResponse?> GetInitialMessageResponse() => Task.FromResult<AiraUnifiedAIResponse?>(new AiraUnifiedAIResponse
{
    Responses =
    [
        new ResponseMessageModel { Content = "MOCK: Hello! I'm your AI assistant. How can I help you today?", ContentType = "text" }
    ],
    SuggestedQuestions =
    [
        "What can you help me with?",
        "How do I create content?",
        "How do I manage contacts?"
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
        new ResponseMessageModel { Content = "MOCK: Welcome back! I'm here to help you. What would you like to know?", ContentType = "text" }
    ],
    SuggestedQuestions =
    [
        "Continue previous conversation",
        "Start new topic",
        "Show me my content"
    ]
});
```

### 3. Ongoing Messages

Modify the `GetOngoingMessageResponse()` method. This method has two main parts:

#### a) Insights Query Detection

```csharp
var isInsightsQuery = request.ChatMessage.Contains("insights", StringComparison.OrdinalIgnoreCase);
var category = isInsightsQuery ? GetInsightsCategory(request.ChatMessage) : null;
```

#### b) Insights Query Categorization

Modify the `GetInsightsCategory()` method to add new categories:

```csharp
private static string? GetInsightsCategory(string message)
{
    message = message.ToLowerInvariant();
    return message switch
    {
        var m when m.Contains("content") => "content",
        var m when m.Contains("email") => "email",
        var m when m.Contains("marketing") => "marketing",
        _ => null
    };
}
```

## Adding New Insights Category

To add a new insights category:

1. Add the new category to the `GetInsightsCategory()` method
2. Add a new case to the switch expression in `GetOngoingMessageResponse()`
3. Implement the corresponding data model

Example of adding a new category:

```csharp
// 1. Adding to GetInsightsCategory
var m when m.Contains("analytics") => "analytics",

// 2. Adding to switch expression
"analytics" => new AnalyticsInsightsDataModel
{
    // Model implementation
}
```

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

## Testing

To test different states:

1. **Initial**: Start a new conversation
2. **Returning**: Return to an existing conversation
3. **Ongoing**: 
   - For regular messages: Type any message
   - For insights: Use the keyword "insights" and the appropriate category (e.g., "content insights", "email insights", "marketing insights")

## Development Tips

1. Always keep the "MOCK:" prefix in text messages for easy identification of mock responses
2. Use collection expressions (`[ ... ]`) instead of `new List<T> { ... }`
3. Always use the 'M' suffix for decimal values (e.g., `45.5M`)
4. Use explicit cast for decimal to double conversion: `(double)45.5M` 