using FieldManagementSystem.Services.User.Core.Types;

namespace FieldManagementSystem.Services.User.Core.Interfaces.Repository;

public interface IUserRepositoryAdapter
{
    Task<IEnumerable<UserEntity>> GetAllUsersAsync(CancellationToken ct = default);
    Task<UserEntity?> GetUserAsync(string email, CancellationToken ct = default);
    Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken ct = default);
    Task<UserEntity?> CreateUserAsync(UserEntity userToAdd, CancellationToken ct = default);
    Task<bool> UpdateUserAsync(UserEntity userToUpdate, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken ct = default);
}