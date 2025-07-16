using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using CustomInsights.Models;
using CustomInsights.Services;
using CustomInsights.Strategies;

namespace CustomInsights.Tests;

public class UsersInsightsStrategyTests
{
    private readonly Mock<IUserService> mockUserService;
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly Mock<ILogger<UsersInsightsStrategy>> mockLogger;
    private readonly UsersInsightsStrategy strategy;

    public UsersInsightsStrategyTests()
    {
        mockUserService = new Mock<IUserService>();
        mockConfiguration = new Mock<IConfiguration>();
        mockLogger = new Mock<ILogger<UsersInsightsStrategy>>();
        
        strategy = new UsersInsightsStrategy(
            mockUserService.Object,
            mockConfiguration.Object,
            mockLogger.Object);
    }

    [Fact]
    public void Category_ShouldReturnUsers()
    {
        // Act
        var result = strategy.Category;

        // Assert
        Assert.Equal("users", result);
    }

    [Fact]
    public void ComponentType_ShouldReturnUsersInsightsComponent()
    {
        // Act
        var result = strategy.ComponentType;

        // Assert
        Assert.Equal(typeof(UsersInsightsComponent), result);
    }

    [Fact]
    public async Task LoadRealDataAsync_ShouldReturnCorrectData()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new() { Id = 1, Name = "John", Role = "Admin", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 2, Name = "Jane", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 3, Name = "Bob", Role = "Admin", LastLogin = DateTime.UtcNow.AddDays(-35) } // Inactive
        };
        
        mockUserService.Setup(x => x.GetAllUsersAsync())
            .ReturnsAsync(users);

        var context = new InsightsContext { UserId = 1, Category = "users" };

        // Act
        var result = await strategy.LoadDataAsync(context);

        // Assert
        Assert.IsType<UsersInsightsDataModel>(result);
        var data = (UsersInsightsDataModel)result;
        Assert.Equal(3, data.Summary.TotalUsers);
        Assert.Equal(2, data.Summary.ActiveUsers); // Users with LastLogin within 30 days
        Assert.Equal(1, data.Summary.InactiveUsers);
        Assert.Equal(2, data.RoleStats.Count); // Admin and User roles
    }

    [Fact]
    public async Task LoadMockDataAsync_ShouldReturnMockData()
    {
        // Arrange
        var context = new InsightsContext { UserId = 1, Category = "users" };

        // Act
        var result = await strategy.LoadMockDataAsync(context);

        // Assert
        Assert.IsType<UsersInsightsDataModel>(result);
        var data = (UsersInsightsDataModel)result;
        Assert.Equal(1250, data.Summary.TotalUsers);
        Assert.Equal(890, data.Summary.ActiveUsers);
        Assert.Equal(360, data.Summary.InactiveUsers);
        Assert.Equal(45, data.Summary.NewUsersThisMonth);
        Assert.Equal(3, data.ActiveUsers.Count);
        Assert.Equal(3, data.RoleStats.Count);
    }

    [Fact]
    public async Task LoadRealDataAsync_WithEmptyUserList_ShouldReturnEmptyData()
    {
        // Arrange
        mockUserService.Setup(x => x.GetAllUsersAsync())
            .ReturnsAsync(new List<UserModel>());

        var context = new InsightsContext { UserId = 1, Category = "users" };

        // Act
        var result = await strategy.LoadDataAsync(context);

        // Assert
        Assert.IsType<UsersInsightsDataModel>(result);
        var data = (UsersInsightsDataModel)result;
        Assert.Equal(0, data.Summary.TotalUsers);
        Assert.Equal(0, data.Summary.ActiveUsers);
        Assert.Equal(0, data.Summary.InactiveUsers);
        Assert.Empty(data.ActiveUsers);
        Assert.Empty(data.RoleStats);
    }

    [Fact]
    public async Task LoadRealDataAsync_ShouldLimitActiveUsersToTen()
    {
        // Arrange
        var users = Enumerable.Range(1, 15)
            .Select(i => new UserModel
            {
                Id = i,
                Name = $"User{i}",
                Role = "User",
                LastLogin = DateTime.UtcNow.AddDays(-1) // All active
            })
            .ToList();
        
        mockUserService.Setup(x => x.GetAllUsersAsync())
            .ReturnsAsync(users);

        var context = new InsightsContext { UserId = 1, Category = "users" };

        // Act
        var result = await strategy.LoadDataAsync(context);

        // Assert
        Assert.IsType<UsersInsightsDataModel>(result);
        var data = (UsersInsightsDataModel)result;
        Assert.Equal(15, data.Summary.TotalUsers);
        Assert.Equal(15, data.Summary.ActiveUsers);
        Assert.Equal(10, data.ActiveUsers.Count); // Limited to 10
    }

    [Fact]
    public async Task LoadRealDataAsync_ShouldCalculateRolePercentagesCorrectly()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new() { Id = 1, Name = "Admin1", Role = "Admin", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 2, Name = "Admin2", Role = "Admin", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 3, Name = "User1", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 4, Name = "User2", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 5, Name = "User3", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 6, Name = "User4", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 7, Name = "User5", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 8, Name = "User6", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 9, Name = "User7", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 10, Name = "User8", Role = "User", LastLogin = DateTime.UtcNow.AddDays(-1) }
        };
        
        mockUserService.Setup(x => x.GetAllUsersAsync())
            .ReturnsAsync(users);

        var context = new InsightsContext { UserId = 1, Category = "users" };

        // Act
        var result = await strategy.LoadDataAsync(context);

        // Assert
        Assert.IsType<UsersInsightsDataModel>(result);
        var data = (UsersInsightsDataModel)result;
        
        var adminStats = data.RoleStats.First(r => r.RoleName == "Admin");
        var userStats = data.RoleStats.First(r => r.RoleName == "User");
        
        Assert.Equal(2, adminStats.UserCount);
        Assert.Equal(20.0M, adminStats.Percentage); // 2/10 * 100
        Assert.Equal(8, userStats.UserCount);
        Assert.Equal(80.0M, userStats.Percentage); // 8/10 * 100
    }
}