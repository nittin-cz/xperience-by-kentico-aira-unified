namespace Samples.DancingGoat.Models;

/// <summary>
/// Data model for users insights.
/// </summary>
public sealed class UsersInsightsDataModel
{
    /// <summary>
    /// Summary of user data
    /// </summary>
    public UsersSummaryModel Summary { get; set; } = new();

    /// <summary>
    /// List of active users
    /// </summary>
    public List<UserModel> ActiveUsers { get; set; } = new();

    /// <summary>
    /// User roles and their counts
    /// </summary>
    public List<UserRoleStatsModel> RoleStats { get; set; } = new();
}

/// <summary>
/// Summary statistics for users.
/// </summary>
public sealed class UsersSummaryModel
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
}

/// <summary>
/// Individual user information.
/// </summary>
public sealed class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime LastLogin { get; set; }
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Statistics for user roles.
/// </summary>
public sealed class UserRoleStatsModel
{
    public string RoleName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public decimal Percentage { get; set; }
}
