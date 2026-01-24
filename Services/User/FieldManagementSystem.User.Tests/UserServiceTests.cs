using Moq;
using FieldManagementSystem.User.Core.Interfaces.Repository;
using FieldManagementSystem.User.Core.Interfaces.Validation;
using FieldManagementSystem.User.Core.Types;
using FieldManagementSystem.User.Core.Types.DTOs;
using FieldManagementSystem.User.Infrastructure.Services;
using FieldManagementSystem.User.Infrastructure.types;
using Microsoft.Extensions.Logging;

namespace FieldManagementSystem.User.Tests;

public class UserServiceTests
{
    private Mock<ILogger<UserService>> _mockLogger;
    private Mock<IUserRepository> _mockRepository;
    private Mock<IUserValidation> _mockUserValidation;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockRepository = new Mock<IUserRepository>();
        _mockUserValidation = new Mock<IUserValidation>();
        _userService = new UserService(_mockLogger.Object, _mockRepository.Object, _mockUserValidation.Object);
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _mockLogger = null;
        _mockRepository = null;
        _mockUserValidation = null;
        _userService = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    // GetUsersAsync Tests
    [Test]
    public async Task GetUsersAsync_ReturnsListOfUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<UserEntity>
        {
            new UserEntity { Id = Guid.NewGuid(), Email = "test1@example.com", FirstName = "F1", LastName = "L1" },
            new UserEntity { Id = Guid.NewGuid(), Email = "test2@example.com", FirstName = "F2", LastName = "L2" }
        };
        _mockRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Count(), Is.EqualTo(2));
        // _mockLogger.Verify(
        //     x => x.Log(
        //         LogLevel.Information,
        //         It.IsAny<EventId>(),
        //         It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Get Users - Success: True")),
        //         It.IsAny<Exception>(),
        //         (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        //     Times.Once);
    }

    [Test]
    public async Task GetUsersAsync_ReturnsEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<UserEntity>());

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data, Is.Empty);
    }

    // GetUserAsync Tests
    [Test]
    public async Task GetUserAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userId = "test@example.com";
        var user = new UserEntity { Id = Guid.NewGuid(), Email = userId, FirstName = "F1", LastName = "L1" };
        _mockRepository.Setup(r => r.GetUserAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserAsync(userId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Email, Is.EqualTo(userId));
    }

    [Test]
    public async Task GetUserAsync_ReturnsError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "nonexistent@example.com";
        _mockRepository.Setup(r => r.GetUserAsync(userId)).ReturnsAsync(null as UserEntity);

        // Act
        var result = await _userService.GetUserAsync(userId);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"User with token nonexistent@example.com not found (Parameter 'id')"));
    }

    // CreateUserAsync Tests
    [Test]
    public async Task CreateUserAsync_ReturnsUserEntity_WhenValidationPassesAndUserAdded()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "new@example.com", FirstName = "New", LastName = "User" };
        _mockUserValidation.Setup(v => v.Validate(It.IsAny<UserEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.CreateUserAsync(It.IsAny<UserEntity>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.GetUserAsync(createDto.Email)).ReturnsAsync(null as UserEntity);

        // Act
        var result = await _userService.CreateUserAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Email, Is.EqualTo(createDto.Email));
        _mockRepository.Verify(r => r.CreateUserAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Test]
    public async Task CreateUserAsync_ReturnsError_WhenValidationFails()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "invalid", FirstName = "", LastName = "U" };
        var validationErrors = new List<string> { "Email invalid", "First name too short" };
        _mockUserValidation.Setup(v => v.Validate(It.IsAny<UserEntity>(), out It.Ref<IEnumerable<string>>.IsAny))
            .Callback((UserEntity _, out IEnumerable<string> errors) => { errors = validationErrors; })
            .Returns(false);

        // Act
        var result = await _userService.CreateUserAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<UserValidationException>());
        Assert.That(((UserValidationException)result.Error).ValidationErrors, Is.EqualTo(validationErrors));
        _mockRepository.Verify(r => r.CreateUserAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Test]
    public async Task CreateUserAsync_ReturnsError_WhenUserAlreadyExists()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "existing@example.com", FirstName = "Exist", LastName = "User" };
        var existingUser = new UserEntity { Id = Guid.NewGuid(), Email = createDto.Email };
        _mockUserValidation.Setup(v => v.Validate(It.IsAny<UserEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.GetUserAsync(createDto.Email)).ReturnsAsync(existingUser); 

        // Act
        var result = await _userService.CreateUserAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"User with email {createDto.Email} exists (Parameter 'createUserDto')"));
        _mockRepository.Verify(r => r.CreateUserAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Test]
    public async Task CreateUserAsync_ReturnsError_WhenRepositoryFailsToAddUser()
    {
        // Arrange
        var createDto = new CreateUserDto { Email = "new@example.com", FirstName = "New", LastName = "User" };
        _mockUserValidation.Setup(v => v.Validate(It.IsAny<UserEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.CreateUserAsync(It.IsAny<UserEntity>())).ReturnsAsync(false); 
        _mockRepository.Setup(r => r.GetUserAsync(createDto.Email)).ReturnsAsync(null as UserEntity); 

        // Act
        var result = await _userService.CreateUserAsync(createDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Data, Is.Null);
        Assert.That(result.Error, Is.InstanceOf<Exception>());
        Assert.That(result.Error.Message, Is.EqualTo("Failed to add user to repository."));
    }

    // UpdateUser Tests
    [Test]
    public async Task UpdateUser_ReturnsSuccessMessage_WhenUserExistsAndValidationPassesAndUserUpdated()
    {
        // Arrange
        var updateDto = new UpdateUserDto { Email = "existing@example.com", FirstName = "Updated", LastName = "User" };
        var existingUser = new UserEntity { Id = Guid.NewGuid(), Email = updateDto.Email, FirstName = "Old", LastName = "Data", CreatedDate = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetUserAsync(updateDto.Email)).ReturnsAsync(existingUser);
        _mockUserValidation.Setup(v => v.Validate(It.IsAny<UserEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.UpdateUser(It.IsAny<UserEntity>())).ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUser(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.EqualTo($"User {updateDto.Email} updated"));
        _mockRepository.Verify(r => r.UpdateUser(It.Is<UserEntity>(u => u.Email == updateDto.Email && u.FirstName == updateDto.FirstName)), Times.Once);
    }

    [Test]
    public async Task UpdateUser_ReturnsError_WhenUserDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateUserDto { Email = "nonexistent@example.com", FirstName = "Updated", LastName = "User" };
        _mockRepository.Setup(r => r.GetUserAsync(updateDto.Email)).ReturnsAsync(null as UserEntity);

        // Act
        var result = await _userService.UpdateUser(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"User with email {updateDto.Email} not found (Parameter 'updateUserDto')"));
        _mockRepository.Verify(r => r.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
    }

    [Test]
    public async Task UpdateUser_ReturnsError_WhenValidationFails()
    {
        // Arrange
        var updateDto = new UpdateUserDto { Email = "existing@example.com", FirstName = "I", LastName = "nvalid" };
        var existingUser = new UserEntity { Id = Guid.NewGuid(), Email = updateDto.Email, FirstName = "Old", LastName = "Data", CreatedDate = DateTime.UtcNow };
        var validationErrors = new List<string> { "First name too short" };
        _mockRepository.Setup(r => r.GetUserAsync(updateDto.Email)).ReturnsAsync(existingUser);
        _mockUserValidation.Setup(v => v.Validate(It.IsAny<UserEntity>(), out It.Ref<IEnumerable<string>>.IsAny))
            .Callback((UserEntity _, out IEnumerable<string> errors) => { errors = validationErrors; })
            .Returns(false);

        // Act
        var result = await _userService.UpdateUser(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<UserValidationException>());
        Assert.That(((UserValidationException)result.Error).ValidationErrors, Is.EqualTo(validationErrors));
        _mockRepository.Verify(r => r.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
    }

    [Test]
    public async Task UpdateUser_ReturnsError_WhenRepositoryFailsToUpdate()
    {
        // Arrange
        var updateDto = new UpdateUserDto { Email = "existing@example.com", FirstName = "Updated", LastName = "User" };
        var existingUser = new UserEntity { Id = Guid.NewGuid(), Email = updateDto.Email, FirstName = "Old", LastName = "Data", CreatedDate = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetUserAsync(updateDto.Email)).ReturnsAsync(existingUser);
        _mockUserValidation.Setup(v => v.Validate(It.IsAny<UserEntity>(), out It.Ref<IEnumerable<string>>.IsAny)).Returns(true);
        _mockRepository.Setup(r => r.UpdateUser(It.IsAny<UserEntity>())).ReturnsAsync(false);

        // Act
        var result = await _userService.UpdateUser(updateDto);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<Exception>()); 
        Assert.That(result.Error.Message, Is.EqualTo($"User {updateDto.Email} update failed"));
        _mockRepository.Verify(r => r.UpdateUser(It.IsAny<UserEntity>()), Times.Once);
    }

    // DeleteUserAsync Tests
    [Test]
    public async Task DeleteUserAsync_ReturnsSuccessMessage_WhenUserExistsAndDeleted()
    {
        // Arrange
        var userId = "userToDelete@example.com";
        var existingUser = new UserEntity { Id = Guid.NewGuid(), Email = userId, FirstName = "To", LastName = "Delete", CreatedDate = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetUserAsync(userId)).ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.DeleteUser(userId)).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data, Is.EqualTo($"User {userId} deleted"));
        _mockRepository.Verify(r => r.DeleteUser(userId), Times.Once);
    }

    [Test]
    public async Task DeleteUserAsync_ReturnsError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "nonexistent@example.com";
        _mockRepository.Setup(r => r.GetUserAsync(userId)).ReturnsAsync(null as UserEntity);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<ArgumentException>());
        Assert.That(result.Error.Message, Is.EqualTo($"User with email {userId} not found (Parameter 'id')"));
        _mockRepository.Verify(r => r.DeleteUser(userId), Times.Never);
    }

    [Test]
    public async Task DeleteUserAsync_ReturnsError_WhenRepositoryFailsToDelete()
    {
        // Arrange
        var userId = "userToDelete@example.com";
        var existingUser = new UserEntity { Id = Guid.NewGuid(), Email = userId, FirstName = "To", LastName = "Delete", CreatedDate = DateTime.UtcNow };
        _mockRepository.Setup(r => r.GetUserAsync(userId)).ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.DeleteUser(userId)).ReturnsAsync(false); // Repository fails

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.InstanceOf<Exception>());
        Assert.That(result.Error.Message, Is.EqualTo($"User id {userId} delete failed"));
        _mockRepository.Verify(r => r.DeleteUser(userId), Times.Once);
    }
}
