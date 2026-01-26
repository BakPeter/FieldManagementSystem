using FieldManagementSystem.Services.User.Core.Types;

namespace FieldManagementSystem.Services.User.Core.Interfaces.Repository;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity>> GetAllUsersAsync(CancellationToken ct = default);
    Task<UserEntity?> GetUserAsync(string id, CancellationToken ct = default);
    Task<UserEntity?> GetUserByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> CreateUserAsync(UserEntity userToAdd, CancellationToken ct = default);
    Task<bool> UpdateUser(UserEntity userToUpdate, CancellationToken ct = default);
    Task<bool> DeleteUser(string id, CancellationToken ct = default);
}