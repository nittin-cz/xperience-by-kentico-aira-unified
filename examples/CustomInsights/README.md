# Custom Insights Example - Users Insights

This example demonstrates how to implement a custom insights provider using the new insights architecture in Kentico Xperience by Aira Unified.

## Structure

- **Models/**: Data models for user insights
  - `UsersInsightsDataModel.cs` - Main data model
  - `UsersSummaryModel.cs` - Summary statistics
  - `UserModel.cs` - Individual user information
  - `UserRoleStatsModel.cs` - Role distribution statistics

- **Services/**: Service interfaces and implementations
  - `IUserService.cs` - User data service interface

- **Strategies/**: Insights strategy implementations
  - `UsersInsightsStrategy.cs` - Main strategy implementation

- **Components/**: Blazor components for rendering
  - `UsersInsightsComponent.razor` - Component for displaying user insights

## Usage

### 1. Register the Custom Strategy

```csharp
// In your Startup.cs or Program.cs
services.AddKenticoAiraUnified(Configuration);

// Register your custom strategy
services.AddInsightsStrategy<UsersInsightsStrategy>();

// Register your custom services
services.AddScoped<IUserService, UserService>();
```

### 2. Using Mock Data

You can enable mock data for this insights category by adding configuration:

```json
{
  "AiraUnified": {
    "MockInsights": {
      "users": true
    }
  }
}
```

### 3. Query Users Insights

Users can now query for users insights by asking questions like:
- "Show me user insights"
- "What's the user activity status?"
- "How many users are active?"

## Features

- **Real Data Integration**: Connects to actual user data through IUserService
- **Mock Data Support**: Provides sample data for development/testing
- **Role-based Statistics**: Shows user distribution by roles
- **Activity Tracking**: Displays active vs inactive users
- **Responsive UI**: Uses the same styling as built-in insights components

## Customization

You can extend this example by:
1. Adding more user properties to the data models
2. Implementing additional filtering options in the strategy
3. Enhancing the UI component with charts or graphs
4. Adding user permission checks in the strategy
5. Implementing caching for performance optimization

## Mock Data vs Real Data

The strategy automatically switches between mock and real data based on configuration:
- Mock data provides consistent test data for development
- Real data integrates with your actual user management system
- The switch is transparent to the UI layer

## Integration with AI

The insights are automatically integrated with the AI chat system, so users can:
- Ask natural language questions about users
- Get contextual responses with the insights data
- View the data in a structured format within the chat interface