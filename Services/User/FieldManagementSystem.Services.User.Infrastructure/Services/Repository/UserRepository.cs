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

    public Task<IEnumerable<UserEntity>> GetAllUsersAsync(CancellationToken ct = default) => _adapter.GetAllUsersAsync(ct);
    public Task<UserEntity?> GetUserAsync(string id, CancellationToken ct = default) => _adapter.GetUserAsync(id, ct);

    public Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken ct = default)=> _adapter.GetUserByEmailAsync(email, ct);

    public Task<UserEntity?> CreateUserAsync(UserEntity userToAdd, CancellationToken ct = default) => _adapter.CreateUserAsync(userToAdd, ct);
    public Task<bool> UpdateUser(UserEntity userToUpdate, CancellationToken ct = default) => _adapter.UpdateUserAsync(userToUpdate, ct);
    public Task<bool> DeleteUser(string id, CancellationToken ct = default) => _adapter.DeleteUserAsync(id, ct);
}