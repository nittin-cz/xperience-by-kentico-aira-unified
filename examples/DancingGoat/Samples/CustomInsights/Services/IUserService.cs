using Samples.DancingGoat.Models;

namespace Samples.DancingGoat.Services;

/// <summary>
/// Service for managing user data operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <returns>Collection of all users</returns>
    Task<IEnumerable<UserModel>> GetAllUsersAsync();

    /// <summary>
    /// Gets users by their role.
    /// </summary>
    /// <param name="role">The user role to filter by</param>
    /// <returns>Collection of users with the specified role</returns>
    Task<IEnumerable<UserModel>> GetUsersByRoleAsync(string role);

    /// <summary>
    /// Gets users who have logged in within the specified time period.
    /// </summary>
    /// <param name="since">The date since when to check for activity</param>
    /// <returns>Collection of active users</returns>
    Task<IEnumerable<UserModel>> GetActiveUsersAsync(DateTime since);
}
