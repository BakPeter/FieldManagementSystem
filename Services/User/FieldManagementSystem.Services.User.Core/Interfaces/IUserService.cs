using FieldManagementSystem.Services.User.Core.Types;
using FieldManagementSystem.Services.User.Core.Types.DTOs;

namespace FieldManagementSystem.Services.User.Core.Interfaces;

public interface IUserService
{
    Task<Result<IEnumerable<UserEntity>>> GetUsersAsync();
    Task<Result<UserEntity>> GetUserAsync(string id);
    Task<Result<UserEntity>> CreateUserAsync(CreateUserDto createUserDto);
    Task<Result<string>> UpdateUser(UpdateUserDto updateUserDto);
    Task<Result<string>> DeleteUserAsync(string id);
}