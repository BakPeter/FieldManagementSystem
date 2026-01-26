using FieldManagementSystem.Services.User.Core.Types;
using FieldManagementSystem.Services.User.Infrastructure.Services.Repository;
using Microsoft.Extensions.Logging;
using Moq;

namespace FieldManagementSystem.Services.User.Tests;

[Ignore("Temporarily disabled")]
[TestFixture]
public class UserCachedRepositoryAdapterTests
{
    private Mock<ILogger<UserCachedRepositoryAdapter>> _mockLogger;
    private UserCachedRepositoryAdapter _adapter;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<UserCachedRepositoryAdapter>>();
        _adapter = new UserCachedRepositoryAdapter(_mockLogger.Object);
    }

    [Test]
    public async Task GetAllUsersAsync_WhenCalled_ReturnsAllUsers()
    {
        // Act
        var result = await _adapter.GetAllUsersAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetUserAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var email = "mymail@gmail.com";

        // Act
        var result = await _adapter.GetUserAsync(email);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(email));
    }

    [Test]
    public async Task GetUserAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexisting@gmail.com";

        // Act
        var result = await _adapter.GetUserAsync(email);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateUserAsync_WithNewUser_ReturnsTrue()
    {
        // Arrange
        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "newuser@gmail.com",
            FirstName = "New",
            LastName = "User",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = await _adapter.CreateUserAsync(newUser);
        var createdUser = await _adapter.GetUserAsync(newUser.Email);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(createdUser, Is.Not.Null);
        Assert.That(createdUser.Email, Is.EqualTo(newUser.Email));
    }

    [Test]
    public async Task CreateUserAsync_WithExistingUser_ReturnsFalse()
    {
        // Arrange
        var existingUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "mymail@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = await _adapter.CreateUserAsync(existingUser);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateUser_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        var userToUpdate = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "mymail@gmail.com",
            FirstName = "Updated",
            LastName = "User",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = await _adapter.UpdateUserAsync(userToUpdate);
        var updatedUser = await _adapter.GetUserAsync(userToUpdate.Email);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser.FirstName, Is.EqualTo("Updated"));
    }

    [Test]
    public async Task DeleteUser_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        var email = "mymail@gmail.com";

        // Act
        var result = await _adapter.DeleteUserAsync(email);
        var deletedUser = await _adapter.GetUserAsync(email);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(deletedUser, Is.Null);
    }

    [Test]
    public async Task DeleteUser_WithNonExistingEmail_ReturnsFalse()
    {
        // Arrange
        var email = "nonexisting@gmail.com";

        // Act
        var result = await _adapter.DeleteUserAsync(email);

        // Assert
        Assert.That(result, Is.False);
    }
}
