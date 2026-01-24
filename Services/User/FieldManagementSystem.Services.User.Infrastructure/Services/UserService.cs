using System.Text.Json;
using FieldManagementSystem.User.Core.Interfaces;
using FieldManagementSystem.User.Core.Interfaces.Repository;
using FieldManagementSystem.User.Core.Interfaces.Validation;
using FieldManagementSystem.User.Core.Types;
using FieldManagementSystem.User.Core.Types.DTOs;
using FieldManagementSystem.User.Infrastructure.types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.User.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _repository;
    private readonly IUserValidation _validation;

    public UserService(ILogger<UserService> logger, IUserRepository repository, IUserValidation validation)
    {
        _logger = logger;
        _repository = repository;
        _validation = validation;
    }

    public async Task<Result<IEnumerable<UserEntity>>> GetUsersAsync()
    {
        var users = (await _repository.GetAllUsersAsync()).ToList();
        var result = new Result<IEnumerable<UserEntity>>(true, users);
        _logger.LogInformation("Get Users - Success: {Success} Result: {result}", result.IsSuccess, JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<Result<UserEntity>> GetUserAsync(string id)
    {
        var user = await _repository.GetUserAsync(id);

        if (user == null)
        {
            var errorResult = new Result<UserEntity>(false, null,
                new ArgumentException($"User with token {id} not found", nameof(id)));

            _logger.LogInformation("Get User - token: {token} Success: {Success} Error: {Error}",
                id, errorResult.IsSuccess, errorResult.Error!.Message);

            return errorResult;
        }

        var result = new Result<UserEntity>(true, user);
        _logger.LogInformation("Get User - token: {token} Success: {Success} Result: {result}",
            id, result.IsSuccess, JsonSerializer.Serialize(result));
        return result;

    }

    public async Task<Result<UserEntity>> CreateUserAsync(CreateUserDto createUserDto)
    {
        var currentTime = DateTime.UtcNow;
        var userToAdd = new UserEntity()
        {
            Id = Guid.NewGuid(),
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            CreatedDate = currentTime,
            ModifiedDate = currentTime
        };

        if (_validation.Validate(userToAdd, out var validationErrors) is false)
            return new Result<UserEntity>(false, null, new UserValidationException(validationErrors));

        var getUserResult = await GetUserAsync(userToAdd.Email);
        if (getUserResult.IsSuccess)
            return new Result<UserEntity>(false, null,
                new ArgumentException($"User with email {createUserDto.Email} exists", nameof(createUserDto)));

        var isUserAdded = await _repository.CreateUserAsync(userToAdd);
        if (isUserAdded)
        {
            return new Result<UserEntity>(true, userToAdd);
        }
        else
        {
            // This case might need more specific error handling depending on repository
            return new Result<UserEntity>(false, null, new Exception("Failed to add user to repository."));
        }
    }

    public async Task<Result<string>> UpdateUser(UpdateUserDto updateUserDto)
    {
        var getUserResponse = await GetUserAsync(updateUserDto.Email);
        if (getUserResponse.IsSuccess is false)
            return new Result<string>(false, null,
                new ArgumentException($"User with email {updateUserDto.Email} not found", nameof(updateUserDto)));

        var user = getUserResponse.Data!;
        var updateUser = new UserEntity()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = updateUserDto.FirstName,
            LastName = updateUserDto.LastName,
            CreatedDate = user.CreatedDate,
            ModifiedDate = DateTime.UtcNow
        };

        if (_validation.Validate(updateUser, out var validationErrors) is false)
            return new Result<string>(false, "User Validation Failed", new UserValidationException(validationErrors));

        var isUserUpdated = await _repository.UpdateUser(updateUser);
        return isUserUpdated
            ? new Result<string>(true, $"User {updateUser.Email} updated")
            : new Result<string>(false, null, new Exception($"User {updateUser.Email} update failed"));
    }

    public async Task<Result<string>> DeleteUserAsync(string id)
    {
        var getUserResponse = await GetUserAsync(id);
        if (getUserResponse.IsSuccess is false)
            return new Result<string>(false, null,
                new ArgumentException($"User with email {id} not found", nameof(id)));

        var isUserDeleted = await _repository.DeleteUser(id);
        return isUserDeleted
                ? new Result<string>(isUserDeleted, $"User {id} deleted")
            : new Result<string>(false, null, new Exception($"User id {id} delete failed"));
    }
}