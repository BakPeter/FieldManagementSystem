using FieldManagementSystem.User.Core.DTOs;
using FieldManagementSystem.User.Core.Interfaces;
using Repository.Core.Interfaces;

namespace FieldManagementSystem.User.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IRepository<Core.Types.User> _repository;

    public UserService(IRepository<Core.Types.User> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var user = _repository.GetAllAsync();
        return user;
    }

    public Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user == null)
        {
            return null;
        }
        var userDto = new UserDto { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName };
        return Task.FromResult(userDto);
    }

    public Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new Core.Types.User
        {
            Id = Guid.NewGuid(),
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        _users.Add(user);
        var userDto = new UserDto { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName };
        return Task.FromResult(userDto);
    }

    public Task UpdateUserAsync(string email, UpdateUserDto updateUserDto)
    {
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user != null)
        {
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.ModifiedDate = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user != null)
        {
            _users.Remove(user);
        }
        return Task.CompletedTask;
    }
}
