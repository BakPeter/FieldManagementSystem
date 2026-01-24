using FieldManagementSystem.User.Core.Types;

namespace FieldManagementSystem.User.Core.Interfaces.Repository;

public interface IUserRepository
{
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();
    Task<UserEntity?> GetUserAsync(string token);
    Task<bool> CreateUserAsync(UserEntity userToAdd);
    Task<bool> UpdateUser(UserEntity userToUpdate);
    Task<bool> DeleteUser(string id);
}