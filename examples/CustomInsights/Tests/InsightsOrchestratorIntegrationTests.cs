using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using CustomInsights.Models;
using CustomInsights.Strategies;
using CustomInsights.Services;
using Microsoft.Extensions.Configuration;

namespace CustomInsights.Tests;

public class InsightsOrchestratorIntegrationTests
{
    private readonly IServiceProvider serviceProvider;

    public InsightsOrchestratorIntegrationTests()
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
        services.AddScoped<IInsightsOrchestrator, Kentico.Xperience.AiraUnified.Insights.Implementation.InsightsOrchestrator>();
        services.AddScoped<IInsightsStrategyFactory, Kentico.Xperience.AiraUnified.Insights.Implementation.InsightsStrategyFactory>();
        
        // Add custom strategy
        services.AddScoped<IInsightsStrategy, UsersInsightsStrategy>();
        
        // Add mock user service
        services.AddScoped<IUserService, MockUserService>();
        
        serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task ProcessInsightsAsync_WithValidUsersCategory_ShouldReturnSuccess()
    {
        // Arrange
        var orchestrator = serviceProvider.GetRequiredService<IInsightsOrchestrator>();
        var request = new InsightsRequest(1, "users");

        // Act
        var result = await orchestrator.ProcessInsightsAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.IsType<UsersInsightsDataModel>(result.Data);
        Assert.Equal(typeof(UsersInsightsComponent), result.ComponentType);
        Assert.NotNull(result.Metadata);
        Assert.Equal("users", result.Metadata.Category);
        Assert.True(result.Metadata.UseMockData);
    }

    [Fact]
    public async Task ProcessInsightsAsync_WithInvalidCategory_ShouldReturnNotFound()
    {
        // Arrange
        var orchestrator = serviceProvider.GetRequiredService<IInsightsOrchestrator>();
        var request = new InsightsRequest(1, "invalid");

        // Act
        var result = await orchestrator.ProcessInsightsAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No insights strategy found for category: invalid", result.ErrorMessage);
    }

    [Fact]
    public async Task ProcessInsightsAsync_WithEmptyCategory_ShouldReturnNotFound()
    {
        // Arrange
        var orchestrator = serviceProvider.GetRequiredService<IInsightsOrchestrator>();
        var request = new InsightsRequest(1, "");

        // Act
        var result = await orchestrator.ProcessInsightsAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No insights strategy found for category:", result.ErrorMessage);
    }

    [Fact]
    public async Task ProcessInsightsAsync_WithMockData_ShouldReturnMockValues()
    {
        // Arrange
        var orchestrator = serviceProvider.GetRequiredService<IInsightsOrchestrator>();
        var request = new InsightsRequest(1, "users");

        // Act
        var result = await orchestrator.ProcessInsightsAsync(request);

        // Assert
        Assert.True(result.Success);
        var data = (UsersInsightsDataModel)result.Data!;
        Assert.Equal(1250, data.Summary.TotalUsers); // Mock data values
        Assert.Equal(890, data.Summary.ActiveUsers);
        Assert.Equal(360, data.Summary.InactiveUsers);
        Assert.Equal(45, data.Summary.NewUsersThisMonth);
    }

    [Fact]
    public void StrategyFactory_ShouldRegisterUsersStrategy()
    {
        // Arrange
        var factory = serviceProvider.GetRequiredService<IInsightsStrategyFactory>();

        // Act
        var strategy = factory.GetStrategy("users");
        var availableCategories = factory.GetAvailableCategories();
        var hasStrategy = factory.HasStrategy("users");

        // Assert
        Assert.NotNull(strategy);
        Assert.IsType<UsersInsightsStrategy>(strategy);
        Assert.Contains("users", availableCategories);
        Assert.True(hasStrategy);
    }

    [Fact]
    public void StrategyFactory_WithInvalidCategory_ShouldReturnNull()
    {
        // Arrange
        var factory = serviceProvider.GetRequiredService<IInsightsStrategyFactory>();

        // Act
        var strategy = factory.GetStrategy("invalid");
        var hasStrategy = factory.HasStrategy("invalid");

        // Assert
        Assert.Null(strategy);
        Assert.False(hasStrategy);
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
                new() { Id = 2, Name = "Test User 2", Email = "test2@example.com", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-2) },
                new() { Id = 3, Name = "Test User 3", Email = "test3@example.com", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-35) }
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