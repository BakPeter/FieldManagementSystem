using FieldManagementSystem.Services.User.Core.Interfaces.Repository;
using FieldManagementSystem.Services.User.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.User.Infrastructure.Services.Repository;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IUserRepositoryAdapter _adapter;

    public UserRepository(ILogger<UserRepository> logger, IUserRepositoryAdapter adapter)
    {
        _logger = logger;
        _adapter = adapter;
    }

    public Task<IEnumerable<UserEntity>> GetAllUsersAsync() => _adapter.GetAllUsersAsync();
    public Task<UserEntity?> GetUserAsync(string token) => _adapter.GetUserAsync(token);
    public Task<bool> CreateUserAsync(UserEntity userToAdd) => _adapter.CreateUserAsync(userToAdd);
    public Task<bool> UpdateUser(UserEntity userToUpdate) => _adapter.UpdateUser(userToUpdate);
    public Task<bool> DeleteUser(string id) => _adapter.DeleteUser(id);
}