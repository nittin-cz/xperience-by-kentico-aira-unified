using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using Kentico.Xperience.AiraUnified.Insights.Models;
using Kentico.Xperience.AiraUnified.Chat.Services;
using CustomInsights.Models;
using CustomInsights.Strategies;
using CustomInsights.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace CustomInsights.Tests;

public class EnhancedInsightsParserTests
{
    private readonly IServiceProvider serviceProvider;
    private readonly EnhancedInsightsParser parser;

    public EnhancedInsightsParserTests()
    {
        var services = new ServiceCollection();
        
        // Configure logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        // Configure configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AiraUnified:MockInsights:users"] = "true"
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        
        // Add insights architecture
        services.AddScoped<IInsightsStrategyFactory, Kentico.Xperience.AiraUnified.Insights.Implementation.InsightsStrategyFactory>();
        
        // Add custom strategy
        services.AddScoped<IInsightsStrategy, UsersInsightsStrategy>();
        
        // Add mock user service
        services.AddScoped<IUserService, MockUserService>();
        
        // Add enhanced parser
        services.AddScoped<EnhancedInsightsParser>();
        
        serviceProvider = services.BuildServiceProvider();
        parser = serviceProvider.GetRequiredService<EnhancedInsightsParser>();
    }

    [Fact]
    public void ParseSystemMessage_WithEnhancedFormat_ShouldDeserializeCorrectly()
    {
        // Arrange
        var originalData = new UsersInsightsDataModel
        {
            Summary = new UsersSummaryModel { TotalUsers = 100, ActiveUsers = 80 },
            ActiveUsers = new List<UserModel>
            {
                new() { Id = 1, Name = "Test User", Email = "test@example.com", Role = "Admin" }
            }
        };

        var enhancedModel = new InsightsSerializationModel
        {
            Version = 2,
            IsInsightsQuery = true,
            Category = "users",
            DataType = typeof(UsersInsightsDataModel).AssemblyQualifiedName,
            ComponentType = typeof(UsersInsightsComponent).AssemblyQualifiedName,
            InsightsData = JsonSerializer.Serialize(originalData),
            Metadata = new InsightsMetadataModel { Timestamp = DateTime.UtcNow }
        };

        var json = JsonSerializer.Serialize(enhancedModel, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act
        var (category, data, timestamp, componentType) = parser.ParseSystemMessage(json);

        // Assert
        Assert.Equal("users", category);
        Assert.NotNull(data);
        Assert.IsType<UsersInsightsDataModel>(data);
        Assert.Equal(typeof(UsersInsightsComponent), componentType);
        Assert.NotNull(timestamp);

        var usersData = (UsersInsightsDataModel)data;
        Assert.Equal(100, usersData.Summary.TotalUsers);
        Assert.Equal(80, usersData.Summary.ActiveUsers);
        Assert.Single(usersData.ActiveUsers);
        Assert.Equal("Test User", usersData.ActiveUsers[0].Name);
    }

    [Fact]
    public void ParseSystemMessage_WithLegacyFormat_ShouldFallbackCorrectly()
    {
        // Arrange
        var legacyJson = @"{
            ""category"": ""users"",
            ""insightsData"": {
                ""summary"": {
                    ""totalUsers"": 50,
                    ""activeUsers"": 40
                },
                ""activeUsers"": []
            },
            ""metadata"": {
                ""timestamp"": ""2023-01-01T00:00:00Z""
            }
        }";

        // Act
        var (category, data, timestamp, componentType) = parser.ParseSystemMessage(legacyJson);

        // Assert
        Assert.Equal("users", category);
        Assert.NotNull(data);
        Assert.Equal(typeof(UsersInsightsComponent), componentType);
        Assert.NotNull(timestamp);
    }

    [Fact]
    public void ParseSystemMessage_WithInvalidData_ShouldReturnFallback()
    {
        // Arrange
        var invalidJson = @"{
            ""version"": 2,
            ""category"": ""users"",
            ""dataType"": ""Invalid.Type.Name"",
            ""insightsData"": ""invalid json data""
        }";

        // Act
        var (category, data, timestamp, componentType) = parser.ParseSystemMessage(invalidJson);

        // Assert
        Assert.Equal("users", category);
        Assert.NotNull(data);
        Assert.IsType<FallbackInsightsData>(data);
        Assert.Equal(typeof(UsersInsightsComponent), componentType);
        
        var fallbackData = (FallbackInsightsData)data;
        Assert.Equal("users", fallbackData.Category);
        Assert.Contains("Failed to deserialize", fallbackData.Error);
    }

    [Fact]
    public void ParseSystemMessage_WithUnknownCategory_ShouldReturnFallback()
    {
        // Arrange
        var unknownCategoryJson = @"{
            ""version"": 2,
            ""category"": ""unknown"",
            ""insightsData"": ""{}\""
        }";

        // Act
        var (category, data, timestamp, componentType) = parser.ParseSystemMessage(unknownCategoryJson);

        // Assert
        Assert.Equal("unknown", category);
        Assert.NotNull(data);
        Assert.IsType<FallbackInsightsData>(data);
        Assert.Null(componentType); // No component type for unknown category
    }

    [Fact]
    public void ParseSystemMessage_WithCompletelyInvalidJson_ShouldReturnNull()
    {
        // Arrange
        var invalidJson = "not json at all";

        // Act
        var (category, data, timestamp, componentType) = parser.ParseSystemMessage(invalidJson);

        // Assert
        Assert.Null(category);
        Assert.Null(data);
        Assert.Null(timestamp);
        Assert.Null(componentType);
    }

    [Fact]
    public void ParseSystemMessage_WithMissingCategory_ShouldReturnNull()
    {
        // Arrange
        var noCategoryJson = @"{
            ""version"": 2,
            ""insightsData"": ""{}""
        }";

        // Act
        var (category, data, timestamp, componentType) = parser.ParseSystemMessage(noCategoryJson);

        // Assert
        Assert.Null(category);
        Assert.Null(data);
        Assert.Null(timestamp);
        Assert.Null(componentType);
    }

    /// <summary>
    /// Mock implementation of IUserService for testing
    /// </summary>
    private class MockUserService : IUserService
    {
        public Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            var users = new List<UserModel>
            {
                new() { Id = 1, Name = "Test User 1", Email = "test1@example.com", Role = "Admin", LastLogin = DateTime.UtcNow.AddDays(-1) },
                new() { Id = 2, Name = "Test User 2", Email = "test2@example.com", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-2) }
            };
            
            return Task.FromResult<IEnumerable<UserModel>>(users);
        }

        public Task<IEnumerable<UserModel>> GetUsersByRoleAsync(string role)
        {
            return GetAllUsersAsync().ContinueWith(task => 
                task.Result.Where(u => u.Role == role));
        }

        public Task<IEnumerable<UserModel>> GetActiveUsersAsync(DateTime since)
        {
            return GetAllUsersAsync().ContinueWith(task => 
                task.Result.Where(u => u.LastLogin > since));
        }
    }
}