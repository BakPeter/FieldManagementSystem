using System.Collections.Concurrent;
using FieldManagementSystem.Services.User.Core.Interfaces.Repository;
using FieldManagementSystem.Services.User.Core.Types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.Services.User.Infrastructure.Services.Repository;

public class UserCachedRepositoryAdapter : IUserRepositoryAdapter
{
    private readonly ConcurrentDictionary<string, UserEntity> _data = new();
    private readonly ILogger<UserCachedRepositoryAdapter> _logger;

    public UserCachedRepositoryAdapter(ILogger<UserCachedRepositoryAdapter> logger)
    {
        _data["mymail@gmail.com"] = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "mymail@gmail.com",
            UserType = UserType.Admin,
            FirstName = "John",
            LastName = "Doe",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        _data["mymail2@gmail.com"] = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "mymail2@gmail.com",
            UserType = UserType.Costumer,
            FirstName = "John2",
            LastName = "Doe2",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _logger = logger;
    }

    public Task<IEnumerable<UserEntity>> GetAllUsersAsync()
    {
        return Task.FromResult(_data.Values.AsEnumerable());
    }

    public Task<UserEntity?> GetUserAsync(string token)
    {
        _ = _data.TryGetValue(token, out var user);
        return Task.FromResult(user);
    }

    public Task<bool> CreateUserAsync(UserEntity userToAdd)
    {
        return Task.FromResult(_data.TryAdd(userToAdd.Email, userToAdd));
    }

    public Task<bool> UpdateUser(UserEntity userToUpdate)
    {
        try
        {
            _data[userToUpdate.Email] = userToUpdate;
            return Task.FromResult(true);
        }
        catch (Exception )
        {
            _logger.LogError("Failed To Update User, {userToUpdate}", userToUpdate);
            throw;
        }
    }

    public Task<bool> DeleteUser(string id)
    {
        return Task.FromResult(_data.TryRemove(id, out _));
    }
}