using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Kentico.Xperience.AiraUnified.Insights.Abstractions;
using CustomInsights.Models;
using CustomInsights.Services;
using CustomInsights.Components;

namespace CustomInsights.Strategies;

/// <summary>
/// Example implementation of custom insights strategy for user data.
/// </summary>
public sealed class UsersInsightsStrategy : InsightsStrategyBase
{
    private readonly IUserService userService;
    
    public UsersInsightsStrategy(
        IUserService userService,
        IConfiguration configuration,
        ILogger<UsersInsightsStrategy> logger) 
        : base(configuration, logger)
    {
        this.userService = userService;
    }
    
    public override string Category => "users";
    public override Type ComponentType => typeof(UsersInsightsComponent);
    
    protected override async Task<object> LoadRealDataAsync(InsightsContext context)
    {
        var allUsers = await userService.GetAllUsersAsync();
        var allUsersList = allUsers.ToList();
        var activeUsers = allUsersList.Where(u => u.LastLogin > DateTime.UtcNow.AddDays(-30)).ToList();
        var newUsers = allUsersList.Where(u => u.LastLogin > DateTime.UtcNow.AddDays(-30)).ToList(); // Mock created date with last login
        
        var roleStats = allUsersList
            .GroupBy(u => u.Role)
            .Select(g => new UserRoleStatsModel
            {
                RoleName = g.Key,
                UserCount = g.Count(),
                Percentage = (decimal)g.Count() / allUsersList.Count * 100
            })
            .ToList();
        
        return new UsersInsightsDataModel
        {
            Summary = new UsersSummaryModel
            {
                TotalUsers = allUsersList.Count,
                ActiveUsers = activeUsers.Count,
                InactiveUsers = allUsersList.Count - activeUsers.Count,
                NewUsersThisMonth = newUsers.Count
            },
            ActiveUsers = activeUsers.Take(10).ToList(),
            RoleStats = roleStats
        };
    }
    
    public override Task<object> LoadMockDataAsync(InsightsContext context)
    {
        return Task.FromResult<object>(new UsersInsightsDataModel
        {
            Summary = new UsersSummaryModel
            {
                TotalUsers = 1250,
                ActiveUsers = 890,
                InactiveUsers = 360,
                NewUsersThisMonth = 45
            },
            ActiveUsers = new List<UserModel>
            {
                new() { Id = 1, Name = "John Doe", Email = "john@example.com", LastLogin = DateTime.UtcNow.AddHours(-2), Role = "Admin" },
                new() { Id = 2, Name = "Jane Smith", Email = "jane@example.com", LastLogin = DateTime.UtcNow.AddHours(-5), Role = "Editor" },
                new() { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", LastLogin = DateTime.UtcNow.AddDays(-1), Role = "User" }
            },
            RoleStats = new List<UserRoleStatsModel>
            {
                new() { RoleName = "Admin", UserCount = 25, Percentage = 2.0M },
                new() { RoleName = "Editor", UserCount = 150, Percentage = 12.0M },
                new() { RoleName = "User", UserCount = 1075, Percentage = 86.0M }
            }
        });
    }
}