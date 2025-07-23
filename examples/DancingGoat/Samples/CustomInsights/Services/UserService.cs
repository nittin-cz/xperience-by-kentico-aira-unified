using Samples.DancingGoat.Models;

namespace Samples.DancingGoat.Services;

public class UserService : IUserService
{
    public Task<IEnumerable<UserModel>> GetAllUsersAsync() => throw new NotImplementedException();

    public Task<IEnumerable<UserModel>> GetUsersByRoleAsync(string role) => throw new NotImplementedException();

    public Task<IEnumerable<UserModel>> GetActiveUsersAsync(DateTime since) => throw new NotImplementedException();
}
