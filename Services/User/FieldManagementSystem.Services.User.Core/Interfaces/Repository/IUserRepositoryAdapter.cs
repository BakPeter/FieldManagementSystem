using FieldManagementSystem.Services.User.Core.Types;

namespace FieldManagementSystem.Services.User.Core.Interfaces.Repository;

public interface IUserRepositoryAdapter
{
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();
    Task<UserEntity?> GetUserAsync(string token);
    Task<bool> CreateUserAsync(UserEntity userToAdd);
    Task<bool> UpdateUser(UserEntity userToUpdate);
    Task<bool> DeleteUser(string id);
}