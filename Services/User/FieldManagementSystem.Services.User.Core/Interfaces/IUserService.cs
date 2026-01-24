using FieldManagementSystem.User.Core.Types;
using FieldManagementSystem.User.Core.Types.DTOs;

namespace FieldManagementSystem.User.Core.Interfaces;

public interface IUserService
{
    Task<Result<IEnumerable<UserEntity>>> GetUsersAsync();
    Task<Result<UserEntity>> GetUserAsync(string id);
    Task<Result<UserEntity>> CreateUserAsync(CreateUserDto createUserDto);
    Task<Result<string>> UpdateUser(UpdateUserDto updateUserDto);
    Task<Result<string>> DeleteUserAsync(string id);
}