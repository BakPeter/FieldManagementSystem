using FieldManagementSystem.User.Core.DTOs;

namespace FieldManagementSystem.User.Core.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task UpdateUserAsync(string email, UpdateUserDto updateUserDto);
    Task DeleteUserAsync(string email);
}