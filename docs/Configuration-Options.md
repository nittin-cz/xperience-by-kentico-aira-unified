# Configuration Options for Local Development

This document provides detailed information about the configuration options available in the `AiraUnifiedOptions` class, with a focus on settings that help developers run and test the code locally.

## Overview

The `AiraUnifiedOptions` class contains several configuration properties that can be set in your `appsettings.json` file. These options allow you to customize the behavior of the Aira Unified integration, particularly for local development and testing scenarios.

## Mock Client Options

### AiraUnifiedUseMockClient

```json
"AiraUnifiedUseMockClient": true
```

When set to `true`, this option enables a mock implementation of the AI client that returns predefined responses instead of making actual HTTP requests to the AI service.

**Benefits:**
- Develop locally without requiring a valid API key
- Test functionality without consuming API quota
- Work offline without internet connectivity
- Avoid rate limiting issues during development

**Use case:**
This is particularly useful when you're developing features that interact with the AI service but don't need to make actual API calls during development.

### AiraUnifiedUseMockInsights

```json
"AiraUnifiedUseMockInsights": true
```

When enabled, this option uses a mock implementation of the insights service that simulates analytics and reporting functionality without sending data to the actual service.

**Benefits:**
- Test insights-related features without affecting production data
- Debug analytics functionality in isolation
- Avoid unnecessary data collection during development
- Develop without requiring access to the insights service

**Use case:**
Use this when you're working on features that interact with the insights service but don't need to send actual analytics data during development.

## Custom Endpoint Configuration

### AiraUnifiedAIEndpoint

```json
"AiraUnifiedAIEndpoint": "https://your-custom-endpoint.com/api"
```

This property allows you to override the default AI service endpoint for development or testing purposes.

**Benefits:**
- Point to a local instance of the AI service
- Connect to a test environment instead of production
- Use a different API version for testing
- Route requests through a proxy for debugging

**Use case:**
Use this when you need to connect to a specific AI service endpoint that differs from the default one defined in `AiraUnifiedConstants`.

## Complete Configuration Example

Here's an example of how you might configure these options in your `appsettings.json` file for local development:

```json
{
  "AiraUnifiedOptions": {
    "AiraUnifiedApiSubscriptionKey": "your-api-key-here",
    "AiraUnifiedAIEndpoint": "https://your-custom-endpoint.com/api",
    "AiraUnifiedUseMockClient": true,
    "AiraUnifiedUseMockInsights": true
  }
}
```

## Best Practices

1. **Development Environment:**
   - Enable mock clients and insights for local development
   - Use a test API key if required
   - Consider using a local endpoint for faster development cycles

2. **Testing Environment:**
   - Use mock implementations for unit tests
   - Configure custom endpoints for integration tests
   - Ensure test data doesn't affect production systems

3. **Production Environment:**
   - Disable mock implementations
   - Use the default endpoint or a production-specific endpoint
   - Ensure proper API keys are configured

## Troubleshooting

If you encounter issues with these configuration options:

1. Verify that your `appsettings.json` file is being loaded correctly
2. Check that the property names match exactly (they are case-sensitive)
3. Ensure that the configuration section is properly registered in your application's startup code
4. For mock implementations, verify that the mock implementations are properly registered in your dependency injection container 