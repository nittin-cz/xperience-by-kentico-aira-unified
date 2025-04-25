# Configuration Options

This document provides detailed information about the configuration options available in the `AiraUnifiedOptions` class.

## Overview

The `AiraUnifiedOptions` class contains several configuration properties that can be set in your `appsettings.json` file. These options allow you to customize the behavior of the Aira Unified integration.

## Core Configuration Options

### AiraUnifiedApiSubscriptionKey

```json
"AiraUnifiedApiSubscriptionKey": "your-api-key-here"
```

Required API key for authenticating with the Aira Unified service.

### AiraUnifiedAIEndpoint

```json
"AiraUnifiedAIEndpoint": "https://your-custom-endpoint.com/api"
```

Optional custom endpoint for the AI service. If not specified, the default endpoint from `AiraUnifiedConstants` will be used.

## Development Options

### AiraUnifiedUseMockClient

```json
"AiraUnifiedUseMockClient": true
```

When set to `true`, enables a mock implementation of the AI client that returns predefined responses.

**Benefits:**
- Develop locally without requiring a valid API key
- Test functionality without consuming API quota
- Work offline without internet connectivity
- Avoid rate limiting issues during development

### AiraUnifiedUseMockInsights

```json
"AiraUnifiedUseMockInsights": true
```

When enabled, uses a mock implementation of the insights service.

**Benefits:**
- Test insights-related features without affecting production data
- Debug analytics functionality in isolation
- Avoid unnecessary data collection during development

## Security Options

## Complete Configuration Examples

### Development Configuration

```json
{
  "AiraUnifiedOptions": {
    "AiraUnifiedApiSubscriptionKey": "dev-key-here",
    "AiraUnifiedAIEndpoint": "https://dev-endpoint.com/api",
    "AiraUnifiedUseMockClient": true,
    "AiraUnifiedUseMockInsights": true
  }
}
```

### Production Configuration

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

If you encounter issues with configuration:

1. **Verify Settings:**
   - Check that your `appsettings.json` file is being loaded
   - Ensure property names match exactly (case-sensitive)
   - Verify the configuration section is registered in startup

2. **Check Logs:**
   - Review application logs for configuration errors
   - Check for missing or invalid settings
   - Look for connection issues with the AI service